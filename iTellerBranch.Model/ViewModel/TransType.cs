using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public static class TransType
    {
        /// <summary>
        /// //1-Cashwithdrawal, 2-Cash Deposit, 3 - Cheque Lodgement,4-Clearing Cheque, 5-VaultOut, 6-VVaultIn, 7-TillTransfer, 
        /// //8-Inhouse cheque Deposit, 9 - Inhouse Transfer
        /// </summary>
        public enum TransactionType
        {
            Withdrawal = 1,
            Deposit = 2,
            ChequeLodgement=3,
            ClearingCheque=4,
            VaultOut =5,
            vaultIn = 6,
            TillTransfer=7,
            InHouseChequesDeposit = 8,
            InHouseTransfer=9,
            TreasuryTransfer=10,
            Liquidation = 11,
            ManagerChequeIssuance = 12,
            CashWithDrawalCounter = 13

        }

        public static string getTranTypeName(int tranType)
        {
            string TransTypeName = string.Empty;

            if (tranType == 1)
                return "Cash Withdrawal";
            if (tranType == 2)
                return "Cash Deposit";
            if (tranType == 3)
                return "Cash Withdrawal with Cheque";
            if (tranType == 4)
                return "InHouse Cheque Deposit";
            if (tranType == 5)
                return "Vault Out";
            if (tranType == 6)
                return "Vault In";
            if (tranType == 7)
                return "Till Transfer";
            if (tranType == 8)
                return "Cash Withdrawal with Cheque";
            if (tranType == 9)
                return "In-House Transfer";
            if (tranType == 10)
                return "Treasury Transfer";
            if (tranType == 11)
                return "Liquidation";
            if (tranType == 12)
                return "Cash Withdrawal with Cheque";
            if (tranType == 13)
                return "Cash Withdrawal with Counter Cheque";

            return "";
        }


        public static int getTranType(string tranType)
        {
            string TransTypeName = string.Empty;

            if (tranType == "Withdrawal")
                return 1;
            if (tranType == "Deposit")
                return 2;
            if (tranType == "ChequeLodgement")
                return 3;
            if (tranType == "ClearingCheque")
                return 4;
            if (tranType == "VaultOut")
                return 5;
            if (tranType == "vaultIn")
                return 6;
            if (tranType == "TillTransfer")
                return 7;
            if (tranType == "InHouseChequesDeposit")
                return 8;
            if (tranType == "InHouseTransfer")
                return 9;
            if (tranType == "TreasuryTransfer")
                return 10;
            if (tranType == "Liquidation")
                return 11;
            if (tranType == "ManagerChequeIssuance")
                return 12;
            if (tranType == "CashWithDrawalCounter")
                return 13;
      
  
            return 0;
        }

        public enum TransactionStatus
        {
            [EnumMember(Value = "R")]
            RollOver,
            [EnumMember(Value = "A")]
            Active,
            [EnumMember(Value = "P")]
            Pending,
        }

        public enum ProcessStatus
        {
            Pending = 1,
            Disapproved = 2,
            Updated = 4,
            Approved = 10
        }

        public enum TerminationInstructionCode 
        {
            RollOverPrincipalWithInterest = 1,
            RollOverPrincipal = 2,
            NoRollOver = 3
        }

        public enum ProductCode
        {
            FixedDeposit = 1,
            CallDeposit = 2,
            CollaterizedCallDeposit = 3,
            BankersAcceptances = 4
        }

        public class TextAndValue
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        
    }

    
    
    

}