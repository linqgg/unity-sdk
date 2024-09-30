#import <StoreKit/StoreKit.h>

char* convert(const NSString* string)
{
    if (string == NULL) {
        return NULL;
    }

    const char* encoded = [string UTF8String];
    char* result = (char*) malloc(strlen(encoded) + 1);
    strcpy(result, encoded);

    return result;
}

char* _GetAppStoreRegion () {

    NSString* country = @"";

    if (@available(iOS 13.0, *)) {
        NSString* code = [[[SKPaymentQueue defaultQueue] storefront] countryCode];
        if (code) {
            country = code;
        }
    }

    return convert(country);
}
