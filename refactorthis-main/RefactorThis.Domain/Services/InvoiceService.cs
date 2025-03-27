using System;
using System.Linq;
using RefactorThis.Domain.Models;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Services
{
    /// <summary>
    /// Represents core business logic
    /// </summary>
    public class InvoiceService
    {
        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService(InvoiceRepository invoiceRepository) {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment) {
            if (payment == null || string.IsNullOrWhiteSpace(payment.Reference))
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            if (invoice == null)
            {
                throw new InvalidOperationException("There is no invoice matching this payment");
            }

            decimal amountDue = invoice.Amount - invoice.AmountPaid;

            if (invoice.Amount == 0)
            {
                return invoice.Payments == null || !invoice.Payments.Any()
                    ? "no payment needed"
                    : throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            if (invoice.AmountPaid == invoice.Amount)
            {
                return "invoice was already fully paid";
            }

            if (payment.Amount > invoice.Amount)
            {
                return "the payment is greater than the invoice amount";
            }

            if (payment.Amount > amountDue)
            {
                return "the payment is greater than the partial amount remaining";
            }

            if (payment.Amount == amountDue && invoice.AmountPaid > 0)
            {
                return "final partial payment received, invoice is now fully paid";
            }

            return ApplyPayment(invoice, payment, amountDue);
        }



        private string ApplyPayment(Invoice invoice, Payment payment, decimal amountDue) {
            invoice.AmountPaid += payment.Amount;
            invoice.Payments.Add(payment);

            if (invoice.Type == InvoiceType.Commercial)
            {
                invoice.TaxAmount += payment.Amount * 0.14m;
            }

            if (invoice.AmountPaid == invoice.Amount)
            {
                return "invoice is now fully paid";
            }

            return amountDue == payment.Amount
                ? "final partial payment received, invoice is now fully paid"
                : "invoice is now partially paid";
        }
    }
}
