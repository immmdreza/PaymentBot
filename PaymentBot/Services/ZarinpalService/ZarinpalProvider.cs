using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentBot.Services.ZarinpalService.Models;

namespace PaymentBot.Services.ZarinpalService
{
    public class ZarinpalProvider : IZarinpalProvider
    {
        private readonly ZarinpalConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _requestUrl;
        private readonly string _verifyUrl;

        public ZarinpalProvider(IOptionsSnapshot<ZarinpalConfiguration> options, HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (_httpClient == null)
                throw new ArgumentNullException(nameof(_httpClient));

            var zarinpalConfiguration = options;
            if (zarinpalConfiguration == null)
                throw new ArgumentNullException(nameof(zarinpalConfiguration));

            _configuration = zarinpalConfiguration.Value;
            if (_configuration == null)
                throw new ArgumentNullException(nameof(_configuration));

            _requestUrl = ZarinpalUrlConfig.GetPaymentRequestUrl(_configuration.UseSandbox);
            _verifyUrl = ZarinpalUrlConfig.GetVerificationUrl(_configuration.UseSandbox);
        }

        private async Task<ZarinpalResult<T>> PostRequestBase<T, TU>(TU model, string url) where T : class
        {
            try
            {
                var t = await _httpClient.PostAsync(url,
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
                var f = await t.Content.ReadAsAsync<T>();
                return ZarinpalResult<T>.Invoke(f);
            }
            catch
            {
                return ZarinpalResult<T>.Failed(new ZarinpalError
                {
                    Code = "8000",
                    Description = "Could not send request!"
                });
            }
        }

        public async Task<ZarinpalResult<ZarinpalVerificationResponseModel>> VerifyAsync(ZarinpalPaymentVerificationModel model)
        {
            var errors = new List<ZarinpalError>();

            model = new ZarinpalPaymentVerificationModel(_configuration.Token, model.Amount, model.Authority);
            model.ValidateModel(errors);

            if (errors.Any())
                return ZarinpalResult<ZarinpalVerificationResponseModel>.Failed(errors.ToArray());

            var t = await PostRequestBase<ZarinpalVerificationResponseModel, ZarinpalPaymentVerificationModel>(
                model,
                _verifyUrl);

            return !t.Succeeded ?
                ZarinpalResult<ZarinpalVerificationResponseModel>.Failed(errors.ToArray()) :
                ZarinpalResult<ZarinpalVerificationResponseModel>.Invoke(t.Result);
        }

        public async Task<ZarinpalResult<ZarinpalPaymentResponseModel>> PayAsync(ZarinpalPaymentRequestModel model)
        {
            var errors = new List<ZarinpalError>();
            model.ValidateModel(errors);
            if (errors.Any())
                return ZarinpalResult<ZarinpalPaymentResponseModel>.Failed(errors.ToArray());

            model.MerchantId = _configuration.Token;
            var t = await PostRequestBase<ZarinpalPaymentResponseModel, ZarinpalPaymentRequestModel>(
                model,
                _requestUrl);

            if (!t.Succeeded)
            {
                return ZarinpalResult<ZarinpalPaymentResponseModel>.Failed(errors.ToArray());
            }

            t.Result.PaymentUrl = _configuration.UseZarinLink ?
                ZarinpalUrlConfig.GetWebGateRequestUrl(t.Result.Authority, false) :
                ZarinpalUrlConfig.GetPaymenGatewayUrl(t.Result.Authority, false);
            t.Result.Validate(errors);

            return errors.Any()
                ? ZarinpalResult<ZarinpalPaymentResponseModel>.Failed(errors.ToArray())
                : ZarinpalResult<ZarinpalPaymentResponseModel>.Invoke(t.Result);
        }
    }
}
