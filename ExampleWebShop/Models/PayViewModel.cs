﻿using System.Collections.Generic;
using System.Web.Mvc;
using PaysonIntegration.Utils;

namespace ExampleWebShop.Models
{
    public class OrderItem
    {
        public string Description { get; set; }
        public string Sku { get; set; }
        public decimal Quantity { get; set; }
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
        public IList<SelectListItem> AvailableFundingConstraint { get; set; }
        public IList<FundingConstraint> SelectedFundingConstraint { get; set; }
        public decimal InvoiceFee { get; set; }
        public string UserId { get; set; }
        public string UserKey { get; set; }
        public GuaranteeOffered GuaranteeOffered { get; set; }
        public string ForwardUrl { get; set; }
        public bool IncludeOrderDetails { get; set; }
    }

    public class ValidateViewModel
    {
        public string UserId { get; set; }
        public string UserKey { get; set; }
    }
}