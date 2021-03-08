using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class ManagerChequeIssuanceDetailsModel
    {
        public int ID { get; set; }
        public Nullable<long> TranId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string DraftNumber { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public int TemplateCode { get; set; } 
    }
}
