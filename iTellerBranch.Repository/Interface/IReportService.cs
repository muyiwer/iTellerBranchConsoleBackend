using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    public interface IReportService
    {
        object GetVaultTransaction(DateTime dtFrom, DateTime dtTo);

        
        object TellerCastReport(DateTime dtFrom, DateTime dtTo);

        object TransactionReport(DateTime dtFrom, DateTime dtTo);

        object TillReport(DateTime dtFrom, DateTime dtTo);
        object AuditReport(DateTime dtFrom, DateTime dtTo);
        object TreasuryDealReport(DateTime dtFrom, DateTime dtTo);
        object TerminationReport(DateTime dtFrom, DateTime dtTo);
    }
}
