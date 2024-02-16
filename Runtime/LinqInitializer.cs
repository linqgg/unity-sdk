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

      if (string.IsNullOrEmpty(settings.SecretKey)) {
        Debug.LogError("LinQ: Secret Key is missing. Open menu \"LinQ/Edit Settings\" and provide correct Secret Key.");
        return;
      }

      Debug.Log("LinQ: Initializing LinQ SDK" + settings.SecretKey);

      // Smartlook.SetupAndStartRecording(new SetupOptions(settings.ProjectKey, settings.FPS, ResetSession, ResetUser));
    }
  }
}