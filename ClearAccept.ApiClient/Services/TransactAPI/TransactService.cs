using System;
using ClearAccept.ApiClient.BaseClasses;
using ClearAccept.ApiClient.Helpers;
using ClearAccept.ApiClient.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ClearAccept.ApiClient.Services
{
    public class TransactService : BaseService
    {
        public TransactService(ServiceConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public PaymentRequestModel GetPaymentRequest(string paymentRequestId)
        {
            return Get<PaymentRequestModel>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}");
        }

        public PaymentRequestModel PostPaymentRequest(PaymentRequestModel paymentRequest)
        {
            return Post<PaymentRequestModel, PaymentRequestModel>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}", paymentRequest);
        }

        public PaymentRequestModel PutPaymentRequest(PaymentRequestModel paymentRequest)
        {
            return Put<PaymentRequestModel, PaymentRequestModel>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequest.PaymentRequestId}", paymentRequest);
        }

        public PaymentRequestModel ConfirmPaymentRequest(PaymentRequestModel paymentRequest)
        {
            return Post<PaymentRequestModel, PaymentRequestModel>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequest.PaymentRequestId}/confirm", paymentRequest);
        }

        public PaymentRequestModel RefundTransaction(RefundModel refund, string paymentRequestId)
        {
            if (string.IsNullOrEmpty(paymentRequestId))
            {
                throw new Exception("PaymentRequestId cannot be null or empty.");
            }
            return Post<RefundModel, PaymentRequestModel>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}/transactions/{refund.ReferenceId}/refund", refund);
        }

        public FieldTokenModel GetFieldToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("FieldToken cannot be null or empty.");
            }
            return Get<FieldTokenModel>($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}/{token}");
        }

        public FieldTokenModel PostFieldToken(FieldTokenModel token)
        {
            return Post<FieldTokenModel, FieldTokenModel>($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}", token);
        }
    }
}