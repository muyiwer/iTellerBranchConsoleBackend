using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class TransactionDetailsModel
    {
        public int Id { get; set; }
        public int TranId { get; set; }
        public int Counter { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
