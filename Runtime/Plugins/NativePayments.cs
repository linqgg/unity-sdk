using UnityEngine;
using System.Threading.Tasks;
#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif

namespace LinqUnity
{
	public static class NativePayments
	{
		private delegate void successCallbackDelegate(string data);
		private delegate void failureCallbackDelegate(string code, string message);

#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern bool _canMakePayments();

		[DllImport("__Internal")]
		private static extern void _showPaymentsView(
			string config,
			successCallbackDelegate success,
			failureCallbackDelegate failure);

#endif

		[AOT.MonoPInvokeCallbackAttribute(typeof(failureCallbackDelegate))]
		public static void handleFailureCallback(string code, string message)
		{
			Debug.Log("Code: " + code + ", Message: " + message);
		}

		[AOT.MonoPInvokeCallbackAttribute(typeof(successCallbackDelegate))]
		public static void handleSuccessCallback(string message)
		{
			Debug.Log("Returned payment msg: " + message);
		}

    public static void RequestPaymentView(string config)
    {
	    //todo: validate object types at least?
     	#if UNITY_IOS
				_showPaymentsView(config, handleSuccessCallback, handleFailureCallback);
      #else
        // DataSession.OnSessionIdCaptured("");
      #endif
    }

    public static bool CanMakePayments()
    {
	    #if UNITY_IOS
				return _canMakePayments();
			#else
				return false;
			#endif
    }
  }
}
