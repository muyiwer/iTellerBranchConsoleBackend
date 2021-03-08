using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.BankService
{
    public class ResponseModel<T>
    {
        public int Code { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public T Data { get; set; }

        public static ResponseModel<T> Error(String message)
        {
            return new ResponseModel<T>
            {
                status = false,
                message = message,
                Data = default(T)
            };
        }

        public static ResponseModel<T> Success(T data, String message = "")
        {
            return new ResponseModel<T>
            {
                status = true,
                message = message,
                Data = data
            };
        }
    }
}
