using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace iTellerBranch
{
    public static class Utils
    {
        public static void LogNO(string message)
        {//NO for Neft Outward
            string path = AppDomain.CurrentDomain.BaseDirectory;
                      
            try
            {
                if (path.Trim() != "")	//there may be instances where logging to file may not be possible or desirable
                {

                    int maxSize = 204800;       //default max log size 200KB


                    //  path = path + "\\EventLog4NO.txt";
                    path = @"C:\itellerBranchConsoleLogs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = @"C:\ItellerBranchConsoleLogs\AppLog.txt";
                    FileInfo fInfo = new FileInfo(path);



                    if (message.Equals("clearAll"))
                    {
                        //File.WriteAllText(path, String.Empty);
                        using (var sr = new StreamWriter(path, true))
                            sr.WriteLine(String.Empty);
                    }
                    else
                    {
                        string logtxt = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt") + ": " + message;
                        // File.AppendAllText(path+"\\EventLogIBS", logtxt + Environment.NewLine);
                        using (var sr = new StreamWriter(path, true))
                            sr.WriteLine(logtxt);

                    }
                    if (fInfo.Exists)
                    {
                        if (fInfo.Length >= maxSize)
                        {
                            var fname = fInfo.FullName.Replace(fInfo.Extension, "");

                            fInfo.MoveTo(fname + "_" + DateTime.Now.ToString("dd_MM_yyyy_h_mm_ss_tt") + ".txt");
                        }
                    }

                }
            }
            catch { }
        }
        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }


        public static string VaultUrl(bool iSVaultIn, string currencyAbbrev)
        {
            string url = "";
            if (iSVaultIn && currencyAbbrev == "NGN")
            {
                url = System.Configuration.ConfigurationManager.AppSettings["TillToVaultLocal"];
            }
            else if (iSVaultIn && currencyAbbrev != "NGN")
            {
                url = System.Configuration.ConfigurationManager.AppSettings["TillToVaultForeign"];
            }else if (!iSVaultIn && currencyAbbrev != "NGN")
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VaultToTillForeign"];
            }
            else
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VaultToTillLocal"];
            }
            return url;
        }
 

      
        private static int getRandomNumber()
        {
            string str = Guid.NewGuid().ToString();
            string[] s = str.Split("-".ToCharArray());
            return Int32.Parse(s[0], NumberStyles.AllowHexSpecifier);
        }
        public static string generateRandomNo()
        {
            var ItemSequenceNo = "";
            Random random = new Random(getRandomNumber());

            return ItemSequenceNo = random.Next(1, 999999).ToString().PadLeft(6, Convert.ToChar("0")) +
                                    random.Next(1, 99).ToString().PadLeft(2, Convert.ToChar("0")) +
                                    random.Next(1, 999999).ToString().PadLeft(6, Convert.ToChar("0"));

        }

      


     

    }
}