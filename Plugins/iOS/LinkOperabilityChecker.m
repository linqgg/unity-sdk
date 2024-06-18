NSString *ToNSString(char* string) {
    return [NSString stringWithUTF8String:string];
}

bool _IOSCanOpenURL (char* url) {
    return [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:ToNSString(url)]];
}
