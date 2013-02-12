using System.Collections.Generic;

namespace ExampleWebShop.Models
{
    public class OrderItem
    {
        public string Description { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxPercentage { get; set; }
    }

    public class Receiver
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Sender
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public enum PaymentMethod
    {
        PaysonDirect,
        PaysonInvoice
    }

    public enum GuaranteeOffered
    {
        OPTIONAL,
        REQUIRED,
        NO
    }

    public class PayViewModel
    {
        public string Memo { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public Receiver Receiver { get; set; }
        public Sender Sender { get; set; }
        public string LocaleCode { get; set; }
        public string CurrencyCode { get; set; }
        public string FundingConstraint { get; set; }
        public decimal InvoiceFee { get; set; }

        public string UserId { get; set; }
        public string UserKey { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public GuaranteeOffered GuaranteeOffered { get; set; }

        public string ForwardUrl { get; set; }
        public bool IncludeOrderDetails { get; set; }
    }
}