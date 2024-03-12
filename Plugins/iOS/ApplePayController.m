#import <Foundation/Foundation.h>
#import "ApplePayController.h"

typedef void (*messageDelegate)(const bool* status, const char* message);

static NSString* stringFromChar(const char *string) {
    return string ? [NSString stringWithUTF8String:string] : nil;
}

@interface ApplePayController : NSObject<PKPaymentAuthorizationControllerDelegate>

@property (nonatomic, strong) PKPaymentAuthorizationController * _Nullable viewController;
@property (nonatomic, copy) void (^__strong _Nonnull completion)(PKPaymentAuthorizationResult * _Nonnull __strong);
@property (nonatomic) messageDelegate _Nullable notify;

@end

@implementation ApplePayController

static const PKMerchantCapability PKMerchantCapabilityUnknown = 9999;
static const PKPaymentNetwork PKPaymentNetworkUnknown = 0;

+ (instancetype) sharedInstance
{
    static id sharedInstance = nil;

    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });

    return sharedInstance;
}

- (void) showPaymentsView: (messageDelegate) notifier
                   params: (NSString *) params
{
    self.notify = notifier;

    NSError *error;
    NSData *json = [params dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *data = [NSJSONSerialization JSONObjectWithData:json options:kNilOptions error:&error];

    if (error) {
        self.notify(false, [@"Invalid JSON payment methodData passed" UTF8String]);
        return;
    }

    NSString *merchantId = data[@"MerchantId"];
    NSString *orderLabel = data[@"LineItemLabel"];
    NSString *countryCode = data[@"CountryCode"];

    NSNumber *orderAmount = data[@"OrderAmount"];
    NSString *orderCurrency = data[@"OrderCurrency"];

    NSMutableArray *supportedNetworks = [NSMutableArray array];
    for (NSString *supportedNetwork in data[@"SupportedNetworks"]) {
        PKPaymentNetwork paymentNetwork = [self paymentNetworkFromString:supportedNetwork];
        if (paymentNetwork != PKPaymentNetworkUnknown) {
            [supportedNetworks addObject:paymentNetwork];
        } else {
            self.failure([@"invalid_supported_network" UTF8String], [[NSString stringWithFormat:@"Invalid supportedNetwork passed '%@'", supportedNetwork] UTF8String]);
            return;
        }
    }

    NSArray *merchantCapabilitiesData = data[@"MerchantCapabilities"];
    PKMerchantCapability merchantCapabilities = 0;
    if (merchantCapabilitiesData.count > 0) {
        for (NSString *capabilityString in merchantCapabilitiesData) {
            PKMerchantCapability capability = [self merchantCapabilityFromString:capabilityString];
            if (capability != PKMerchantCapabilityUnknown) {
                merchantCapabilities |= capability;
            } else {
                self.failure([@"invalid_merchant_capability" UTF8String], [[NSString stringWithFormat:@"Invalid merchant capability passed '%@'", capabilityString] UTF8String]);
                return;
            }
        }
    }

    NSDecimalNumber *ratio = [NSDecimalNumber decimalNumberWithDecimal:[@100 decimalValue]];
    NSDecimalNumber *coins = [NSDecimalNumber decimalNumberWithDecimal:[orderAmount decimalValue]];
    NSDecimalNumber *amount = [coins decimalNumberByDividingBy:ratio];

    PKPaymentSummaryItem *paymentSummaryItem = [PKPaymentSummaryItem summaryItemWithLabel:orderLabel amount:amount];

    NSMutableArray <PKPaymentSummaryItem *> *paymentSummaryItems = [NSMutableArray array];
    [paymentSummaryItems addObject: paymentSummaryItem];

    PKPaymentRequest *paymentRequest = [[PKPaymentRequest alloc] init];

    paymentRequest.merchantCapabilities = merchantCapabilities;
    paymentRequest.supportedNetworks = supportedNetworks;
    paymentRequest.merchantIdentifier = merchantId;
    paymentRequest.countryCode = countryCode;
    paymentRequest.currencyCode = orderCurrency;
    paymentRequest.paymentSummaryItems = paymentSummaryItems;

    if (data[@"RequiredBillingContactFields"]) {
        // todo: need apply mapping and configure from server?
        paymentRequest.requiredBillingContactFields = [NSSet setWithArray:@[PKContactFieldName, PKContactFieldEmailAddress, PKContactFieldPostalAddress, PKContactFieldPhoneNumber]];
    }

    self.viewController = [[PKPaymentAuthorizationController alloc] initWithPaymentRequest:paymentRequest];
    self.viewController.delegate = self;

    if (!self.viewController) {
        failure([@"no_view_controller" UTF8String], [@"Failed initializing PKPaymentAuthorizationViewController, check you app ApplePay capabilities and merchantIdentifier" UTF8String]);
        return;
    }

    [self.viewController presentWithCompletion:^(BOOL success) {
        if (success) {
            NSLog(@"Payment shit is presented to the player");
            self.success([@"Payment shit is presented to the player" UTF8String]);
        }
    }];
}

- (void) _putConfirmation: (messageDelegate) notifier
                   status: (BOOL *) status
{
    PKPaymentAuthorizationStatus status = PKPaymentAuthorizationStatusFailure;

    if ([incomingStatus isEqualToString: @"success"]) {
        status = PKPaymentAuthorizationStatusSuccess;
    }

    self.completion([[PKPaymentAuthorizationResult alloc] initWithStatus:status errors:nil]);

    self.success("ololo completed");
}

- (void) paymentAuthorizationController:(PKPaymentAuthorizationViewController *)controller
                    didAuthorizePayment:(PKPayment *)payment
                                handler:(void (^)(PKPaymentAuthorizationResult *result))completion
{
    self.completion = completion;

    NSLog(@"Seems comething triggered here!");

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
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:paymentDict options:NSJSONWritingPrettyPrinted error:&error];

    if (!jsonData) {
        self.failure([@"json_serialization_error" UTF8String], [@"Failed to serialize PKPayment to JSON." UTF8String]);
    } else {
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        self.success([jsonString UTF8String]);
    }

    PKPaymentAuthorizationResult *result = [[PKPaymentAuthorizationResult alloc] initWithStatus:PKPaymentAuthorizationStatusFailure errors:nil];

    completion(result);
}

