using System;

namespace RefactorThis.Domain.Models
{
	public class Payment
	{
		public decimal Amount { get; set; }
		public string Reference { get; set; }

        /// <summary>
        /// Added date/time for payment documentation purposes
        /// </summary>
        public DateTime PaymentDate { get; set; }

    }
}