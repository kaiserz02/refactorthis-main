using System.Collections.Generic;

namespace RefactorThis.Models
{
    public class Invoice
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public InvoiceType Type { get; set; }


    }

    /// <summary>
    /// Enum representing different invoice types.
    /// </summary>
    public enum InvoiceType
    {
        Standard,
        Commercial
    }

}