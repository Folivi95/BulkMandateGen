using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkMandateGen.Data
{
    public class SetupMandate
    {
        public int Id { get; set; }
        public string PayerName { get; set; }
        public string PayerEmail { get; set; }
        public string PayerPhone { get; set; }
        public string Amount { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string MandateType { get; set; }
        public string MaxNoOfDebits { get; set; }
        public string PayerAccount { get; set; }
        public string PayerBankCode { get; set; }
        public string MandateId { get; set; }
        public string RequestId { get; set; }
    }
}
