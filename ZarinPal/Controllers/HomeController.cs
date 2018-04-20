using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ZarinPal.Models;

namespace ZarinPal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PaymentModel model = new PaymentModel
            {
                OrderId = new Random().Next(1001, 9999),// Just generate random number for simulation.
                Amount = 1000,
                Description = "توضیحات تست درگاه پرداخت زرین پال",
                Email = "vira1368@gmail.com",
                Mobile = "09357924021"
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Payment(PaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                string message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
            ServicePointManager.Expect100Continue = false;
            Zarinpal.PaymentGatewayImplementationServicePortTypeClient client = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
            string authority;
            int amount = model.Amount;
            string description = model.Description;
            string email = model.Email;
            string mobile = model.Mobile;
            int orderId = model.OrderId;
            string callbackUrl = "http://" + Request.Url.Authority + "/Home/Verify/" + orderId;

            int status = client.PaymentRequest("MerchantID", amount, description, email, mobile, callbackUrl, out authority);

            if (status == 100)
            {
                ////For release mode
                //Response.Redirect("https://zarinpal.com/pg/StartPay/" + authority);

                ////For test mode
                Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + authority);
                return null;
            }
            TempData["Message"] = GetMessage(status);
            return View("Index");
        }

        public ActionResult Verify(int id)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Status"]) && !string.IsNullOrEmpty(Request.QueryString["Authority"]))
            {
                if (Request.QueryString["Status"].Equals("OK"))
                {
                    int amount = 1000;
                    long refId;
                    ServicePointManager.Expect100Continue = false;
                    Zarinpal.PaymentGatewayImplementationServicePortTypeClient client = new Zarinpal.PaymentGatewayImplementationServicePortTypeClient();
                    int status = client.PaymentVerification("MerchantID", Request.QueryString["Authority"], amount, out refId);
                    if (status == 100 || status == 101)
                    {
                        ViewBag.RefId = "کد پیگیری: " + refId + " - کد سفارش: " + id;
                    }
                    else
                    {
                        ViewBag.Message = GetMessage(status);
                    }
                }
                else
                {
                    ViewBag.Message = "کد مرجع: " + Request.QueryString["Authority"] + " - وضعیت:" + Request.QueryString["Status"];
                }
            }
            else
            {
                ViewBag.Message = "ورودی نامعتبر است.";
            }
            return View();
        }

        public string GetMessage(int status)
        {
            switch (status)
            {
                case -1:
                    return "اطلاعات ارسال شده  ناقص است.";
                case -2:
                    return "IP و یا مرچنت کد پذیرنده صحیح نیست.";
                case -3:
                    return "با توجه به محدودیت های شاپرک امکان پرداخت با رقم درخواست شده میسر نمی باشد.";
                case -4:
                    return "سطح تایید پذیرنده پایین تر از سطح نقره ای است.";
                case -11:
                    return "درخواست مورد نظر یافت نشد.";
                case -12:
                    return "امکان ویرایش درخواست میسر نمی باشد.";
                case -21:
                    return "هیچ نوع عملیات مالی برای این تراکنش یافت نشد.";
                case -22:
                    return "تراکنش ناموفق می باشد.";
                case -33:
                    return "رقم تراکنش با رقم پرداخت شده مطابقت ندارد.";
                case -34:
                    return "سقف تقسیم تراکنش از لحاظ تعداد یا رقم عبور نموده است.";
                case -40:
                    return "اجازه دسترسی به متد مربوطه وجود ندارد.";
                case -41:
                    return "اطلاعات ارسال شده مربوط به AdditionalData غیر معتبر می باشد.";
                case -42:
                    return "مدت زمان معتبر طول عمر شناسه پرداخت باید بین 30 دقیقه تا 45 روز می باشد.";
                case -54:
                    return "درخواست مورد نظر آرشیو شده است.";
                case 100:
                    return "عملیات با موفقیت انجام گردیده است.";
                case 101:
                    return "عملیات پرداخت موفق بوده و قبلاً PaymentVerification تراکنش انجام شده است.";
                default:
                    return "کد تعریف نشده.";
            }
        }
    }
}