- (void) paymentAuthorizationControllerDidFinish:(PKPaymentAuthorizationController *)controller
{
    [controller dismissWithCompletion:^{
        NSLog(@"Payment process canceled by user.");
        self.failure([@"payment_error" UTF8String], [@"Payment process canceled by user." UTF8String]);
    }];
}

- (PKPaymentNetwork)paymentNetworkFromString:(NSString *)paymentNetworkString {
    if ([paymentNetworkString isEqualToString:@"amex"]) {
        return PKPaymentNetworkAmex;
    } else if ([paymentNetworkString isEqualToString:@"discover"]) {
        return PKPaymentNetworkDiscover;
    } else if ([paymentNetworkString isEqualToString:@"masterCard"]) {
        return PKPaymentNetworkMasterCard;
    } else if ([paymentNetworkString isEqualToString:@"visa"]) {
        return PKPaymentNetworkVisa;
    }

    return PKPaymentNetworkUnknown;
}

- (PKMerchantCapability)merchantCapabilityFromString:(NSString *)capabilityString {
    NSDictionary *capabilityMap = @{
        @"supports3DS": @(PKMerchantCapability3DS),
        @"supportsEMV": @(PKMerchantCapabilityEMV),
        @"supportsCredit": @(PKMerchantCapabilityCredit),
        @"supportsDebit": @(PKMerchantCapabilityDebit)
    };

    NSNumber *mappedCapabilityNumber = capabilityMap[capabilityString];
    if (mappedCapabilityNumber != nil) {
        return (PKMerchantCapability)mappedCapabilityNumber.unsignedLongValue;
    } else {
        return PKMerchantCapabilityUnknown;
    }
}

- (NSString *)stringFromPaymentMethodType:(PKPaymentMethodType)type {
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

extern void _putConfirmation(messageDelegate notifier, const bool* status) {
    [[ApplePayController sharedInstance] putConfirmation:notifier status:status];
}
