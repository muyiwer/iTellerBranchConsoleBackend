using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TransMaster
    {
        public string CustomerAcctNos { get; set; }
        public decimal TotalAmt { get; set; }
        public string CashierID { get; set; }
        public string CashierTillNos { get; set; }
        public string CashierTillGL { get; set; }
        public string DepositorName { get; set; }
        public string DepositorPhoneNo { get; set; }
        public Nullable<byte> Status { get; set; }
        public Nullable<System.DateTime> WhenApproved { get; set; }
        public string SortCode { get; set; }
        public Nullable<int> Currency { get; set; }
        public Nullable<System.DateTime> ValueDate { get; set; }
        public string SupervisoryUser { get; set; }
        public string Narration { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public string Occupation { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string MotherMaidenNAme { get; set; }
        public string Address { get; set; }
        public Nullable<int> IDType { get; set; }
        public string IDNo { get; set; }
        public int TranId { get; set; }
        public string MachineName { get; set; }
        public Nullable<bool> NeededApproval { get; set; }
        public string Beneficiary { get; set; }
        public string Chequeno { get; set; }
        public Nullable<System.DateTime> DateonCheque { get; set; }
        public Nullable<int> TransType { get; set; }

       
    }
}
