using iTellerBranch.Model;
using iTellerBranch.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public static class DoubleEntriesTransfer
    {
        public static bool TransferEntries(List<TransactionModel> transMasters)  
        {
            try
            {
                List<string> transferReference = new List<string>();
                foreach (var transMaster in transMasters)
                {
                    FundTransferModel fundTransferModel = new FundTransferModel();
                    fundTransferModel.FT_Request = new FTRequest();
                    fundTransferModel.FT_Request.TransactionBranch = transMaster.Branch;//ook
                    fundTransferModel.FT_Request.TransactionType = transMaster.ChargeType; //"AVCE";
                    fundTransferModel.FT_Request.DebitAcctNo = transMaster.AccountNo;
                    fundTransferModel.FT_Request.CreditAccountNo = transMaster.CustomerAcctNos;
                    fundTransferModel.FT_Request.DebitCurrency = transMaster.CurrencyAbbrev;
                    fundTransferModel.FT_Request.CreditCurrency = transMaster.CurrCode;
                    fundTransferModel.FT_Request.DebitAmount = "" + Math.Round(Convert.ToDecimal(transMaster.TotalAmt), 2);
                    fundTransferModel.FT_Request.CommissionCode = "";
                    //finding a way to link individual posting with the main using the tranId
                    fundTransferModel.FT_Request.narrations = transMaster.Remark;
                    fundTransferModel.FT_Request.SessionId = transMaster.access_token;
                    fundTransferModel.FT_Request.TrxnLocation = "1";
                    var cbaResponse = ImalService.FundTransferNar(transMaster.url, fundTransferModel, transMaster.access_token);

                    if (cbaResponse.FTResponse.ResponseCode == "00")
                    {
                        transferReference.Add(cbaResponse.FTResponse.ReferenceID);
                    }
                    else
                    {
                        if(transferReference.Count() > 0)
                        {
                            ReverseFundTransfer(transMaster, transferReference);
                        }
                        return false;
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
          
        }

        public static void ReverseFundTransfer(TransactionModel transMaster, List<string> TranReferences)
        {
            foreach(var TReference in TranReferences) 
            {
                TellerReversalRequest request = new TellerReversalRequest();
                request.TellerReversal = new TellerReversalModel();
                request.TellerReversal.access_token = transMaster.access_token;
                request.TellerReversal.TransactionBranch = transMaster.Branch;
                request.TellerReversal.TTReference = TReference;
                Transaction.TellerReversal(transMaster.reversalUrl, request);
            }
            
        }
    }
}
