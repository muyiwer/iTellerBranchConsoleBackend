using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class SystemConfigurationModel
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }

        public string PreviousKeyValue { get; set; }

        public string Description { get; set; }
        public string PreviousDescription { get; set; }
        public bool UserEditable { get; set; }
        public Nullable<int> Autofield { get; set; }
        public string userId { get; set; }
    }
}
