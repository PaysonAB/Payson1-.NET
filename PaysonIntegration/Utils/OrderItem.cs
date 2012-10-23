using System;

namespace PaysonIntegration.Utils
{
    public class OrderItem
    {
        private const int MaxSkuLength = 128;
        private const int MaxDescriptionLength = 128;

        public string Description { get; private set; }
        public string Sku { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TaxPercentage { get; private set; }

        public OrderItem(string description)
        {
            SetDescription(description);
        }

        public void SetOptionalParameters(string sku, decimal quantity, decimal unitPrice, decimal taxPercentage)
        {
            SetSku(sku);
            SetQuantity(quantity);
            SetUnitPrice(unitPrice);
            SetTaxPercentage(taxPercentage);
        }

        private void SetTaxPercentage(decimal taxPercentage)
        {
            if (taxPercentage < 0.0m || taxPercentage > 1.0m)
                throw new ArgumentException("taxPercentage must be range [0.0, 1.0]");

            TaxPercentage = taxPercentage;
        }

        private void SetUnitPrice(decimal unitPrice)
        {
            UnitPrice = unitPrice;
        }

        private void SetQuantity(decimal quantity)
        {
            if (quantity <= 0.0m)
                throw new ArgumentException("quantity must be a positive value");

            Quantity = quantity;
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("description cannot be null or empty");
            if (description.Length > MaxDescriptionLength)
                throw new ArgumentException(string.Format("description may not have more than {0} characters", MaxDescriptionLength));

            Description = description;
        }

        private void SetSku(string sku)
        {
            if(string.IsNullOrEmpty(sku))
                throw new ArgumentException("sku cannot be null or empty");
            if(sku.Length > MaxSkuLength)
                throw new ArgumentException(string.Format("sku may not have more than {0} characters", MaxSkuLength));

            Sku = sku;
        }

        
    }
}
