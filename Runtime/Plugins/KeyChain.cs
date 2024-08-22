#if UNITY_IOS
  using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace LinqUnity
{
  public static class Keychain
  {

#if UNITY_IOS

    [DllImport("__Internal")]
    public static extern void setKeychainValue(string key, string value, string accessGroup);

#endif

    public static void setAuthUserToken(string token, string accessGroup)
    {
#if UNITY_IOS && !UNITY_EDITOR
      setKeychainValue("GAME_LOGIN_TO_GALACTICA_WALLET_TOKEN", token, accessGroup);
#else
      Debug.LogError("Support only on IOS device");
#endif
    }
  }
}
