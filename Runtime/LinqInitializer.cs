using UnityEngine;

namespace LinqUnity
{
  [DefaultExecutionOrder(-5000)]
  public class LinqInitializer : MonoBehaviour
  {
    private void Awake() {
      
      var settings = Settings.Instance;

      if (settings == null) {
        Debug.LogError("LinQ: Settings file is missing. Open menu \"LinQ/Edit Settings\" and provide desired settings.");
        return;  
      }

      if (string.IsNullOrEmpty(settings.remoteUrl)) {
        Debug.LogError("LinQ: Remote Url is missing. Open menu \"LinQ/Edit Settings\" and provide correct Remote Url.");
        return;
      }

      if (string.IsNullOrEmpty(settings.secretKey)) {
        Debug.LogError("LinQ: Secret Key is missing. Open menu \"LinQ/Edit Settings\" and provide correct Secret Key.");
        return;
      }

      LinqSDK.InitSDK(settings.remoteUrl, settings.secretKey);
    }
  }
}