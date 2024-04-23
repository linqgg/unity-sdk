#import <Foundation/Foundation.h>
#import "DataCollector.h"

static NSString* stringFromChar(const char *string) {
    return string ? [NSString stringWithUTF8String:string] : nil;
}

typedef void (*sessionCallBackDelegate)(const char* message);
static sessionCallBackDelegate callback = NULL;

void _Init(sessionCallBackDelegate delegate, const char* kountClientId, const bool isProd) {

    callback = delegate;

    if (isProd) {
      [[KDataCollector sharedCollector] setDebug:NO];
      [[KDataCollector sharedCollector] setEnvironment:KEnvironmentProduction];
    } else {
      [[KDataCollector sharedCollector] setDebug:YES];
      [[KDataCollector sharedCollector] setEnvironment:KEnvironmentTest];
    }

    // Set your Merchant ID
    [[KDataCollector sharedCollector] setMerchantID:stringFromChar(kountClientId)];

    // Set the location collection configuration
    [[KDataCollector sharedCollector]setLocationCollectorConfig:KLocationCollectorConfigRequestPermission];
    [[KountAnalyticsViewController sharedInstance] setEnvironmentForAnalytics: [KDataCollector.sharedCollector environment]];

    NSLog(@"Kount DataCollector is initialized");
}

void _Collect() {
    dispatch_async(dispatch_get_main_queue(), ^{

        NSString *sessionID = nil;

        [[KountAnalyticsViewController sharedInstance] collect:sessionID analyticsSwitch:true completion:^(NSString * _Nonnull sessionID, bool success, NSError * _Nullable error) {
            if (success) {
                callback([sessionID UTF8String]);
            } else {
                if (error != nil) {
                    NSLog([@(error.code) stringValue], error.description, error);
                } else {
                    NSLog(@0, @"Unknown kount error", nil);
                }
            }
        }];
    });
}
