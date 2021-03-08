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
                    path = @"C:\itellerBranchConsoleLogs\AppLog.txt";
                   
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

 

        public static string gen_Digits(int length)
        {
            var rndDigits = new System.Text.StringBuilder().Insert(0, "0123456789", length).ToString().ToCharArray();
            return string.Join("", rndDigits.OrderBy(o => Guid.NewGuid().ToString("D")).Take(length));
        }

        public static string GenerateRandomString(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            char ch;
            long chi;
            for (int i = 0; i < size; i++)
            {
                chi = Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65));
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder2.Append(chi);
                builder.Append(ch);
            }

            return builder.ToString() + builder2.ToString();
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

        public static void doBulkCopy(DataTable recordset, string[] selected, string tblname, string filename, string connectionString)
        {

            var conn = new SqlConnection(connectionString);


            try
            {
                conn.Open();
                var dt = new DataView(recordset).ToTable(false, selected);


                SqlBulkCopy bulkcopy = new SqlBulkCopy(conn);

                bulkcopy.BulkCopyTimeout = 0;

                bulkcopy.DestinationTableName = tblname;

                bulkcopy.WriteToServer(dt);

                bulkcopy.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
               
                throw (ex);
            }

        }


        public static void doAllBulkCopy(DataTable recordset, string[] selected, string tblname, string connection)
        {

            var conn = new SqlConnection(connection);

            try
            {
                conn.Open();
                var dt = new DataView(recordset).ToTable(false, selected);

                SqlBulkCopy bulkcopy = new SqlBulkCopy(conn);

                bulkcopy.BulkCopyTimeout = 0;

                bulkcopy.DestinationTableName = tblname;

                bulkcopy.WriteToServer(dt);

                bulkcopy.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                
                throw (ex);
            }

        }

        public static IEnumerable<DataTable> EnumerateRowsInBatches(DataTable table, int batchSize)
        {
            int rowCount = table.Rows.Count;
            int batchIndex = 0;
            DataTable result = table.Clone(); // This will not change, avoid recreate it
            while (batchIndex * batchSize < rowCount)
            {
                result.Rows.Clear(); // Reuse that DataTable, clear previous results
                int batchStart = batchIndex * batchSize;
                int batchLimit = (batchIndex + 1) * batchSize;
                if (rowCount < batchLimit)
                    batchLimit = rowCount;

                for (int i = batchStart; i < batchLimit; i++)
                    result.Rows.Add(table.Rows[i].ItemArray); // Avoid ImportRow

                batchIndex++;
                yield return result;
            }
        }

    }
}