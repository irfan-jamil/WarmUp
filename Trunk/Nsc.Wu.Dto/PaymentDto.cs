using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsc.Wu.Dto
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> PayerId { get; set; }
        public Nullable<int> BeneficiaryId { get; set; }
        public string TransactionId { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> DeductionAmont { get; set; }
        public Nullable<double> TransferAmount { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string EventEntryCode { get; set; }
        public string BarCode { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
 


    }
}
