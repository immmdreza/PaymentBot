using PaymentBot.Services.ZarinpalService.Models;
using System.Threading.Tasks;

namespace PaymentBot.Services.ZarinpalService
{
    public interface IZarinpalProvider
    {
        Task<ZarinpalResult<ZarinpalPaymentResponseModel>> PayAsync(ZarinpalPaymentRequestModel model);
        Task<ZarinpalResult<ZarinpalVerificationResponseModel>> VerifyAsync(ZarinpalPaymentVerificationModel model);

    }
}
