#import <Foundation/Foundation.h>
#import "NativePayments.h"

typedef void (*successCallbackDelegate)(const char* data);
typedef void (*failureCallbackDelegate)(const char* code, const char* message);

static NSString* stringFromChar(const char *string) {
    return string ? [NSString stringWithUTF8String:string] : nil;
}

//@interface NativePayments : NSObject<PKPaymentAuthorizationViewControllerDelegate>
@interface NativePayments : NSObject

//- (void) showPaymentsView;

@property (nonatomic, strong) PKPaymentAuthorizationViewController * _Nullable viewController;
@property (nonatomic, copy) void (^__strong _Nonnull completion)(PKPaymentAuthorizationResult * _Nonnull __strong);
@property (nonatomic) successCallbackDelegate _Nullable success;
@property (nonatomic) failureCallbackDelegate _Nullable failure;

@end

@implementation NativePayments

static const PKMerchantCapability PKMerchantCapabilityUnknown = 9999;
static const PKPaymentNetwork PKPaymentNetworkUnknown = 0;

- (void) showPaymentsView: (NSString *) params
                   success: (successCallbackDelegate) success
                   failure: (failureCallbackDelegate) failure
{
    self.success = success;
    self.failure = failure;

    NSError *error;

    NSData *json = [params dataUsingEncoding:NSUTF8StringEncoding];

    NSDictionary *data = [NSJSONSerialization JSONObjectWithData:json options:kNilOptions error:&error];

    if (error) {
        NSLog(@"Invalid json");
        self.failure([@"wrong_payment_data" UTF8String], [@"Invalid JSON payment methodData passed" UTF8String]);
        return;
    }

    NSString *merchantId = data[@"MerchantId"];
    NSString *countryCode = data[@"CountryCode"];
    NSString *currencyCode = data[@"CurrencyCode"];

//    NSMutableArray *supportedNetworks =  [NSMutableArray array];
//    for (NSString *supportedNetwork in data[@"supportedNetworks"]) {
//        PKPaymentNetwork paymentNetwork = [self paymentNetworkFromString:supportedNetwork];
//        if (paymentNetwork != PKPaymentNetworkUnknown) {
//            [supportedNetworks addObject:paymentNetwork];
//        } else {
//            self.failure(@"invalid_supported_network", [NSString stringWithFormat:@"Invalid supportedNetwork passed '%@'", supportedNetwork]);
//            return;
//        }
//    }

//    NSArray *merchantCapabilitiesData = data[@"merchantCapabilities"];
//    PKMerchantCapability merchantCapabilities = 0;
//    if (merchantCapabilitiesArray.count > 0) {
//        for (NSString *capabilityString in merchantCapabilitiesData) {
//            PKMerchantCapability capability = [self merchantCapabilityFromString:capabilityString];
//            if (capability != PKMerchantCapabilityUnknown) {
//                merchantCapabilities |= capability;
//            } else {
//                self.failure(@"invalid_merchant_capability", [NSString stringWithFormat:@"Invalid merchant capability passed '%@'", capabilityString]);
//                return;
//            }
//        }
//    }

    PKPaymentRequest *paymentRequest = [[PKPaymentRequest alloc] init];

    //paymentRequest.merchantCapabilities = merchantCapabilities;
//    paymentRequest.supportedNetworks = supportedNetworks;
    paymentRequest.merchantIdentifier = merchantId;
    paymentRequest.countryCode = countryCode;
    paymentRequest.currencyCode = currencyCode;

    // paymentRequest.paymentSummaryItems = [self getPaymentSummaryItemsFromDetails:details];

    if (data[@"requiredBillingContactFields"]) {
        paymentRequest.requiredBillingContactFields = [NSSet setWithArray:@[PKContactFieldName, PKContactFieldEmailAddress, PKContactFieldPostalAddress, PKContactFieldPhoneNumber]];
    }

    self.viewController = [[PKPaymentAuthorizationViewController alloc] initWithPaymentRequest:paymentRequest];
    self.viewController.delegate = self;

    if (!self.viewController) {
        self.failure([@"no_view_controller" UTF8String], [@"Failed initializing PKPaymentAuthorizationViewController, check you app ApplePay capabilities and merchantIdentifier" UTF8String]);
        return;
    }

    UIViewController *rootViewController = UnityGetGLViewController();
    [rootViewController presentViewController:self.viewController animated:YES completion:nil];
}

@end

void _showPaymentsView(const char* config, successCallbackDelegate success, failureCallbackDelegate failure) {
    [[NativePayments alloc] showPaymentsView:stringFromChar(config) success:success failure:failure];
}
