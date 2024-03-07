using System;
using UnityEngine;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using static Linq.Money.Payments.V1.NativePaymentsService;
using Linq.Money.Payments.V1;
using Grpc.Core;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using JetBrains.Annotations;
using Linq.Shared;

namespace LinqUnity
{
  [Serializable]
  public record TokenexDataRequest
  {
    public string tokenexid;
    public string timestamp;
    public string authenticationKey;
    public string tokenScheme;
    public string data;
    public string cvv;
  }

  [Serializable]
  public record TokenexDataResponse
  {
    public bool Success;
    [CanBeNull] public string Error;
    public string ReferenceNumber;
    public string Token;
    public string TokenHMAC;
  }

  [Serializable]
  public record PaymentDetails
  {
    public string CardNumber;
    public string HolderName;
    public string Expiration;
    public string Protection;
  }

  public class LinqSDK : MonoBehaviour
  {
    private static GrpcChannel _channel;
    private static Metadata _headers;

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
      #if UNITY_EDITOR
        Debug.Log("LinQ Initializing: " + remoteUrl + ", with key: " + secretKey);
      #endif

      _channel = GrpcChannel.ForAddress(remoteUrl, new GrpcChannelOptions() {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        }
      );

      _headers = new Metadata {
        { "Authorization", $"Bearer {secretKey}" }
      };
    }

    public static async Task<OrderResponse> StartPaymentProcessing(string orderId)
    {
      // 1. Getting config for a payment intention
      ApplePayConfig config = await GetApplePayConfig(orderId);
      Debug.Log("Fetched payment config: " + JsonConvert.SerializeObject(config));

      // 2. Request Apple Pay Session
      NativePayments.RequestPaymentView(JsonConvert.SerializeObject(config));

      // covert apple pay data - address to internal type

      // 3. Send full payload for processing payment
      // PaymentResponse payment = await

      // 4. Build PKPaymentRequest using data from apple_pay_config. Proceed with Apple Pay and get PKPayment in response.
      // split card and apple pay
      // if apple pay data provided - send using it, else - use card details
      // need to split to two methods?

      // PaymentResponse payment = await SetPaymentHandle(orderId, address, tokenizedCard);
      // Debug.Log("Payment result: " + JsonConvert.SerializeObject(payment));

      return null;
    }

    public static async Task<OrderResponse> StartPaymentProcessing(string orderId, PaymentDetails details, BillingAddress address)
    {
      // 1. Getting config for a payment intention
      CardPaymentConfig config = await GetPaymentConfig(orderId);
      Debug.Log("Fetched payment config: " + JsonConvert.SerializeObject(config));

      // 2. Asking token for a card
      CardTokenexPayment tokenizedCard = await GetTokenizedCard(config.TokenexConfig, details);
      Debug.Log("Tokenized card: " + JsonConvert.SerializeObject(tokenizedCard));

      // 3. Request Kount Session ID
      KountData kountSessionData = await GetSpecialChecks(config.KountConfig, details);
      tokenizedCard.KountData = kountSessionData;
      Debug.Log("Kount session: " + JsonConvert.SerializeObject(kountSessionData));

      // 4. Send full payload for processing payment
      PaymentResponse payment = await SetPaymentHandle(orderId, address, tokenizedCard);
      Debug.Log("Payment result: " + JsonConvert.SerializeObject(payment));

      return payment.Order;
    }

    private static async Task<ApplePayConfig> GetApplePayConfig(string orderId)
    {
      #if UNITY_EDITOR
        Debug.Log("Getting Apple Pay config for order id: " + orderId);
      #endif

      NativePaymentsServiceClient client = new(_channel);
      OrderConfigRequest request = new() { OrderId = orderId };

      return await client.GetApplePayConfigAsync(request, _headers);
    }

    private static async Task<CardPaymentConfig> GetPaymentConfig(string orderId)
    {
      #if UNITY_EDITOR
        Debug.Log("Getting Card Payment config for order id: " + orderId);
      #endif

      NativePaymentsServiceClient client = new(_channel);
      OrderConfigRequest request = new() { OrderId = orderId };

      return await client.GetCardPaymentConfigAsync(request, _headers);
    }

    private static async Task<CardTokenexPayment> GetTokenizedCard(TokenexConfig config, PaymentDetails details)
    {
      var tokenexRequest = new TokenexDataRequest()
      {
        tokenexid = config.TokenexId,
        timestamp = config.Timestamp,
        authenticationKey = config.AuthenticationKey,
        tokenScheme = config.TokenScheme,
        data = details.CardNumber,
        cvv = details.Protection,
      };

      TokenexDataResponse response = await ProcessWebRequest<TokenexDataResponse>(config.Url, JsonConvert.SerializeObject(tokenexRequest));

      string[] expiration = details.Expiration.Split("/");

      return new CardTokenexPayment()
      {
        CardHolderName = details.HolderName,
        ExpMonth = expiration[0],
        ExpYear = expiration[1],
        Token = response.Token,
        TokenHmac = response.TokenHMAC,
      };
    }

    private static async Task<KountData> GetSpecialChecks(KountConfig config, PaymentDetails details)
    {
      var data = new KountData()
      {
        FirstSix = details.CardNumber.Substring(0, 6),
        LastFour = details.CardNumber[^4..],
      };

      Task<string> sessionId = DataSession.RequestSessionId(config.ClientId, config.IsProd);

      data.SessionId = await sessionId;

      return data;
    }

    private static async Task<PaymentResponse> SetPaymentHandle(
      string orderId,
      BillingAddress address,
      ApplePayPayment payment
      )
    {
      #if UNITY_EDITOR
        Debug.Log("Setting payment handle for intent id: " + orderId);
      #endif

      NativePaymentsServiceClient client = new(_channel);
      PaymentRequest request = new()
      {
        OrderId = orderId,
        Address = address,
        ApplePayPayment = payment,
      };

      Debug.Log("Payment handle: " + JsonConvert.SerializeObject(request));

      return await client.MakePaymentAsync(request, _headers);

    }

    private static async Task<PaymentResponse> SetPaymentHandle(
      string orderId,
      BillingAddress address,
      CardTokenexPayment payment
      )
    {
      #if UNITY_EDITOR
        Debug.Log("Setting payment handle for intent id: " + orderId);
      #endif

      NativePaymentsServiceClient client = new(_channel);
      PaymentRequest request = new()
      {
        OrderId = orderId,
        Address = address,
        CardTokenexPayment = payment,
      };

      Debug.Log("Payment handle: " + JsonConvert.SerializeObject(request));

      return await client.MakePaymentAsync(request, _headers);
    }

    private static async UniTask<T> ProcessWebRequest<T>(string url, string payload)
    {
      var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

      request.uploadHandler = (UploadHandler) new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
      request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
      request.SetRequestHeader("Content-Type", "application/json");

      await request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.ProtocolError) {
        throw new Exception("HTTP ERROR " + request.error); // todo: improve error handling
      }

      var response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);

      request.Dispose();

      return response;
    }
  }
}
