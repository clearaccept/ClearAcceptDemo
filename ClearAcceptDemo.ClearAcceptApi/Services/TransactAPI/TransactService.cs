using System;
using ClearAcceptDemo.ClearAcceptApi.BaseClasses;
using ClearAcceptDemo.ClearAcceptApi.Helpers;
using ClearAcceptDemo.ClearAcceptApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ClearAcceptDemo.ClearAcceptApi.Services
{
    public class TransactService : BaseService
    {
        public TransactService(ServiceConfiguration configuration, IMemoryCache memoryCache) : base(configuration, memoryCache)
        {
        }

        public PaymentRequest GetPaymentRequest(string paymentRequestId)
        {
            return Get<PaymentRequest>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}");
        }

        public PaymentRequest PostPaymentRequest(PaymentRequest paymentRequest)
        {
            return Post<PaymentRequest, PaymentRequest>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}", paymentRequest);
        }

        public PaymentRequest PutPaymentRequest(PaymentRequest paymentRequest)
        {
            return Put<PaymentRequest, PaymentRequest>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}", paymentRequest);
        }

        public PaymentRequest ConfirmPaymentRequest(PaymentRequest paymentRequest)
        {
            return Post<PaymentRequest, PaymentRequest>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequest.PaymentRequestId}/confirm", paymentRequest);
        }

        public PaymentRequest RefundTransaction(RefundRequest refundRequest, string paymentRequestId)
        {
            if (string.IsNullOrEmpty(paymentRequestId))
            {
                throw new Exception("PaymentRequestId cannot be null or empty.");
            }
            return Post<RefundRequest, PaymentRequest>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}/refund", refundRequest);
        }

        public FieldToken GetFieldToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("FieldToken cannot be null or empty.");
            }
            return Get<FieldToken>($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}/{token}");
        }

        public FieldToken PostFieldToken(FieldToken token)
        {
            return Post<FieldToken, FieldToken>($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}", token);
        }

        public System.Net.HttpStatusCode DeleteFieldToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("FieldToken cannot be null or empty.");
            }
            return Delete($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}/{token}");
        }
    }
}