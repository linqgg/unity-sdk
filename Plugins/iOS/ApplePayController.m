#import <Foundation/Foundation.h>
#import "ApplePayController.h"

typedef void (*messageDelegate)(const bool status, const char* message);

static NSString* stringFromChar(const char *string) {
    return string ? [NSString stringWithUTF8String:string] : nil;
}

@interface ApplePayController : NSObject<PKPaymentAuthorizationControllerDelegate>

@property (nonatomic) BOOL completed;
@property (nonatomic) NSDictionary * _Nonnull context;
@property (nonatomic) messageDelegate _Nullable notify;
@property (nonatomic, strong) PKPaymentAuthorizationController * _Nullable paymentSheet;
@property (nonatomic, copy) void (^__strong _Nonnull completion)(PKPaymentAuthorizationResult * _Nonnull __strong);

@end

@implementation ApplePayController

static const PKMerchantCapability PKMerchantCapabilityUnknown = 9999;
static const PKPaymentNetwork PKPaymentNetworkUnknown = 0;
static const PKContactField PKContactFieldUnknown = 0;

+ (instancetype) sharedInstance
{
    static id sharedInstance = nil;

    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });

    return sharedInstance;
}

- (void) askPaymentSheet: (messageDelegate) notifier
                 context: (NSString *) context
                  config: (NSString *) config
{
    self.notify = notifier;
    self.completed = NO;

    NSError *error;

    NSData *contextString = [context dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *contextData = [NSJSONSerialization JSONObjectWithData:contextString options:kNilOptions error:&error];

    if (error) {
        self.notify(false, [@"Invalid JSON with ApplePay context with creds is passed" UTF8String]);
        self.completed = YES;
        return;
    }

    self.context = contextData;

    NSData *configString = [config dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *configData = [NSJSONSerialization JSONObjectWithData:configString options:kNilOptions error:&error];

    if (error) {
        self.notify(false, [@"Invalid JSON with ApplePay server configuration is passed" UTF8String]);
        self.completed = YES;
        return;
    }

    PKPaymentRequest *paymentRequest = [[PKPaymentRequest alloc] init];

    paymentRequest.countryCode = [self parseCountryCode:configData];
    paymentRequest.currencyCode = [self parseCurrencyCode:configData];
    paymentRequest.supportedNetworks = [self parseSupportedNetworks:configData];
    paymentRequest.merchantIdentifier = [self parseMerchantIdentifier:configData];
    paymentRequest.paymentSummaryItems = [self parsePaymentSummaryItems:configData];
    paymentRequest.merchantCapabilities = [self parseMerchantCapabilities:configData];
    paymentRequest.requiredBillingContactFields = [self parseRequiredBillingContactFields:configData];

    self.paymentSheet = [[PKPaymentAuthorizationController alloc] initWithPaymentRequest:paymentRequest];
    self.paymentSheet.delegate = self;

    if (!self.paymentSheet) {
        self.notify(false, [@"Failed initializing payment sheet, check your ApplePay configuration" UTF8String]);
        self.completed = YES;
        return;
    }

    [self.paymentSheet presentWithCompletion:^(BOOL success) {
        if (success) {
            NSLog(@"Payment sheet is presented to the player");
        }
    }];
}

- (void) paymentAuthorizationController:(PKPaymentAuthorizationController *)controller
                    didAuthorizePayment:(PKPayment *)payment
                                handler:(void (^)(PKPaymentAuthorizationResult *result))completion
{
    self.completion = completion;

    NSMutableDictionary *payload = [NSMutableDictionary dictionary];

    NSMutableDictionary *session = [NSMutableDictionary dictionary];
    NSString *paymentData64 = [payment.token.paymentData base64EncodedStringWithOptions:0];
    NSData *decodedPaymentData = [[NSData alloc] initWithBase64EncodedString:paymentData64 options:0];
    session[@"paymentData"] = [[NSString alloc] initWithData:decodedPaymentData encoding:NSUTF8StringEncoding];

    CNPostalAddress *postalAddress = payment.billingContact.postalAddress;
    NSMutableDictionary *address = [NSMutableDictionary dictionary];
    address[@"country"] = postalAddress.ISOCountryCode;
    address[@"region"] = postalAddress.state;
    address[@"city"] = postalAddress.city;
    address[@"street"] = postalAddress.street;
    address[@"zip"] = postalAddress.postalCode;

    payload[@"orderId"] = self.context[@"reference"];
    payload[@"address"] = address;
    payload[@"applePayPayment"] = session;
    payload[@"cardTokenexPayment"] = nil;

    NSData *data = [NSJSONSerialization dataWithJSONObject:payload options:NSJSONWritingPrettyPrinted error:nil];

    NSURLSession *handler = [NSURLSession sharedSession];

    NSURLSessionDataTask *dataTask = [handler dataTaskWithRequest:[self preparePaymentValidationRequest:data] completionHandler:^(NSData *data, NSURLResponse *response, NSError *error)
    {
        NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)response;

        NSString *json = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];

        if (httpResponse.statusCode != 200) {
            self.completion([[PKPaymentAuthorizationResult alloc] initWithStatus:PKPaymentAuthorizationStatusFailure errors:nil]);
            self.notify(false, [[NSString stringWithFormat:@"Payment validation failed with error: %@", json] UTF8String]);
            self.completed = YES;
            return;
        }

        NSError *parseError;
        NSDictionary *answer = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:&parseError];

        if (!answer[@"success"]) {
            self.completion([[PKPaymentAuthorizationResult alloc] initWithStatus:PKPaymentAuthorizationStatusFailure errors:nil]);
            self.notify(false, [[NSString stringWithFormat:@"Payment validation failed with answer: %@", json] UTF8String]);
            self.completed = YES;
            return;
        }

        self.completion([[PKPaymentAuthorizationResult alloc] initWithStatus:PKPaymentAuthorizationStatusSuccess errors:nil]);
        self.notify(true, [json UTF8String]);
        self.completed = YES;
    }];

    [dataTask resume];
}

