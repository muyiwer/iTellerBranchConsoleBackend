using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class TillTransferRequest
    {
        public TillTransferLCYModel TiiTransferLCY { get; set; }
    }

    public class TillTransferRequestForeign
    {
        public TillTransferLCYModel TiiTransferFCY { get; set; } 
    }
}
