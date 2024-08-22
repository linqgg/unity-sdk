#if UNITY_IOS && !UNITY_EDITOR
  using System.Runtime.InteropServices;
#else
  using UnityEngine;
#endif

namespace LinqUnity
{
    public static class Keychain
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void setKeychainValue(string key, string value, string accessGroup);

        public static void setAuthUserToken(string token, string accessGroup)
        {
            setKeychainValue("GAME_LOGIN_TO_GALACTICA_WALLET_TOKEN", token, accessGroup);
        }
#else
        public static void setAuthUserToken(string token, string accessGroup)
        {
            Debug.LogError("Support only on IOS device");
        }
#endif
    }
}