using System.Collections.Generic;
using System.Globalization;

namespace PaysonIntegration.Utils
{
    public class PaymentDetails
    {
        public string PurchaseId { get; protected set; }
        public PaymentType? PaymentType { get; protected set; }
        public PaymentStatus? PaymentStatus { get; protected set; }

        public GuaranteeStatus? GuaranteeStatus { get; protected set; }
        public string GuaranteeDeadlineTimestamp { get; protected set; }

        public InvoiceStatus? InvoiceStatus { get; protected set; }
        public ShippingAddress ShippingAddress { get; protected set; }

        public Sender Sender { get; protected set; }
        public IList<Receiver> Receivers { get; protected set; }

        public string Custom { get; protected set; }
        public string TrackingId { get; protected set; }
        public string CurrencyCode { get; protected set; }

        public PaymentDetails(IDictionary<string, string> nvpContent)
        {
            InitiatePaymentDetails(nvpContent);
        }

        public PaymentDetails(string nvpContent)
        {
            InitiatePaymentDetails(nvpContent);
        }

        protected void InitiatePaymentDetails(IDictionary<string, string> nvpContent)
        {
            if (nvpContent.ContainsKey("purchaseId"))
                PurchaseId = nvpContent["purchaseId"];
            PaymentType = GetPaymentTypeFromString(nvpContent["type"]);
            PaymentStatus = GetPaymentStatusFromString(nvpContent["status"]);

            if (PaymentType.HasValue && PaymentType.Value == Utils.PaymentType.Guarantee)
            {
                GuaranteeStatus = GetGuaranteeStatusFromString(nvpContent["guaranteeStatus"]);
                GuaranteeDeadlineTimestamp = nvpContent["guaranteeDeadlineTimestamp"];
            }

            if (PaymentType.HasValue && PaymentType.Value == Utils.PaymentType.Invoice)
            {
                InvoiceStatus = GetInvoiceStatusFromString(nvpContent["invoiceStatus"]);
                if (InvoiceStatus == Utils.InvoiceStatus.Done ||
                    InvoiceStatus == Utils.InvoiceStatus.OrderCreated ||
                    InvoiceStatus == Utils.InvoiceStatus.Shipped ||
                    InvoiceStatus == Utils.InvoiceStatus.Credited)
                    ShippingAddress = new ShippingAddress
                                          {
                                              City = nvpContent["shippingAddress.city"],
                                              Country = nvpContent["shippingAddress.country"],
                                              Name = nvpContent["shippingAddress.name"],
                                              PostalCode = nvpContent["shippingAddress.postalCode"],
                                              StreetAddress = nvpContent["shippingAddress.streetAddress"]
                                          };
            }

            Sender = new Sender(nvpContent["senderEmail"]);
            Receivers = GetReceivers(nvpContent);

            Custom = nvpContent.GetValueOrNull("custom");
            TrackingId = nvpContent.GetValueOrNull("trackingId");
            CurrencyCode = nvpContent["currencyCode"];
        }

        protected void InitiatePaymentDetails(string nvpContent)
        {
            InitiatePaymentDetails(NvpCodec.ConvertToNameValueCollection(nvpContent));
        }

        private static InvoiceStatus? GetInvoiceStatusFromString(string s)
        {
            if (s == null)
            {
                return null;
            }

            switch (s.ToUpper())
            {
                case "PENDING":
                    return Utils.InvoiceStatus.Pending;
                case "ORDERCREATED":
                    return Utils.InvoiceStatus.OrderCreated;
                case "CANCELED":
                    return Utils.InvoiceStatus.Canceled;
                case "SHIPPED":
                    return Utils.InvoiceStatus.Shipped;
                case "DONE":
                    return Utils.InvoiceStatus.Done;
                case "CREDITED":
                    return Utils.InvoiceStatus.Credited;
                default:
                    return null;
            }
        }

        private static GuaranteeStatus? GetGuaranteeStatusFromString(string s)
        {
            if (s == null)
            {
                return null;
            }

            switch (s.ToUpper())
            {
                case "WAITINGFORSEND":
                    return Utils.GuaranteeStatus.WaitingForSend;
                case "WAITINGFORACCEPTANCE":
                    return Utils.GuaranteeStatus.WaitingForAcceptance;
                case "WAITINGFORRETURN":
                    return Utils.GuaranteeStatus.WaitingForReturn;
                case "WAITINGFORRETURNACCEPTANCE":
                    return Utils.GuaranteeStatus.WaitingForReturnAcceptance;
                case "RETURNNOTACCEPTED":
                    return Utils.GuaranteeStatus.ReturnNotAccepted;
                case "NOTRECEIVED":
                    return Utils.GuaranteeStatus.NotReceived;
                case "RETURNNOTRECEIVED":
                    return Utils.GuaranteeStatus.ReturnNotReceived;
                case "MONEYRETURNEDTOSENDER":
                    return Utils.GuaranteeStatus.MoneyReturnedToSender;
                case "RETURNACCEPTED":
                    return Utils.GuaranteeStatus.ReturnAccepted;
                default:
                    return null;
            }

        }

        private static IList<Receiver> GetReceivers(IDictionary<string, string> nvpResponseContent)
        {
            var receivers = new List<Receiver>();
            int i = 0;
            while (nvpResponseContent.ContainsKey(string.Format("receiverList.receiver({0}).email", i)))
            {
                var rec = new Receiver(nvpResponseContent[string.Format("receiverList.receiver({0}).email", i)],
                                       decimal.Parse(nvpResponseContent[string.Format("receiverList.receiver({0}).amount", i)], CultureInfo.InvariantCulture));
                if (nvpResponseContent.ContainsKey(string.Format("receiverList.receiver({0}).primary", i)))
                    rec.SetPrimaryReceiver(nvpResponseContent[string.Format("receiverList.receiver({0}).primary", i)].ToUpper() == "TRUE");
                receivers.Add(rec);
                i++;
            }
            return receivers;
        }

        private static PaymentStatus? GetPaymentStatusFromString(string s)
        {
            if (s == null)
            {
                return null;
            }

            switch (s.ToUpper())
            {
                case "CREATED":
                    return Utils.PaymentStatus.Created;
                case "PENDING":
                    return Utils.PaymentStatus.Pending;
                case "PROCESSING":
                    return Utils.PaymentStatus.Processing;
                case "COMPLETED":
                    return Utils.PaymentStatus.Completed;
                case "CREDITED":
                    return Utils.PaymentStatus.Credited;
                case "INCOMPLETE":
                    return Utils.PaymentStatus.Incomplete;
                case "ERROR":
                    return Utils.PaymentStatus.Error;
                case "EXPIRED":
                    return Utils.PaymentStatus.Expired;
                case "REVERSALERROR":
                    return Utils.PaymentStatus.ReversalError;
                case "ABORTED":
                    return Utils.PaymentStatus.Aborted;
                default:
                    return null;
            }
        }

        private static PaymentType? GetPaymentTypeFromString(string s)
        {
            if (s == null)
            {
                return null;
            }

            switch (s.ToUpper())
            {
                case "TRANSFER":
                    return Utils.PaymentType.Direct;
                case "GUARANTEE":
                    return Utils.PaymentType.Guarantee;
                case "INVOICE":
                    return Utils.PaymentType.Invoice;
                default:
                    return null;
            }
        }
    }
}