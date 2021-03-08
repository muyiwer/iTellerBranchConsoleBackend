using iTellerBranch.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Business.Transaction
{
    public class ReportBusiness
    {
        private readonly ReportService _reportService;
        public ReportBusiness()
        {
            _reportService = new ReportService();
        }

        public object GetVaultTransaction
            (DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.GetVaultTransaction(dtFrom, dtTo);
        }

        public object TransactionReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.TransactionReport(dtFrom, dtTo);
        }

        public object PostedCallOverReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.PostedCallOverReport(dtFrom, dtTo);
        }
        public object TellerCastReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.TellerCastReport(dtFrom, dtTo);
        }
        public object TillReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.TillReport(dtFrom, dtTo);
        }
        public object AuditReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.AuditReport(dtFrom, dtTo);
        }
        public object TreasuryDealReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.TreasuryDealReport(dtFrom, dtTo);
        }
        public object TerminationReport(DateTime dtFrom, DateTime dtTo)
        {
            return _reportService.TerminationReport(dtFrom, dtTo);
        }

        public object ChequeIssuanceReport(DateTime dtFrom, DateTime dtTo)
        { 
            return _reportService.ChequeIssuanceReport(dtFrom, dtTo);
        }

        public object OutwardChequeReport(DateTime dtFrom, DateTime dtTo) 
        {
            return _reportService.OutwardChequeReport(dtFrom, dtTo);
        }
    }
}
