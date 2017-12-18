using System.ComponentModel.DataAnnotations;

namespace ZarinPal.Models
{
    public class PaymentModel
    {
        [Display(Name = "کد سفارش")]
        public int OrderId { get; set; }
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
    }
}