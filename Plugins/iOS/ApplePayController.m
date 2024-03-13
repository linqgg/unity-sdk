#import <Foundation/Foundation.h>
#import "ApplePayController.h"

typedef void (*messageDelegate)(const bool status, const char* message);

static NSString* stringFromChar(const char *string) {
    return string ? [NSString stringWithUTF8String:string] : nil;
}

@interface ApplePayController : NSObject<PKPaymentAuthorizationControllerDelegate>

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
                  config: (NSString *) config
{
    self.notify = notifier;

    NSError *error;
    NSData *json = [config dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *data = [NSJSONSerialization JSONObjectWithData:json options:kNilOptions error:&error];

    if (error) {
        self.notify(false, [@"Invalid JSON with ApplePay server configuration is passed" UTF8String]);
        return;
    }

    PKPaymentRequest *paymentRequest = [[PKPaymentRequest alloc] init];

    paymentRequest.countryCode = [self parseCountryCode:data];
    paymentRequest.currencyCode = [self parseCurrencyCode:data];
    paymentRequest.supportedNetworks = [self parseSupportedNetworks:data];
    paymentRequest.merchantIdentifier = [self parseMerchantIdentifier:data];
    paymentRequest.paymentSummaryItems = [self parsePaymentSummaryItems:data];
    paymentRequest.merchantCapabilities = [self parseMerchantCapabilities:data];
    paymentRequest.requiredBillingContactFields = [self parseRequiredBillingContactFields:data];

    self.paymentSheet = [[PKPaymentAuthorizationController alloc] initWithPaymentRequest:paymentRequest];
    self.paymentSheet.delegate = self;

    if (!self.paymentSheet) {
        self.notify(false, [@"Failed initializing payment sheet, check your ApplePay configuration" UTF8String]);
        return;
    }

    [self.paymentSheet presentWithCompletion:^(BOOL success) {
        if (success) {
            NSLog(@"Payment sheet is presented to the player");
        }
    }];
}

- (void) putConfirmation: (messageDelegate) notifier
                  result: (bool) result
{
    PKPaymentAuthorizationStatus status = result == true
        ? PKPaymentAuthorizationStatusSuccess
        : PKPaymentAuthorizationStatusFailure;

    NSArray<NSError *> *errors;
    self.completion([[PKPaymentAuthorizationResult alloc] initWithStatus:status errors:errors]);

    if (errors.count > 0) {
        self.notify(false, [@"Payment could not be confirmed due to errors" UTF8String]);
    }
}

- (void) paymentAuthorizationController:(PKPaymentAuthorizationViewController *)controller
                    didAuthorizePayment:(PKPayment *)payment
                                handler:(void (^)(PKPaymentAuthorizationResult *result))completion
{
    self.completion = completion;

    NSMutableDictionary *paymentDict = [NSMutableDictionary dictionary];
    NSMutableDictionary *tokenDict = [NSMutableDictionary dictionary];
    tokenDict[@"transactionIdentifier"] = payment.token.transactionIdentifier;

    NSString *paymentData64 = [payment.token.paymentData base64EncodedStringWithOptions:0];
    NSData *decodedPaymentData = [[NSData alloc] initWithBase64EncodedString:paymentData64 options:0];
    tokenDict[@"paymentData"] = [[NSString alloc] initWithData:decodedPaymentData encoding:NSUTF8StringEncoding];

    NSMutableDictionary *paymentMethodDict = [NSMutableDictionary dictionary];
    paymentMethodDict[@"displayName"] = payment.token.paymentMethod.displayName;
    paymentMethodDict[@"network"] = payment.token.paymentMethod.network;
    paymentMethodDict[@"type"] = [self stringFromPaymentMethodType:payment.token.paymentMethod.type];

    tokenDict[@"paymentMethod"] = paymentMethodDict;
    paymentDict[@"token"] = tokenDict;

    PKContact *billingContact = payment.billingContact;
    if (billingContact) {
        NSMutableDictionary *billingContactDict = [NSMutableDictionary dictionary];

        CNPostalAddress *postalAddress = billingContact.postalAddress;
        NSMutableDictionary *postalAddressDict = [NSMutableDictionary dictionary];
        if (postalAddress) {
            postalAddressDict[@"street"] = postalAddress.street;
            postalAddressDict[@"city"] = postalAddress.city;
            postalAddressDict[@"state"] = postalAddress.state;
            postalAddressDict[@"postalCode"] = postalAddress.postalCode;
            postalAddressDict[@"country"] = postalAddress.country;
            postalAddressDict[@"isoCountryCode"] = postalAddress.ISOCountryCode;
        }

        billingContactDict[@"postalAddress"] = postalAddressDict;

        paymentDict[@"billingContact"] = billingContactDict;
    }

    NSError *error;
    NSData *data = [NSJSONSerialization dataWithJSONObject:paymentDict options:NSJSONWritingPrettyPrinted error:&error];

    if (!data) {
        self.notify(false, [@"Failed to serialize PKPayment to JSON." UTF8String]);
    } else {
        NSString *json = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        self.notify(true, [json UTF8String]);
    }
}

- (void) paymentAuthorizationControllerDidFinish:(PKPaymentAuthorizationController *)controller
{
    [controller dismissWithCompletion:^{
        NSLog(@"Payment process canceled by user."); // need to notify back?
        self.notify(NO, [@"Payment process canceled by user." UTF8String]);
    }];
}

// ------------------------HELPERS------------------------

- (NSString *) parseMerchantIdentifier: (NSDictionary *) data {
    return data[@"MerchantId"];
}

- (NSString *) parseCountryCode: (NSDictionary *) data {
    //todo: add validation
    return data[@"CountryCode"];
}

- (NSString *) parseCurrencyCode: (NSDictionary *) data {
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

- (NSString *) stringFromPaymentMethodType:(PKPaymentMethodType)type {
    switch (type) {
        case PKPaymentMethodTypeUnknown:
            return @"PKPaymentMethodTypeUnknown";
        case PKPaymentMethodTypeDebit:
            return @"PKPaymentMethodTypeDebit";
        case PKPaymentMethodTypeCredit:
            return @"PKPaymentMethodTypeCredit";
        case PKPaymentMethodTypePrepaid:
            return @"PKPaymentMethodTypePrepaid";
        case PKPaymentMethodTypeStore:
            return @"PKPaymentMethodTypeStore";
        default:
            return @"PKPaymentMethodTypeUnknown";
    }
}

@end

extern bool _canMakePayments() {
    return [PKPaymentAuthorizationViewController canMakePayments];
}

extern void _askPaymentSheet(messageDelegate notifier, const char* config) {
    [[ApplePayController sharedInstance] askPaymentSheet:notifier config:stringFromChar(config)];
}

extern void _putConfirmation(messageDelegate notifier, const bool result) {
    [[ApplePayController sharedInstance] putConfirmation:notifier result:result];
}
