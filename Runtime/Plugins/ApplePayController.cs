using UnityEngine;
using System.Threading.Tasks;
#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif

namespace LinqUnity
{
	public static class ApplePayController
	{
		private delegate void messageDelegate(bool status, string message);

		[AOT.MonoPInvokeCallbackAttribute(typeof(messageDelegate))]
		public static void handleMessage(bool status, string message)
		{
			Debug.Log("Status: " + status + ", Message: " + message);
		}

#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern bool _canMakePayments();

		[DllImport("__Internal")]
		private static extern void _askPaymentSheet(messageDelegate message, string config);

		[DllImport("__Internal")]
		private static extern void _putConfirmation(messageDelegate message, bool status);

#endif

		public static bool CanMakePayments()
		{
			#if UNITY_IOS
				return _canMakePayments();
			#else
				return false;
			#endif
		}

    public static void AskPaymentSheet(string config)
    {
     	#if UNITY_IOS
				_askPaymentSheet(handleMessage, config);
      #else
        // do nothing
      #endif
    }

    public static void PutConfirmation(bool status)
    {
			#if UNITY_IOS
				_putConfirmation(handleMessage, status);
			#else
				// do nothing
			#endif
    }
  }
}
