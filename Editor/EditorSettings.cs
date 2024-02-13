using UnityEditor;
using UnityEngine;

namespace LinqUnity.Editor {

  public static class EditorSettings {

    private const string SETTINGS_RESOURCE_SUFFIX = ".asset";
    private const string SETTINGS_RESOURCE_FOLDER = "Packages/gg.linq.unity-sdk/Resources/";

    [MenuItem("LinQ/Edit Settings")]
    public static void EditSettings()
    {
      var settings = Settings.LoadSettings();

      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<Settings>();
        AssetDatabase.CreateAsset(settings, SETTINGS_RESOURCE_FOLDER + Settings.SettingsResourceName + SETTINGS_RESOURCE_SUFFIX);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
      }

      Selection.activeObject = settings;
    }
  }
}
