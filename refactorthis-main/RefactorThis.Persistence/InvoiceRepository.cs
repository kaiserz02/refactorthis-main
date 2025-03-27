using RefactorThis.Domain.Models;
using System;
using System.Collections.Generic;
namespace RefactorThis.Persistence
{
    /// <summary>
    /// For data storage and retrieval
    /// </summary>
    public class InvoiceRepository
    {
        // Dictionary to store invoices in memory, using the invoice reference as the key.
        private readonly Dictionary<string, Invoice> _invoices = new Dictionary<string, Invoice>();

        /// <summary>
        /// Retrieves an invoice by its reference.
        /// </summary>
        /// <param name="reference">The unique identifier of the invoice.</param>
        /// <returns>The corresponding Invoice if found; otherwise, null.</returns>
        public Invoice GetInvoice(string reference) {
            if (string.IsNullOrWhiteSpace(reference))
            {
                throw new ArgumentException("Invoice reference cannot be null or empty.", nameof(reference));
            }

            return _invoices.TryGetValue(reference, out var invoice) ? invoice : null;
        }

        /// <summary>
        /// Saves or updates an invoice in the repository.
        /// </summary>
        /// <param name="invoice">The invoice to be saved or updated.</param>
        public void SaveInvoice(Invoice invoice) {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");
            }

            _invoices[invoice.Reference] = invoice;
        }

        /// <summary>
        /// Adds a new invoice to the repository if it does not already exist.
        /// </summary>
        /// <param name="invoice">The invoice to add.</param>
        public void Add(Invoice invoice) {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(invoice.Reference))
            {
                throw new ArgumentException("Invoice reference cannot be null or empty.", nameof(invoice.Reference));
            }

            if (!_invoices.ContainsKey(invoice.Reference))
            {
                _invoices[invoice.Reference] = invoice;
            }
        }
    }
}
