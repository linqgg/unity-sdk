using System;
using UnityEngine;
using System.Threading.Tasks;
using Linq.Money.Payments.V1;
using static Linq.Money.Payments.V1.NativePaymentsService;

namespace LinqUnity
{
  public static class SecurityCheck
  {
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

      UniWebView browser = new GameObject("WebView").AddComponent<UniWebView>();

      browser.Frame = new Rect(0, 0, Screen.width, Screen.height);
      browser.BackgroundColor = Color.clear;

      browser.SetAcceptThirdPartyCookies(true);
      browser.SetVerticalScrollBarEnabled(false);
      browser.SetHorizontalScrollBarEnabled(false);
      browser.SetTransparencyClickingThroughEnabled(true);
      browser.SetBouncesEnabled(false);
      browser.SetShowSpinnerWhileLoading(true);
      browser.SetZoomEnabled(false);
      browser.LoadHTMLString(script, "");
      browser.Alpha = 0.0f;
      browser.Show();

      browser.OnPageStarted += (view, _) => view.ShowSpinner();

      browser.OnPageFinished += (view, statusCode, url) => {
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

      browser.OnMessageReceived += (view, message) =>
      {
        if (!message.Path.Equals("completion")) return;

        string answer = message.Args["message"];

        if (string.IsNullOrEmpty(answer)) return;

        switch (answer)
        {
          case "challengeRendered":
            view.Alpha = 1.0f;
            view.HideSpinner();
            break;
          case "challengeFinished":
            view.ShowSpinner();
            break;
          case "paymentSuccess":
            OnSecurityCodeChecked(true, view);
            break;
          case "paymentFailed":
          case "challengeSkipped":
          case "error":
            OnSecurityCodeChecked(false, view);
            break;
        }
      };
    }

    public static void OnSecurityCodeChecked(bool status, UniWebView view)
    {
      view.HideSpinner();
      view.Hide(true);

      UnityEngine.Object.Destroy(view);

      _completion.SetResult(status);
    }
  }
}
