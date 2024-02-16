using UnityEngine;

namespace LinqUnity
{
  public class Settings : ScriptableObject
  {
    public const string SettingsResourceName = "LinqSettings";

    [SerializeField]
    [Tooltip("Public Secret Key (PSK) for access from Mobile SDK.")]
    public string SecretKey;

    private static Settings _instance;

    public static Settings Instance => _instance ??= LoadSettings();

    public static Settings LoadSettings()
    {
      return Resources.Load(SettingsResourceName) as Settings;
    }
  }
}
