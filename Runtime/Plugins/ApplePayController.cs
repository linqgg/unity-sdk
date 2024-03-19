using System;
using System.Threading;
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
			if (!status) {
				PaymentSession.onPaymentFailure(message);
				return;
			}

			if (string.IsNullOrEmpty(message))
			{
				PaymentSession.onPaymentDiscard("Payment cancelled by user by closing payment sheet");
				return;
			}

			PaymentSession.onPaymentSuccess(message);
		}

#if UNITY_IOS

		[DllImport("__Internal")]
		private static extern bool _canMakePayments();

		[DllImport("__Internal")]
		private static extern void _askPaymentSheet(messageDelegate message, string context, string config);

#endif

		public static bool CanMakePayments()
		{
			#if UNITY_IOS && !UNITY_EDITOR
				return _canMakePayments();
			#else
				return false;
			#endif
		}

    public static void AskPaymentSheet(string context, string config)
    {
			#if UNITY_IOS && !UNITY_EDITOR
				_askPaymentSheet(handleMessage, context, config);
      #else
				PaymentSession.onPaymentUnknown("ApplePay is not supported on this device");
      #endif
    }
  }

	public static class PaymentSession
  {
    private static TaskCompletionSource<string> _completion;

    public static Task<string> AutorizePayment(string context, string config)
    {
	    _completion = new TaskCompletionSource<string>();

	    ApplePayController.AskPaymentSheet(context, config);

	    return _completion.Task;
    }

    public static void onPaymentSuccess(string message)
    {
	    _completion.SetResult(message);
	    Debug.Log("[ApplePayController] " + message);
    }

    public static void onPaymentDiscard(string message)
    {
	    _completion.SetResult("discard");
	    Debug.Log("[ApplePayController] " + message);
    }

    public static void onPaymentUnknown(string message)
    {
	    _completion.SetResult("unknown");
	    Debug.LogWarning("[ApplePayController] " + message);
    }

    public static void onPaymentFailure(string message)
    {
	    _completion.SetResult("failure");
	    Debug.LogError("[ApplePayController] " + message);
    }
  }
}
