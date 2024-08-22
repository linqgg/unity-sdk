
#import <Foundation/Foundation.h>
#import <Security/Security.h>
#import <UIKit/UIKit.h>

void setKeychainValue(const char* key, const char* value, const char* accessGroup) {
    // Convert C strings to NSString
    NSString *keyString = [NSString stringWithUTF8String:key];
    NSString *valueString = [NSString stringWithUTF8String:value];
    NSString *accessGroupString = [NSString stringWithUTF8String:accessGroup];

    // Get the IDFV (Identifier for Vendor)
    NSString *idfv = [[[UIDevice currentDevice] identifierForVendor] UUIDString];

    // Convert value to NSData
    NSData *valueData = [valueString dataUsingEncoding:NSUTF8StringEncoding];

    // Create the query dictionary
    NSDictionary *query = @{
        (__bridge NSString *)kSecClass: (__bridge id)kSecClassGenericPassword,
        (__bridge NSString *)kSecAttrService: keyString,
        (__bridge NSString *)kSecAttrAccount: idfv,
        (__bridge NSString *)kSecAttrAccessGroup: accessGroupString,
        (__bridge NSString *)kSecValueData: valueData
    };

    // Delete any existing item with the same key and account
    SecItemDelete((__bridge CFDictionaryRef)query);

    // Add the new item to the Keychain
    OSStatus status = SecItemAdd((__bridge CFDictionaryRef)query, NULL);
    if (status != errSecSuccess) {
        NSLog(@"Error setting Keychain value: %d", (int)status);
    } else {
        NSLog(@"Keychain value successfully set");
    }
}
