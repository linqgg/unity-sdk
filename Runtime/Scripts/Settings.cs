using UnityEngine;

namespace LinqUnity
{
  public class Settings : ScriptableObject
  {
    public const string SettingsResourceName = "LinqSettings";

    [SerializeField]
    [Tooltip("Private SDK access token from partner dashboard.")]
    public string Token;

    private static Settings _instance;

    public static Settings Instance => _instance ??= LoadSettings();

    public static Settings LoadSettings()
    {
      return Resources.Load(SettingsResourceName) as Settings;
    }
  }
}
