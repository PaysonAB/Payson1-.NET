using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Data
{
    public class PayData
    {
        //Required properties
        public string ReturnUrl { get; private set; }
        public string CancelUrl { get; private set; }
        public string Memo { get; private set; }
        public Sender Sender { get; private set; }
        private List<Receiver> _receivers;
        public IList<Receiver> Receivers
        {
            get { return _receivers.AsReadOnly(); }
        }

        //Optional properties
        public string IpnNotificationUrl { get; private set; }
        public string LocaleCode { get; private set; }
        public string CurrencyCode { get; private set; }
        private List<FundingConstraint> _fundingConstraints;
        public IList<FundingConstraint> FundingConstraints 
        {
            get
            {
                return _fundingConstraints.AsReadOnly();
            }
        }
        public FeesPayer? FeesPayer { get; set; }
        public decimal? InvoiceFee { get; private set; }
        public string Custom { get; private set; }
        public string TrackingId { get; private set; }
        public GuaranteeOffered? GuaranteeOffered { get; set; }
        private List<OrderItem> _orderItems;
        public IList<OrderItem> OrderItems 
        { 
            get{ return _orderItems.AsReadOnly(); }
        }

        /// <summary>
        /// The arguments are the required parameters
        /// </summary>
        public PayData(string returnUrl, string cancelUrl, string memo, Sender sender, List<Receiver> receivers)
        {
            SetReturnUrl(returnUrl);
            SetCancelUrl(cancelUrl);
            SetMemo(memo);
            SetSender(sender);
            SetReceivers(receivers);

            _fundingConstraints = new List<FundingConstraint>();
            _orderItems = new List<OrderItem>();
        }

        private void SetReceivers(List<Receiver> receivers)
        {
            if (receivers == null || !receivers.Any())
                throw new ArgumentException("At least one receiver must be provided");

            _receivers = receivers;
        }

        private void SetSender(Sender sender)
        {
            if (sender == null)
                throw new ArgumentException("Sender must be provided");

            Sender = sender;
        }

        private void SetMemo(string memo)
        {
            if (string.IsNullOrEmpty(memo))
                throw new ArgumentException("memo cannot be null or empty");
            if(memo.Length > Settings.MemoMaxLength)
                throw new ArgumentException(string.Format("memo can be at most {0} characters long", Settings.MemoMaxLength));

            Memo = memo;
        }

        private void SetCancelUrl(string cancelUrl)
        {
            if (string.IsNullOrEmpty(cancelUrl))
                throw new ArgumentException("cancelUrl cannot be null or empty");
            if (cancelUrl.Length > Settings.UrlMaxLength)
                throw new ArgumentException(string.Format("cancelUrl can be at most {0} characters long", Settings.UrlMaxLength));

            CancelUrl = cancelUrl;
        }

        private void SetReturnUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                throw new ArgumentException("returnUrl cannot be null or empty");
            if(returnUrl.Length > Settings.UrlMaxLength)
                throw new ArgumentException(string.Format("returnUrl can be at most {0} characters long", Settings.UrlMaxLength));

            ReturnUrl = returnUrl;
        }

        public void SetIpnNotificationUrl(string ipnNotificationUrl)
        {
            if (string.IsNullOrEmpty(ipnNotificationUrl))
                throw new ArgumentException("ipnNotificationUrl cannot be null or empty");
            if (ipnNotificationUrl.Length > Settings.UrlMaxLength)
                throw new ArgumentException(string.Format("ipnNotificationUrl can be at most {0} characters long", Settings.UrlMaxLength));

            IpnNotificationUrl = ipnNotificationUrl;
        }

        public void SetLocaleCode(string localeCode)
        {
            if (string.IsNullOrEmpty(localeCode))
                throw new ArgumentException("localeCode cannot be null or empty");
            if (localeCode.Length != Settings.LocaleCodeLength)
                throw new ArgumentException(string.Format("localeCode must be {0} characters long", Settings.LocaleCodeLength));

            LocaleCode = localeCode;
        }

        public void SetCurrencyCode(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                throw new ArgumentException("currencyCode cannot be null or empty");
            if (currencyCode.Length != Settings.CurrencyCodeLength)
                throw new ArgumentException(string.Format("currencyCode must be {0} characters long", Settings.CurrencyCodeLength));

            CurrencyCode = currencyCode;
        }

        public void SetFundingConstraints(List<FundingConstraint> fundingConstraints)
        {
            if(fundingConstraints == null)
                throw new ArgumentException("fundingConstraints cannot be null");

            _fundingConstraints = fundingConstraints;
        }

        public void SetInvoiceFee(decimal amount)
        {
            if (amount < Settings.MinInvoiceFee || amount > Settings.MaxInvoiceFee)
                throw new ArgumentException(string.Format("amount must be in the range [{0}, {1}]", Settings.MinInvoiceFee, Settings.MaxInvoiceFee));

            InvoiceFee = amount;
        }

        public void SetCustom(string custom)
        {
            if (custom == null)
                throw new ArgumentException("custom cannot be null");
            if (custom.Length > Settings.CustomMaxLength)
                throw new ArgumentException(string.Format("custom can be at most {0} characters long", Settings.CustomMaxLength));

            Custom = custom;
        }

        public void SetTrackingId(string trackingId)
        {
            if (trackingId == null)
                throw new ArgumentException("trackingId cannot be null");
            if (trackingId.Length > Settings.TrackingIdMaxLength)
                throw new ArgumentException(string.Format("trackingId can be at most {0} characters long", Settings.TrackingIdMaxLength));

            TrackingId = trackingId;
        }

        public void SetOrderItems(List<OrderItem> orderItems)
        {
            if (orderItems == null)
                throw new ArgumentException("orderItems cannot be null");

            _orderItems = orderItems;
        }

        public IDictionary<string, string> AsNvpDictionary()
        {
            var dictionary = new Dictionary<string, string>
                                 {
                                     {"returnUrl", ReturnUrl},
                                     {"cancelUrl", CancelUrl},
                                     {"memo", Memo},
                                     {"senderEmail", Sender.Email},
                                     {"senderFirstName", Sender.FirstName},
                                     {"senderLastName", Sender.LastName}
                                 };
            for (int i = 0; i < Receivers.Count(); i++ )
            {
                dictionary.Add(string.Format("receiverList.receiver({0}).email", i), Receivers[i].Email);
                dictionary.Add(string.Format("receiverList.receiver({0}).amount", i), FormatDecimal(Receivers[i].Amount));
                if (Receivers[i].IsPrimaryReceiver.HasValue)
                    dictionary.Add(string.Format("receiverList.receiver({0}).primary", i),
                                   Receivers[i].IsPrimaryReceiver.Value ? "true" : "false");
                if (Receivers[i].FirstName != null)
                    dictionary.Add(string.Format("receiverList.receiver({0}).firstName", i), Receivers[i].FirstName);
                if (Receivers[i].LastName != null)
                    dictionary.Add(string.Format("receiverList.receiver({0}).lastName", i), Receivers[i].LastName);
            }

            //Optional parameters
            if(IpnNotificationUrl != null)
                dictionary.Add("ipnNotificationUrl", IpnNotificationUrl);
            if(LocaleCode != null)
                dictionary.Add("localeCode", LocaleCode);
            if (CurrencyCode != null)
                dictionary.Add("currencyCode", CurrencyCode);
            if(FundingConstraints != null)
                for (int i = 0; i < FundingConstraints.Count(); i++)
                    dictionary.Add(string.Format("fundingList.fundingConstraint({0}).constraint", i), FundingConstraints[i].ToString().ToUpper());
            if(FeesPayer.HasValue)
                dictionary.Add("feesPayer", FeesPayer.Value.ToString().ToUpper());
            if(InvoiceFee.HasValue)
                dictionary.Add("invoiceFee", FormatDecimal(InvoiceFee.Value));
            if(Custom != null)
                dictionary.Add("custom", Custom);
            if(TrackingId != null)
                dictionary.Add("trackingId", TrackingId);
            if(GuaranteeOffered.HasValue)
                dictionary.Add("guaranteeOffered", GuaranteeOffered.Value.ToString().ToUpper());
            if (OrderItems != null)
            {
                for (int i = 0; i < OrderItems.Count(); i++)
                {
                    dictionary.Add(string.Format("orderItemList.orderItem({0}).description", i), OrderItems[i].Description);
                    if(OrderItems[i].Sku != null)
                    {
                        dictionary.Add(string.Format("orderItemList.orderItem({0}).sku", i), OrderItems[i].Sku);
                        dictionary.Add(string.Format("orderItemList.orderItem({0}).quantity", i), FormatDecimal(OrderItems[i].Quantity));
                        dictionary.Add(string.Format("orderItemList.orderItem({0}).unitPrice", i), FormatDecimal(OrderItems[i].UnitPrice));
                        dictionary.Add(string.Format("orderItemList.orderItem({0}).taxPercentage", i), FormatDecimal(OrderItems[i].TaxPercentage));
                    }
                }
            }
            return dictionary;
        }

        private static string FormatDecimal(decimal d)
        {
            return d.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}
