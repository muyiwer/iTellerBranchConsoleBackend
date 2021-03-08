using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Interface
{
    public interface IGeneralService
    {
        object DetermineCompName(string IP);
    }
}
