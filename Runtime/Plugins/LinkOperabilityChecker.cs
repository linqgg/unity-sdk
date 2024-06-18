#if UNITY_IOS
    using System.Runtime.InteropServices;
#endif

namespace LinqUnity
{
  public static class LinkOperabilityChecker
  {
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern bool _IOSCanOpenURL(string url);
    public static bool CheckUrl (string url) => _IOSCanOpenURL (url);
#else
    public static bool CheckUrl(string url) => false;
#endif
  }
}