- (void) paymentAuthorizationControllerDidFinish:(PKPaymentAuthorizationController *)controller
{
    [controller dismissWithCompletion:^{
        if (!self.completed) {
            NSLog(@"Payment sheet is closed as payment cancelled by user.");
            self.notify(true, [@"" UTF8String]);
            self.completed = YES;
        }
    }];
}

// ------------------------HELPERS------------------------

- (NSMutableURLRequest *) preparePaymentValidationRequest: (NSData *) payload
{
    NSString *remoteUrl = self.context[@"remoteUrl"];
    NSString *secretKey = self.context[@"secretKey"];

    NSString *service = @"linq.money.payments.v1.NativePaymentsService/MakePayment";
    NSString *url = [NSString stringWithFormat:@"%@/%@", remoteUrl, service];

    NSMutableDictionary *headers = [NSMutableDictionary dictionary];
    headers[@"Authorization"] = [NSString stringWithFormat:@"Bearer %@", secretKey];
    headers[@"Content-Type"] = @"application/json";

    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] init];

    [request setURL:[NSURL URLWithString:url]];
    [request setHTTPMethod:@"POST"];
    [request setHTTPBody:payload];
    for (NSString *header in headers) {
        [request setValue:[headers valueForKey:header] forHTTPHeaderField:header];
    }

    return request;
}

- (NSString *) parseMerchantIdentifier: (NSDictionary *) data
{
    return data[@"MerchantId"];
}

- (NSString *) parseCountryCode: (NSDictionary *) data
{
    return data[@"CountryCode"];
}

- (NSString *) parseCurrencyCode: (NSDictionary *) data
{
    return data[@"OrderCurrency"];
}

- (NSMutableArray *) parseSupportedNetworks: (NSDictionary *) data
{
    NSMutableArray *supportedNetworks = [NSMutableArray array];

    for (NSString *supportedNetworkString in data[@"SupportedNetworks"]) {
        PKPaymentNetwork paymentNetwork = [self paymentNetworkFromString:supportedNetworkString];
        if (paymentNetwork != PKPaymentNetworkUnknown) {
            [supportedNetworks addObject:paymentNetwork];
        }
    }

    return supportedNetworks;
}

