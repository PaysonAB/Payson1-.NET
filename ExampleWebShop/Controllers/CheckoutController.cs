using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ExampleWebShop.Models;
using PaysonIntegration;
using PaysonIntegration.Data;
using PaysonIntegration.Utils;
using GuaranteeOffered = ExampleWebShop.Models.GuaranteeOffered;
using OrderItem = ExampleWebShop.Models.OrderItem;
using Receiver = ExampleWebShop.Models.Receiver;
using Sender = ExampleWebShop.Models.Sender;

namespace ExampleWebShop.Controllers
{
    public class CheckoutController : Controller
    {
        private const string ApplicationId = "Payson Demo WebShop 1.0";
        private Repository repository;

        private PayViewModel GetDefaultPayViewModel()
        {
            var m = new PayViewModel();
            m.CurrencyCode = "SEK";
            m.InvoiceFee = 0;
            m.LocaleCode = "EN";
            m.Memo = "Various items from the demo shop";
            m.OrderItems = new List<OrderItem>();
            var orderItem = new OrderItem
            {
                Description = "Item X",
                Quantity = 2,
                Sku = "test X",
                TaxPercentage = 0,
                UnitPrice = 23
            };
            m.OrderItems.Add(orderItem);

            m.Receiver = new Receiver
            {
                Email = ConfigurationManager.AppSettings["Receiver.Email"] ?? "testagent-1@payson.se",
                FirstName = "Sven",
                LastName = "Svensson"
            };
            m.Sender = new Sender
            {
                Email = "test-shopper@payson.se",
                FirstName = "Anders",
                LastName = "Andersson"
            };

            
            m.UserId = ConfigurationManager.AppSettings["PAYSON-SECURITY-USERID"] ?? "2";
            m.UserKey = ConfigurationManager.AppSettings["PAYSON-SECURITY-PASSWORD"] ?? "2acab30d-fe50-426f-90d7-8c60a7eb31d4";

            m.GuaranteeOffered = GuaranteeOffered.NO;

            m.ForwardUrl = null;
            return m;
        }


        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            repository = new Repository(requestContext.HttpContext);
        }

        public ActionResult Index()
        {
            return View(GetDefaultPayViewModel());
        }

        [HttpPost]
        public ActionResult Pay(PayViewModel payViewModel)
        {
            var orderGuid = Guid.NewGuid().ToString();

            // We remove port info to help when the site is behind a load balancer/firewall that does port rewrites.
            var scheme = Request.Url.Scheme;
            var host = Request.Url.Host;
            var oldPort = Request.Url.Port.ToString();
            var returnUrl = Url.Action("Returned", "Checkout", new RouteValueDictionary(), scheme, host).Replace(oldPort, "") + "?orderGuid=" + orderGuid;

            var cancelUrl = Url.Action("Cancelled", "Checkout", new RouteValueDictionary(), scheme, host).Replace(oldPort, "");

            // When the shop is hosted by Payson the IPN scheme must be http and not https
            var ipnNotificationUrl = Url.Action("IPN", "Checkout", new RouteValueDictionary(), "http", host).Replace(oldPort, "") + "?orderGuid=" + orderGuid;

            var sender = new PaysonIntegration.Utils.Sender(payViewModel.Sender.Email);
            sender.SetFirstName(payViewModel.Sender.FirstName);
            sender.SetLastName(payViewModel.Sender.LastName);

            var totalAmount = payViewModel.OrderItems.Sum(o => o.UnitPrice * o.Quantity * (1 + o.TaxPercentage));

            var receiver = new PaysonIntegration.Utils.Receiver(payViewModel.Receiver.Email, totalAmount);
            receiver.SetFirstName(payViewModel.Receiver.FirstName);
            receiver.SetLastName(payViewModel.Receiver.LastName);
            receiver.SetPrimaryReceiver(true);

            var payData = new PayData(returnUrl, cancelUrl, payViewModel.Memo, sender,
                                      new List<PaysonIntegration.Utils.Receiver> { receiver });

            switch (payViewModel.GuaranteeOffered)
            {
                case GuaranteeOffered.NO:
                    payData.GuaranteeOffered = PaysonIntegration.Utils.GuaranteeOffered.No;
                    break;
                case GuaranteeOffered.OPTIONAL:
                    payData.GuaranteeOffered = PaysonIntegration.Utils.GuaranteeOffered.Optional;
                    break;
                case GuaranteeOffered.REQUIRED:
                    payData.GuaranteeOffered = PaysonIntegration.Utils.GuaranteeOffered.Required;
                    break;
                default:
                    payData.GuaranteeOffered = null;
                    break;
            }

            payData.SetCurrencyCode(payViewModel.CurrencyCode);
            var fundingConstraints = new List<FundingConstraint>();
            if (payViewModel.PaymentMethod == PaymentMethod.PaysonInvoice)
            {
                fundingConstraints.Add(FundingConstraint.Invoice);
            }
            else
            {
                fundingConstraints.Add(FundingConstraint.Bank);
                fundingConstraints.Add(FundingConstraint.CreditCard);
            }

            payData.SetFundingConstraints(fundingConstraints);

            payData.SetInvoiceFee(payViewModel.InvoiceFee);
            payData.SetIpnNotificationUrl(ipnNotificationUrl);
            payData.SetLocaleCode(payViewModel.LocaleCode);
            if (payViewModel.PaymentMethod == PaymentMethod.PaysonInvoice || payViewModel.IncludeOrderDetails)
            {
                var orderItems = new List<PaysonIntegration.Utils.OrderItem>();
                foreach (var orderModel in payViewModel.OrderItems)
                {
                    var oi = new PaysonIntegration.Utils.OrderItem(orderModel.Description);
                    oi.SetOptionalParameters(orderModel.Sku, orderModel.Quantity, orderModel.UnitPrice,
                                             orderModel.TaxPercentage);
                    orderItems.Add(oi);
                }
                payData.SetOrderItems(orderItems);
            }
            payData.SetTrackingId(orderGuid);

            var api = new PaysonApi(payViewModel.UserId, payViewModel.UserKey, ApplicationId, true);


            var response = api.MakePayRequest(payData);
            
            if (response.Success)
            {
                var state = new PurchaseState
                                {
                                    UserId = payViewModel.UserId,
                                    UserKey = payViewModel.UserKey,
                                    Token = response.Token,
                                    Updates = new Dictionary<DateTime, string> {{DateTime.Now, "Created"}},
                                    OrderGuid = orderGuid,
                                    LatestStatus = PaymentStatus.Created.ToString(),
                                    ReceiverEmail = receiver.Email,
                                };

                repository.SavePurchaseState(state);

                string forwardUrl =
                    string.IsNullOrWhiteSpace(payViewModel.ForwardUrl)
                        ? api.GetForwardPayUrl(response.Token)
                        : payViewModel.ForwardUrl + response.Token;

                return Redirect(forwardUrl);
            }
            ViewBag.Errors = response.ErrorMessages;

            return View("Index", GetDefaultPayViewModel());
        }


