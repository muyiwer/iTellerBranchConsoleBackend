using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class TransactionMasterModel
    {
        public int ID { get; set; }
        public Nullable<int> TranID { get; set; }
        public string AccountNo { get; set; }
        public string GLAccountNo { get; set; }
        public string TillNo { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> TransType { get; set; }
        public string TellerId { get; set; }
        public string CrDr { get; set; }
    }
}
