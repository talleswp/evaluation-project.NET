using Ambev.DeveloperEvaluation.Domain.Common;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public string Customer { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Branch { get; private set; }
        public bool IsCancelled { get; private set; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        private Sale() { }

        public Sale(string customer, string branch)
        {
            Customer = customer;
            Branch = branch;
            SaleDate = DateTime.UtcNow;
            SaleNumber = GenerateSaleNumber();
        }

        /// <summary>
        /// Adds a new item to the sale.
        /// </summary>
        /// <param name="productName">The name of the product.</param>
        /// <param name="quantity">The quantity of the product.</param>
        /// <param name="unitPrice">The unit price of the product.</param>
        /// <exception cref="DomainException">Thrown when trying to add more than 20 identical items.</exception>
        public void AddItem(string productName, int quantity, decimal unitPrice)
        {
            if (IsCancelled)
                throw new DomainException("Cannot add items to a cancelled sale.");

            var existingItem = Items.FirstOrDefault(i => i.ProductName == productName);
            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > 20)
                    throw new DomainException($"Cannot sell more than 20 identical items for product '{productName}'. Current: {existingItem.Quantity}, Adding: {quantity}.");

                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                if (quantity > 20)
                    throw new DomainException($"Cannot sell more than 20 identical items in a single transaction.");

                Items.Add(new SaleItem(productName, quantity, unitPrice));
            }
            CalculateTotalAmount();
        }

        /// <summary>
        /// Updates an existing item in the sale.
        /// </summary>
        /// <param name="itemId">The ID of the item to update.</param>
        /// <param name="quantity">The new quantity of the item.</param>
        /// <param name="unitPrice">The new unit price of the item.</param>
        /// <exception cref="DomainException">Thrown when the item is not found or the sale is cancelled.</exception>
        public void UpdateItem(Guid itemId, int quantity, decimal unitPrice)
        {
            if (IsCancelled)
                throw new DomainException("Cannot update items of a cancelled sale.");

            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new DomainException($"Sale item with ID {itemId} not found.");

            item.UpdateQuantity(quantity);
            item.UpdateUnitPrice(unitPrice);
            CalculateTotalAmount();
        }

        /// <summary>
        /// Removes an item from the sale.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        /// <exception cref="DomainException">Thrown when the sale is cancelled.</exception>
        public void RemoveItem(Guid itemId)
        {
            if (IsCancelled)
                throw new DomainException("Cannot remove items from a cancelled sale.");

            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                Items.Remove(item);
                CalculateTotalAmount();
            }
        }

        /// <summary>
        /// Cancels the sale.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the sale is already cancelled.</exception>
        public void CancelSale()
        {
            if (IsCancelled)
                throw new DomainException("Sale is already cancelled.");

            IsCancelled = true;
        }

        /// <summary>
        /// Updates the details of the sale.
        /// </summary>
        /// <param name="customer">The new customer name.</param>
        /// <param name="branch">The new branch name.</param>
        public void UpdateDetails(string customer, string branch)
        {
            Customer = customer;
            Branch = branch;
        }

        internal void CalculateTotalAmount()
        {
            TotalAmount = Items.Sum(i => i.TotalAmount);
        }

        private string GenerateSaleNumber()
        {
            return $"SALE-{DateTime.UtcNow.Ticks}";
        }
    }
}
