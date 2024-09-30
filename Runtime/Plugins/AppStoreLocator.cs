#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif

namespace LinqUnity
{
  public static class AppStoreLocator
  {
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")] public static extern string _GetAppStoreRegion();
    public static string GetAppStoreRegion() => _GetAppStoreRegion();
#else
    public static string GetAppStoreRegion() => ""; // by ISO 3166-1 alpha-3
#endif
  }
}
