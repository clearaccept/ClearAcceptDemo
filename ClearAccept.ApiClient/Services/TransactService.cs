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

        public PaymentRequestResponse GetPaymentRequest(string paymentRequestId)
        {
            return Get<PaymentRequestResponse>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}");
        }

        public PaymentRequestResponse PostPaymentRequest(PaymentRequest paymentRequest)
        {
            return Post<PaymentRequest, PaymentRequestResponse>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}", paymentRequest);
        }

        public PaymentRequestResponse PutPaymentRequest(string paymentRequestId, PaymentRequest paymentRequest)
        {
            return Put<PaymentRequest, PaymentRequestResponse>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}", paymentRequest);
        }

        public PaymentRequestResponse ConfirmPaymentRequest(string paymentRequestId, PaymentRequest paymentRequest)
        {
            return Post<PaymentRequest, PaymentRequestResponse>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}/confirm", paymentRequest);
        }

        public RefundResponse RefundTransaction(string paymentRequestId, string referenceId, RefundRequest refund)
        {
            return Post<RefundRequest, RefundResponse>($"{_configuration.Host}{ApiEndpoints.PaymentRequestsEndpoint}/{paymentRequestId}/transactions/{referenceId}/refund", refund);
        }
        
        public FieldTokenResponse PostFieldToken(FieldTokenRequest token)
        {
            return Post<FieldTokenRequest, FieldTokenResponse>($"{_configuration.Host}{ApiEndpoints.FieldTokensEndpoint}", token);
        }
    }
}