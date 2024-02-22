using UnityEngine;
#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif

namespace LinqUnity
{
  public static class DataCollector
  {
    private delegate void sessionCallBackDelegate(string session);

    [AOT.MonoPInvokeCallbackAttribute(typeof(sessionCallBackDelegate))]
    public static void handleNativeCallBack(string message)
    {
      Debug.Log("Session is cacthed: " + message);
    }

    [DllImport("__Internal")]
    private static extern void _Init(sessionCallBackDelegate sessionCallBack, string kountClientId, bool isProd);
    public static void Init(string kountClientId, bool isProd) => _Init(handleNativeCallBack, kountClientId, isProd);

    [DllImport("__Internal")]
    private static extern void _Collect();
    public static void Collect() => _Collect();
  }
}
