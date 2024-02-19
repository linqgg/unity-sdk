// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: linq/money/payments/v1/native.proto
// </auto-generated>
// Original file comments:
// Copyright 2023 Galactica Games Inc.
//
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Linq.Money.Payments.V1 {
  /// <summary>
  /// Payments service allows to perform mobile native payments
  /// </summary>
  public static partial class NativePaymentsService
  {
    static readonly string __ServiceName = "linq.money.payments.v1.NativePaymentsService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Linq.Money.Payments.V1.OrderConfigRequest> __Marshaller_linq_money_payments_v1_OrderConfigRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Linq.Money.Payments.V1.OrderConfigRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Linq.Money.Payments.V1.ApplePayConfig> __Marshaller_linq_money_payments_v1_ApplePayConfig = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Linq.Money.Payments.V1.ApplePayConfig.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Linq.Money.Payments.V1.CardPaymentConfig> __Marshaller_linq_money_payments_v1_CardPaymentConfig = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Linq.Money.Payments.V1.CardPaymentConfig.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Linq.Money.Payments.V1.PaymentRequest> __Marshaller_linq_money_payments_v1_PaymentRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Linq.Money.Payments.V1.PaymentRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Linq.Money.Payments.V1.PaymentResponse> __Marshaller_linq_money_payments_v1_PaymentResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Linq.Money.Payments.V1.PaymentResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.ApplePayConfig> __Method_GetApplePayConfig = new grpc::Method<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.ApplePayConfig>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetApplePayConfig",
        __Marshaller_linq_money_payments_v1_OrderConfigRequest,
        __Marshaller_linq_money_payments_v1_ApplePayConfig);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.CardPaymentConfig> __Method_GetCardPaymentConfig = new grpc::Method<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.CardPaymentConfig>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetCardPaymentConfig",
        __Marshaller_linq_money_payments_v1_OrderConfigRequest,
        __Marshaller_linq_money_payments_v1_CardPaymentConfig);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Linq.Money.Payments.V1.PaymentRequest, global::Linq.Money.Payments.V1.PaymentResponse> __Method_MakePayment = new grpc::Method<global::Linq.Money.Payments.V1.PaymentRequest, global::Linq.Money.Payments.V1.PaymentResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "MakePayment",
        __Marshaller_linq_money_payments_v1_PaymentRequest,
        __Marshaller_linq_money_payments_v1_PaymentResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Linq.Money.Payments.V1.NativeReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of NativePaymentsService</summary>
    [grpc::BindServiceMethod(typeof(NativePaymentsService), "BindService")]
    public abstract partial class NativePaymentsServiceBase
    {
      /// <summary>
      /// Apple pay config for order
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Linq.Money.Payments.V1.ApplePayConfig> GetApplePayConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      /// <summary>
      /// Card payment config
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Linq.Money.Payments.V1.CardPaymentConfig> GetCardPaymentConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      /// <summary>
      /// Native payment
      /// </summary>
      /// <param name="request">The request received from the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>The response to send back to the client (wrapped by a task).</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Linq.Money.Payments.V1.PaymentResponse> MakePayment(global::Linq.Money.Payments.V1.PaymentRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for NativePaymentsService</summary>
    public partial class NativePaymentsServiceClient : grpc::ClientBase<NativePaymentsServiceClient>
    {
      /// <summary>Creates a new client for NativePaymentsService</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public NativePaymentsServiceClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for NativePaymentsService that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public NativePaymentsServiceClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected NativePaymentsServiceClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected NativePaymentsServiceClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Apple pay config for order
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.ApplePayConfig GetApplePayConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetApplePayConfig(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Apple pay config for order
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.ApplePayConfig GetApplePayConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetApplePayConfig, null, options, request);
      }
      /// <summary>
      /// Apple pay config for order
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.ApplePayConfig> GetApplePayConfigAsync(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetApplePayConfigAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Apple pay config for order
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.ApplePayConfig> GetApplePayConfigAsync(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetApplePayConfig, null, options, request);
      }
      /// <summary>
      /// Card payment config
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.CardPaymentConfig GetCardPaymentConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetCardPaymentConfig(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Card payment config
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.CardPaymentConfig GetCardPaymentConfig(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_GetCardPaymentConfig, null, options, request);
      }
      /// <summary>
      /// Card payment config
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.CardPaymentConfig> GetCardPaymentConfigAsync(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return GetCardPaymentConfigAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Card payment config
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.CardPaymentConfig> GetCardPaymentConfigAsync(global::Linq.Money.Payments.V1.OrderConfigRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_GetCardPaymentConfig, null, options, request);
      }
      /// <summary>
      /// Native payment
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.PaymentResponse MakePayment(global::Linq.Money.Payments.V1.PaymentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return MakePayment(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Native payment
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The response received from the server.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Linq.Money.Payments.V1.PaymentResponse MakePayment(global::Linq.Money.Payments.V1.PaymentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_MakePayment, null, options, request);
      }
      /// <summary>
      /// Native payment
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.PaymentResponse> MakePaymentAsync(global::Linq.Money.Payments.V1.PaymentRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return MakePaymentAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Native payment
      /// </summary>
      /// <param name="request">The request to send to the server.</param>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Linq.Money.Payments.V1.PaymentResponse> MakePaymentAsync(global::Linq.Money.Payments.V1.PaymentRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_MakePayment, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override NativePaymentsServiceClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new NativePaymentsServiceClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(NativePaymentsServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_GetApplePayConfig, serviceImpl.GetApplePayConfig)
          .AddMethod(__Method_GetCardPaymentConfig, serviceImpl.GetCardPaymentConfig)
          .AddMethod(__Method_MakePayment, serviceImpl.MakePayment).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, NativePaymentsServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_GetApplePayConfig, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.ApplePayConfig>(serviceImpl.GetApplePayConfig));
      serviceBinder.AddMethod(__Method_GetCardPaymentConfig, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Linq.Money.Payments.V1.OrderConfigRequest, global::Linq.Money.Payments.V1.CardPaymentConfig>(serviceImpl.GetCardPaymentConfig));
      serviceBinder.AddMethod(__Method_MakePayment, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Linq.Money.Payments.V1.PaymentRequest, global::Linq.Money.Payments.V1.PaymentResponse>(serviceImpl.MakePayment));
    }

  }
}
#endregion
