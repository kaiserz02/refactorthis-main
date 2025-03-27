using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Domain.Models;
using RefactorThis.Domain.Services;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private InvoiceRepository _repo;
        private InvoiceService _paymentProcessor;

        [SetUp]
        public void Setup() {
            _repo = new InvoiceRepository();
            _paymentProcessor = new InvoiceService(_repo);
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference() {
            var payment = new Payment();

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentProcessor.ProcessPayment(payment));
            Assert.AreEqual("There is no invoice matching this payment", ex.Message);
        }

        [Test]
        public void ProcessPayment_Should_ReturnNoPaymentNeeded_When_InvoiceAmountIsZeroAndNoPaymentsExist() {
            var invoice = new Invoice { Reference = "INV002", Amount = 0, AmountPaid = 0, Payments = null };
            _repo.Add(invoice);

            var payment = new Payment { Reference = "INV002" };

            var result = _paymentProcessor.ProcessPayment(payment);
        }
        [Test]
        public void ProcessPayment_Should_ReturnInvoiceFullyPaid_When_InvoiceIsAlreadyPaid() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 10,
                Payments = new List<Payment> { new Payment {Reference = "INV002", Amount = 10 } }
            };
            _repo.Add(invoice);

            var payment = new Payment { Reference = "INV002" }; 

            var result = _paymentProcessor.ProcessPayment(payment);

            Assert.AreEqual("invoice was already fully paid", result);
        }


        [Test]
        public void ProcessPayment_Should_ReturnError_When_PaymentExceedsRemainingBalance() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment> { new Payment { Reference = "INV002", Amount = 5 } }
            };
            _repo.Add(invoice);

            var result = _paymentProcessor.ProcessPayment(new Payment {Reference = "INV002", Amount = 6 });

            Assert.AreEqual("the payment is greater than the partial amount remaining", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnError_When_PaymentExceedsTotalInvoiceAmount() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 5,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _repo.Add(invoice);

            var payment = new Payment { Reference = "INV002", Amount = 6 };

            var result = _paymentProcessor.ProcessPayment(payment);

            Console.WriteLine($"Actual result: {result}"); 

            Assert.AreEqual("the payment is greater than the invoice amount", result);
        }


        [Test]
        public void ProcessPayment_Should_ReturnFullyPaid_When_FinalPartialPaymentMatchesRemainingAmount() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment> { new Payment {Reference = "INV002", Amount = 5 } }
            };
            _repo.Add(invoice);

            var result = _paymentProcessor.ProcessPayment(new Payment {Reference = "INV002", Amount = 5 });

            Assert.AreEqual("final partial payment received, invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaid_When_SinglePaymentCoversInvoiceAmount() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _repo.Add(invoice);

            var result = _paymentProcessor.ProcessPayment(new Payment {Reference = "INV002", Amount = 10 });

            Assert.AreEqual("invoice is now fully paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartialPayment_When_AmountIsLessThanRemainingBalance() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 5,
                Payments = new List<Payment> { new Payment {Reference = "INV002", Amount = 5 } }
            };
            _repo.Add(invoice);

            var result = _paymentProcessor.ProcessPayment(new Payment {Reference = "INV002", Amount = 1 });

            Assert.AreEqual("invoice is now partially paid", result);
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartialPayment_When_FirstPaymentIsLessThanInvoiceAmount() {
            var invoice = new Invoice
            {
                Reference = "INV002",
                Amount = 10,
                AmountPaid = 0,
                Payments = new List<Payment>()
            };
            _repo.Add(invoice);

            var result = _paymentProcessor.ProcessPayment(new Payment {Reference = "INV002", Amount = 1 });

            Assert.AreEqual("invoice is now partially paid", result);
        }
    }
}
