using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public Guid SaleId { get; private set; }
        public virtual Sale Sale { get; private set; }

        private SaleItem() { }

        public SaleItem(string productName, int quantity, decimal unitPrice)
        {
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CalculateDiscount();
            CalculateTotalAmount();
        }

        /// <summary>
        /// Updates the quantity of the sale item and recalculates the discount and total amount.
        /// </summary>
        /// <param name="newQuantity">The new quantity of the sale item.</param>
        /// <exception cref="DomainException">Thrown when the new quantity is less than or equal to zero.</exception>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            Quantity = newQuantity;
            CalculateDiscount();
            CalculateTotalAmount();
        }

        /// <summary>
        /// Updates the unit price of the sale item and recalculates the total amount.
        /// </summary>
        /// <param name="newUnitPrice">The new unit price of the sale item.</param>
        /// <exception cref="DomainException">Thrown when the new unit price is less than or equal to zero.</exception>
        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice <= 0)
                throw new DomainException("Unit price must be greater than zero.");

            UnitPrice = newUnitPrice;
            CalculateTotalAmount();
        }

        /// <summary>
        /// Calculates the discount based on the quantity of the sale item.
        /// This method implements the following business rules:
        /// - Purchases above 4 identical items have a 10% discount
        /// - Purchases between 10 and 20 identical items have a 20% discount
        /// - Purchases below 4 items cannot have a discount
        /// </summary>
        internal void CalculateDiscount()
        {
            if (Quantity >= 10)
                Discount = 0.20m;
            else if (Quantity >= 4)
                Discount = 0.10m;
            else
                Discount = 0;
        }

        /// <summary>
        /// Calculates the total amount of the sale item, including the discount.
        /// </summary>
        internal void CalculateTotalAmount()
        {
            TotalAmount = (Quantity * UnitPrice) * (1 - Discount);
        }
    }
}
