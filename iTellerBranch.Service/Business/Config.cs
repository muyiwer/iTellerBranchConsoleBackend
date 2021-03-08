using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Service.Business
{
    public static class Config
    {
        public static string GetConfigValue(string _key)
        {
            return ConfigurationManager.AppSettings[_key];
        }
    }
}
