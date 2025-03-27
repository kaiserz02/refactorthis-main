using System.Collections.Generic;

namespace RefactorThis.Domain.Models
{   
     /// <summary>
     /// Represents an invoice in the system.
     /// 
     /// This class was moved from the Persistence project to the Domain.Models project 
     /// to enforce a clean separation of concerns. The Persistence layer should only 
     /// handle data storage and retrieval, while domain models should reside in a shared 
     /// library that can be accessed by both the domain logic and persistence layers.
     /// 
     /// This change prevents circular dependencies and allows the domain models to be 
     /// referenced by both .NET Framework and .NET Core projects if needed in the future.
     /// </summary>
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