- (PKMerchantCapability) parseMerchantCapabilities: (NSDictionary *) data
{
    PKMerchantCapability merchantCapability = 0;

    NSArray *merchantCapabilities = data[@"MerchantCapabilities"];
    if (merchantCapabilities.count > 0) {
        for (NSString *merchantCapabilityString in merchantCapabilities) {
            PKMerchantCapability capability = [self merchantCapabilityFromString:merchantCapabilityString];
            if (capability != PKMerchantCapabilityUnknown) {
                merchantCapability |= capability;
            }
        }
    }

    return merchantCapability;
}

- (NSSet *) parseRequiredBillingContactFields: (NSDictionary *) data
{
    NSMutableArray *requiredBillingContactFields = [NSMutableArray array];

    for (NSString *requiredBillingContactFieldString in data[@"RequiredBillingContactFields"]) {
        PKContactField contactField = [self contactFieldFromString:requiredBillingContactFieldString];
        if (contactField != PKContactFieldUnknown) {
            [requiredBillingContactFields addObject:contactField];
        }
    }

    return [NSSet setWithArray:requiredBillingContactFields];
}

- (NSMutableArray *) parsePaymentSummaryItems: (NSDictionary *) data
{
    NSMutableArray <PKPaymentSummaryItem *> *paymentSummaryItems = [NSMutableArray array];

    NSNumber *orderAmount = data[@"OrderAmount"];
    NSString *orderLabel = data[@"LineItemLabel"];

    NSDecimalNumber *ratio = [NSDecimalNumber decimalNumberWithDecimal:[@100 decimalValue]];
    NSDecimalNumber *coins = [NSDecimalNumber decimalNumberWithDecimal:[orderAmount decimalValue]];
    NSDecimalNumber *amount = [coins decimalNumberByDividingBy:ratio];

    PKPaymentSummaryItem *paymentSummaryItem = [PKPaymentSummaryItem summaryItemWithLabel:orderLabel amount:amount];

    [paymentSummaryItems addObject: paymentSummaryItem];

    return paymentSummaryItems;
}

- (PKContactField) contactFieldFromString:(NSString *) contactFieldString
{
    NSDictionary *contactFieldsMap = @{
        @"name": PKContactFieldName,
        @"emailAddress": PKContactFieldEmailAddress,
        @"phoneNumber": PKContactFieldPhoneNumber,
        @"phoneticName": PKContactFieldPhoneticName,
        @"postalAddress": PKContactFieldPostalAddress,
    };

    return contactFieldsMap[contactFieldString]
        ? contactFieldsMap[contactFieldString]
        : PKContactFieldUnknown;
}

- (PKPaymentNetwork) paymentNetworkFromString:(NSString *)paymentNetwork
{
    NSDictionary *paymentNetworksMap = @{
        @"visa": PKPaymentNetworkVisa,
        @"amex": PKPaymentNetworkAmex,
        @"discover": PKPaymentNetworkDiscover,
        @"masterCard": PKPaymentNetworkMasterCard
    };

    return paymentNetworksMap[paymentNetwork]
        ? paymentNetworksMap[paymentNetwork]
        : PKPaymentNetworkUnknown;
}

- (PKMerchantCapability) merchantCapabilityFromString:(NSString *)merchantCapabilityString
{
    NSDictionary *merchantCapabilitiesMap = @{
        @"supports3DS": @(PKMerchantCapability3DS),
        @"supportsEMV": @(PKMerchantCapabilityEMV),
        @"supportsDebit": @(PKMerchantCapabilityDebit),
        @"supportsCredit": @(PKMerchantCapabilityCredit),
    };

    NSNumber *merchantCapabilityNumber = merchantCapabilitiesMap[merchantCapabilityString];

    if (merchantCapabilityNumber != nil) {
        return (PKMerchantCapability)merchantCapabilityNumber.unsignedLongValue;
    }

    return PKMerchantCapabilityUnknown;
}

@end

extern bool _canMakePayments() {
    return [PKPaymentAuthorizationViewController canMakePayments];
}

extern void _askPaymentSheet(messageDelegate notifier, const char* context, const char* config) {
    [[ApplePayController sharedInstance] askPaymentSheet:notifier context:stringFromChar(context) config:stringFromChar(config)];
}
