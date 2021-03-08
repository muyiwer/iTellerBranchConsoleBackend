using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model
{
    public class CustomerDetailModel
    {
        //public ISOCurrency ISOCurrency;
        public string accountNumber;
        public string accountTitle;
        public string account_Group;
        public string account_type;
        public decimal availableBalance;
        public decimal bookBalance;
        public string branch;
        public DateTime dateOpened;
        public string emailAddress;
        public string phoneNumber;
        public string productType;
        public string remarks;
        public List<byte[]> signature;
        public List<string> signatory;
        //public byte[] signature1;
        //public byte[] signature2;
        //public byte[] signature3;
        //public byte[] signature4;
        //public byte[] picture;
        public string contactName;
        public string lastMovementDate;
        //public List<mandateInfo> MandateInfo;
        public bool hasMandate;
        public string accountStatus;
        public string sweepAccount;
        public string sweepAmount;
        public string accountOfficer;
        public string chequeStatus;
        public decimal lienAmount;
        public decimal overdraft;
        public string BranchCode;
        public string custId;
        public bool validForClearingCheque; 
        public string customer_ledger;
        public string CustomerBVN;

        public string Name { get; set; }

        public int Code { get; set; }

        public string Abbreviation { get; set; }
    }

    //public class ISOCurrency
    //{
    //    public string Name { get; set; }

    //    public int Code { get; set; }

    //   public string Abbreviation { get; set; }

    //}

}
