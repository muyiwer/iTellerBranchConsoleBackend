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


                    path = path + "\\ServiceLog.txt";
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
        

    }
}