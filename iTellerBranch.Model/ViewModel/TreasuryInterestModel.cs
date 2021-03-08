using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TreasuryInterestModel
    {
        public int Id { get; set; }
        public Nullable<int> DealId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> NoOfDaysInYear { get; set; }
        public Nullable<decimal> InterestRate { get; set; }
        public Nullable<decimal> InterestAmount { get; set; }
                
    }
}