        public ActionResult Returned(string orderGuid)
        {
            var state = repository.GetPurchaseState(orderGuid);

            if (state != null)
            {
                var api = new PaysonApi(state.UserId, state.UserKey, ApplicationId, true);
                var response = api.MakePaymentDetailsRequest(new PaymentDetailsData(state.Token));

                if (response.Success)
                {
                    var status = response.PaymentDetails.PaymentStatus.HasValue
                                     ? response.PaymentDetails.PaymentStatus.ToString()
                                     : "N/A";
                    state.Updates[DateTime.Now] = "ReturnUrl: " + status;
                    state.LatestStatus = status;
                }
                else
                {
                    ViewBag.Errors = response.ErrorMessages;
                    return View("Index", GetDefaultPayViewModel());
                }
            }

            return View("Result", state);
        }

        public ActionResult Cancelled()
        {
            return View();
        }

        public ActionResult IPN(string orderGuid)
        {
            Request.InputStream.Position = 0;
            var content = new StreamReader(Request.InputStream).ReadToEnd();

            var state = repository.GetPurchaseState(orderGuid);

            if (state != null)
            {
                var api = new PaysonApi(state.UserId, state.UserKey, ApplicationId, true);
                var response = api.MakeValidateIpnContentRequest(content);
                if (response.Success)
                {
                    var status = response.ProcessedIpnMessage.PaymentStatus.HasValue
                                     ? response.ProcessedIpnMessage.PaymentStatus.ToString()
                                     : "N/A";
                    state.Updates[DateTime.Now] = "IPN: " + status;
                    state.LatestStatus = status;
                }
                else
                {
                    state.Updates[DateTime.Now] = "IPN: IPN Failure";
                    state.LatestStatus = "Failure";

                }
            }

            return new EmptyResult();
        }

        public ActionResult About()
        {
            return View();
        }

    }


    //In a real web shop this would probably save the purchase to a database and not just into the application state
    public class Repository
    {
        private readonly HttpContextBase context;

        public Repository(HttpContextBase context)
        {
            this.context = context;
        }

        public void SavePurchaseState(PurchaseState state)
        {
            context.Application["Purchase." + state.OrderGuid] = state;
        }

        public PurchaseState GetPurchaseState(string orderGuid)
        {
            return context.Application["Purchase." + orderGuid] as PurchaseState;
        }
    }

}
