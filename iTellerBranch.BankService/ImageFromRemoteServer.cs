using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public static class ImageFromRemoteServer
    {
        public static string returnString;
        public static string myNetworkPath;
        public static string GetBase64String(string networkPath, string fileName, string userName, string password)
        {
            networkPath = networkPath.Trim();
            fileName = fileName.Trim();
            userName = userName.Trim();
            password = password.Trim();

            NetworkCredential credentials = new NetworkCredential(userName, password);

            byte[] downloadedFile = DownloadFileByte(networkPath, userName, password, fileName);
            string returnString = Convert.ToBase64String(downloadedFile);

            return returnString;
        }

        // method to do grunt work
        public static byte[] DownloadFileByte(string networkPath, string userName, string password, string fileName)
        {
            byte[] fileBytes = null;
            NetworkCredential credentials = new NetworkCredential(userName, password);

            using (new ConnectToSharedFolder(networkPath, credentials))
            {
                var filePath = Directory.GetFiles(networkPath, fileName);
                string finalPAth = filePath[0];

                myNetworkPath = finalPAth;

                try
                {
                    fileBytes = File.ReadAllBytes(myNetworkPath);
                }
                catch (Exception ex)
                {
                    Utils.Log("Error Retrieving image from path: "+ ex.Message);
                    string Message = ex.Message.ToString();
                }
            }

            return fileBytes;
        }
    }

}

