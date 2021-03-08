using iTellerBranch.Model.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTellerBranch.Model.ViewModel.CurrrencyResponse;

namespace iTellerBranch.BankService
{
    public static class TestDummiAPI
    {
        //public static object Currency() 
        //{
        //    try
        //    {
        //        string url = "http://localhost:83/api/Currency";
        //        string response = APIService.GET(url);
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            var result = JsonConvert.DeserializeObject<Root>(response);

        //            if (result != null)
        //            {
        //                Utils.Log("POSTING WAS SUCCESSFUL WITH DETAILS: " + response);
        //                return result;
        //            }
        //            else
        //            {
        //                Utils.Log("POST FAILURE WITH DETAILS: " + result);
        //                return null;
        //            }
        //        }
        //        else
        //        {
        //            Utils.Log("Empty Response from Client's API");
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Utils.Log("GetAccountFullInfo error message: " + ex.Message);
        //        throw ex;
        //    }
        //}
    }
}
