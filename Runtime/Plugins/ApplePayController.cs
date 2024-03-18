using System;
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
				PaymentSession.onPaymentFailed(message);
				return;
			}

			PaymentSession.onPaymentAuthorized(message);
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
				PaymentSession.onPaymentCancelled("Native payments are not supported on this device");
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

    public static void onPaymentAuthorized(string payment)
    {
	    if (string.IsNullOrEmpty(payment)) onPaymentCancelled("Payment cancelled by user");

      _completion.SetResult(payment);
    }

    public static void onPaymentCancelled(string message)
    {
	    throw new OperationCanceledException(message);
    }

    public static void onPaymentFailed(string message)
    {
	    Debug.LogError(message); // will be catched by sentry if set up
	    throw new OperationCanceledException(message);
    }
  }
}
