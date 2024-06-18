using System;
using UnityEngine;
using System.Threading.Tasks;
using Linq.Money.Payments.V1;
using static Linq.Money.Payments.V1.NativePaymentsService;

namespace LinqUnity
{
  public static class SecurityCheck
  {
    private static UniWebView _browser;

    private static TaskCompletionSource<bool> _completion;

    public static Task<bool> Validate3DSCode(string script)
    {
      _completion = new TaskCompletionSource<bool>();

      InitiateView(script);

      return _completion.Task;
    }

    private static void InitiateView(string script)
    {
      UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Debug;

      _browser = new GameObject("WebView").AddComponent<UniWebView>();

      _browser.Frame = new Rect(0, 0, Screen.width, Screen.height);
      _browser.BackgroundColor = Color.clear;

      _browser.SetAcceptThirdPartyCookies(true);
      _browser.SetVerticalScrollBarEnabled(false);
      _browser.SetHorizontalScrollBarEnabled(false);
      _browser.SetTransparencyClickingThroughEnabled(true);
      _browser.SetBouncesEnabled(false);
      _browser.SetShowSpinnerWhileLoading(true);
      _browser.SetZoomEnabled(false);
      _browser.LoadHTMLString(script, "");
      // _browser.Alpha = 0.0f;
      _browser.Show();

      // _browser.OnPageStarted -= (view, url) => { };

      _browser.OnPageStarted += (view, _) => view.ShowSpinner();

      _browser.OnPageFinished += (view, statusCode, url) => {
        view.EvaluateJavaScript("window.uniwebview = true;", (payload) =>
        {
          if (!payload.resultCode.Equals("0"))
          {
            Debug.Log("Something goes wrong: " + payload.data);
            return;
          }
          Debug.Log("UniWebView registered!");
        });
      };

      _browser.OnMessageReceived += (view, message) =>
      {
        if (!message.Path.Equals("completion")) return;

        string answer = message.Args["message"];

        if (string.IsNullOrEmpty(answer)) return;

        switch (answer)
        {
          case "challengeRendered":
            // view.Alpha = 1.0f;
            // view.Show(true, UniWebViewTransitionEdge.Bottom);
            view.HideSpinner();
            break;
          case "challengeFinished":
            view.ShowSpinner();
            break;
          case "paymentSuccess":
            // view.HideSpinner();
            // view.Hide(true);
            // // view.Alpha = 0.0f;
            // _browser = null;
            OnSecurityCodeChecked(true);
            break;
          case "paymentFailed":
          case "challengeSkipped":
          case "error":
          // default:
            // view.HideSpinner();
            // view.Hide(true);
            // _browser = null;
            OnSecurityCodeChecked(false);
            break;
        }
      };
    }

    public static void OnSecurityCodeChecked(bool status)
    {
      _browser.HideSpinner();
      _browser.Hide(true);

      UnityEngine.Object.Destroy(_browser);
      _browser = null;

      _completion.SetResult(status);
    }
  }
}
