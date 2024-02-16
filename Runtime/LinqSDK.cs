using System;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Net.Client;
using System.Net.Http;
using Grpc.Net.Client.Web;
using static Linq.Money.Payments.V1.NativePaymentsService;
using Linq.Money.Payments.V1;
using Grpc.Core;

namespace LinqUnity
{
  public class LinqSDK : MonoBehaviour
  {
    public static readonly string LinqPluginVersion = "0.2.0";

    public static void SendReq(string orderId)
    {
      Debug.Log("SendReq");

      var channel = GrpcChannel.ForAddress(
        "https://services.stage.galactica.games", 
        // "https://services-stage-mx6hvkth4a-uc.a.run.app",
        new GrpcChannelOptions() {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        }
      );

      NativePaymentsServiceClient client = new (channel);

      OrderConfigRequest request = new() {
        OrderId = orderId
      };

      Debug.Log("order id is: " + orderId);

      var token = "fQrV8KoH65ef29SvxAYMvWeA37keijz7aanTWpqNFtAR9DBrv8d8x3VrpNbbKwDRpqDiuCKLztHKPFGfQyiWLf826LVuF8k3kwJX";

      var headers = new Metadata {
        { "Authorization", $"Bearer {token}" }
      };

      var result = client.GetCardPaymentConfig(request, headers);

      Debug.Log($"Result: {result}");
    }

  }
}