using UnityEngine;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using static Linq.Money.Payments.V1.NativePaymentsService;
using Linq.Money.Payments.V1;
using Grpc.Core;
using System.Threading.Tasks;

namespace LinqUnity
{
  public class LinqSDK : MonoBehaviour
  {
    private static GrpcChannel channel;
    private static Metadata headers;

    /// <summary>
    /// Initialize the LinQ SDK with your remoteUrl and secretKey.
    /// </summary>
    /// <param name="remoteUrl">Remote services URL for access from Mobile SDK.</param>
    /// <param name="secretKey">Public Secret Key (PSK) for access from Mobile SDK.</param>
    /// <example>
    /// <code>
    /// AppsFlyer.initSDK("https://services.stage.galactica.games", "fQ***X");
    /// </code>
    /// </example>
    public static void InitSDK(string remoteUrl, string secretKey)
    {
      Debug.Log("LinQ Initializing: " + remoteUrl + ", with key: " + secretKey);

      channel = GrpcChannel.ForAddress(remoteUrl, new GrpcChannelOptions() {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        }
      );

      headers = new Metadata {
        { "Authorization", $"Bearer {secretKey}" }
      };

    }

    public static async void MakePaymentAsync(string intentionId)
    {
      Debug.Log("MakePaymentAsync");

      // var d = await GetPaymentConfig("5f9e3e3e-3b3e-4e3e-8e3e-3e3e3e3e3e3e");
      var d = await GetPaymentConfig(intentionId);

      // need ask config from backend
      // send data to tokenex and get token
      // check data with Kount DDC
      // send request for payment and get its status

      // order id - 5f9e3e3e-3b3e-4e3e-8e3e-3e3e3e3e3e3e - intent id
      // card data
      // billing address data - stored somewhere? - option to store in player prefs
    }

    private static async Task<CardPaymentConfig> GetPaymentConfig(string orderId)
    {
      #if UNITY_EDITOR
        Debug.Log("Getting config for intent id: " + orderId);
      #endif

      NativePaymentsServiceClient client = new (channel);

      OrderConfigRequest request = new() {
        OrderId = orderId
      };

      var result = await client.GetCardPaymentConfigAsync(request, headers);

      Debug.Log($"Result: {result}");

      return result;
    }

  }
}