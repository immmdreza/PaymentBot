using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PaymentBot.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Display(Name = "کد سفارش")]
        public string Authority { get; set; }
        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفاً قیمت را وارد نمائید.")]
        public int Amount { get; set; }
        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفاً توضیحات را وارد نمائید.")]
        [MinLength(2, ErrorMessage = "توضیحات باید حداقل 2 کاراکتر باشد.")]
        public string Description { get; set; }
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        [Display(Name = "موبایل")]
        public string Mobile { get; set; }

        [Display(Name = "هدف پرداخت")]
        public PaymentGoal PaymentGoal { get; set; }
        [Display(Name = "آیا کامل شده؟")]
        public bool IsCompleted { get; set; }
        [Display(Name = "آیا موفق بوده؟")]
        public bool IsSuccesseded { get; set; }

        [DefaultValue(0)]
        [NotNull]
        public int PayMessageId { get; set; }
    }

    public enum PaymentGoal
    {
        Donate,
        Ads
    }
}
