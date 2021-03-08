//using System;
//using System.CodeDom;
//using System.CodeDom.Compiler;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Security.Permissions;
//using System.Web;
//using System.Web.Services.Description;//this is the issue

//namespace iTellerBranch
//{
//    public class WsProxy
//    {

//        [SecurityPermissionAttribute(SecurityAction.Demand, Unrestricted = true)]
//        public object CallWebService(string webServiceAsmxUrl, string serviceName, string methodName, object[] args)
//        {
////            System.Net.WebClient client = new System.Net.WebClient();


////            //Connect To the web service

////            System.IO.Stream stream = client.OpenRead(webServiceAsmxUrl + "?wsdl";


////            // Now read the WSDL file describing a service.

////ServiceDescription description = ServiceDescription.Read(strviceDescriptionImporter();
//////
////            importer.ProtocolName = "Soap12"; // Use SOAP 1.2.
////eam);

////            ///// LOAD THE DOM /////////

////            // Initialize a service description importer.

////ServiceDescriptionImporter importer = new Ser
////importer.AddServiceDescription(description, null, null);

////            // Generate a proxy client.

////            //importer.Style = ServiceDescriptionImportStyle.Client;

////            // Generate properties to represent primitive values.

////           // importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

////            // Initialize a Code-DOM tree into which we will import the service.

////            CodeNamespace nmspace = new CodeNamespace();
////            CodeCompileUnit unit1 = new CodeCompileUnit();

////            unit1.Namespaces.Add(nmspace);

////            // Import the service into the Code-DOM tree. This creates proxy code that uses the service.

////           // ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit1);
////            if (warning == 0) // If zero then we are good to go
////            {

////                // Generate the proxy code

////                CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

////                // Compile the assembly proxy with the appropriate references

////                string[] assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };
////                CompilerParameters parms = new CompilerParameters(assemblyReferences);
////                CompilerResults results = provider1.CompileAssemblyFromDom(parms, unit1);

////                // Check For Errors

////                if (results.Errors.Count > 0)
////                {
////                    foreach (CompilerError oops in results.Errors)
////                    {
////                        System.Diagnostics.Debug.WriteLine("========Compiler error============");
////                        System.Diagnostics.Debug.WriteLine(oops.ErrorText);
////                    }

////                    throw new System.Exception("Compile Error Occured calling webservice. Check Debug ouput window.");
////                }

////                // Finally, Invoke the web service method

////                object wsvcClass = results.CompiledAssembly.CreateInstance(serviceName);
////                MethodInfo mi = wsvcClass.GetType().GetMethod(methodName);
////                return mi.Invoke(wsvcClass, args);

////            }

////            else
////            {

////                return null;

////            }

//        }


//string url = string.Empty;
// weburl = "http://127.0.0.1/CustomerDetailsServiceMergerSterlingFinal_CH/DepotelCustomerDetails.asmx/CustomerDetailsService/getCustomerDetails2?accountNo={1}&sSortCode="
  //              + sSortCode;

//using (WebClient client = new WebClient()) // calling it but the service is returing internal server error
// {
//     // url = string.Format("{0}/CustomerDetailsService/getCustomerDetails2?accountNo={1}&sSortCode={2}", weburl, accountNo, sSortCode);
//     Uri uri = new Uri(weburl); 
//     client.Headers.Add("content-type", "text/xml");
//     var serviceResponse = client.DownloadString(uri);
//     return Json(serviceResponse);
// }

//    }



//}