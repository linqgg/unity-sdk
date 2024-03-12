using UnityEngine;
using System.Threading.Tasks;
#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif

//KountDataCollector

namespace LinqUnity
{
  public static class DataCollector
  {
    private delegate void sessionCallBackDelegate(string session);

	#if UNITY_IOS
      [DllImport("__Internal")]
      private static extern void _Init(sessionCallBackDelegate sessionCallBack, string kountClientId, bool isProd);
	#endif

	#if UNITY_IOS
      [DllImport("__Internal")]
      private static extern void _Collect();
	#endif

    [AOT.MonoPInvokeCallbackAttribute(typeof(sessionCallBackDelegate))]
    public static void handleNativeCallBack(string message) => DataSession.OnSessionIdCaptured(message);

    public static void RequestSessionId(string kountClientId, bool isProd)
    {
      #if UNITY_IOS
        _Init(handleNativeCallBack, kountClientId, isProd);
        _Collect();
      #else
        DataSession.OnSessionIdCaptured("");
      #endif
    }
  }

  public static class DataSession
  {
    private static TaskCompletionSource<string> _completion;

    public static Task<string> RequestSessionId(string kountClientId, bool isProd)
    {
      _completion = new TaskCompletionSource<string>();

      DataCollector.RequestSessionId(kountClientId, isProd);

      return _completion.Task;
    }

    public static void OnSessionIdCaptured(string sessionId)
    {
      _completion.SetResult(sessionId);
    }
  }
}
