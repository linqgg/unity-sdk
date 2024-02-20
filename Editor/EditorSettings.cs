using UnityEditor;
using UnityEngine;

namespace LinqUnity.Editor {

  public static class EditorSettings {

    private const string SettingsResourceSuffix = ".asset";
    private const string SettingsResourceFolder = "Assets/Resources/Settings/";

    [MenuItem("LinQ/Edit Settings")]
    public static void EditSettings()
    {
      var settings = Settings.LoadSettings();

      if (settings == null)
      {
        settings = ScriptableObject.CreateInstance<Settings>();
        AssetDatabase.CreateAsset(settings, SettingsResourceFolder + Settings.SettingsResourceName + SettingsResourceSuffix);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
      }

      Selection.activeObject = settings;
    }

    [MenuItem("GameObject/LinQ/Initializer", false, 10)]
    [MenuItem("LinQ/Create Initializer")]
    public static void CreateInitializer(MenuCommand menuCommand)
    {
      var go = new GameObject("LinQ Initializer");
      go.AddComponent<LinqInitializer>();
      GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
      Selection.activeObject = go;
    }

  }
}
