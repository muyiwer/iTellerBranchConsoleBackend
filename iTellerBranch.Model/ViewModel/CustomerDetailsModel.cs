using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class CustomerDetailsModel
    {
        public class CustomerInfo 
        {
            public string _ID { get; set; }
            [JsonProperty("CUSTOMER.TYPE")]
            public string CUSTOMERTYPE { get; set; }
            [JsonProperty("COMPANY.BOOK")]
            public string COMPANYBOOK { get; set; }
            public string TITLE { get; set; }
            [JsonProperty("FAMIL.NAME")]
            public string FAMILNAME { get; set; }
            [JsonProperty("GIVEN.NAMES")]
            public string GIVENNAMES { get; set; }
            public string GENDER { get; set; }
            public string NATIONALITY { get; set; }
            [JsonProperty("DATE.OF.BIRTH")]
            public string DATEOFBIRTH { get; set; }
            [JsonProperty("LEGAL.DOC.NAME")]
            public string LEGALDOCNAME { get; set; }
            [JsonProperty("LEGAL.ID")]
            public string LEGALID { get; set; }
            [JsonProperty("CUSTOMER.SINCE")]
            public string CUSTOMERSINCE { get; set; }
            [JsonProperty("ACCOUNT.OFFICER")]
            public string ACCOUNTOFFICER { get; set; }
            public string SECTOR { get; set; }
            [JsonProperty("SECTOR.DESC")]
            public string SECTORDESC { get; set; }
            public string INDUSTRY { get; set; }
            [JsonProperty("INDUSTRY.DESC")]
            public string INDUSTRYDESC { get; set; }
            public object OCCUPATION { get; set; }
            [JsonProperty("EMPLOYERS.NAME")]
            public List<object> EMPLOYERSNAME { get; set; }
            [JsonProperty("EMPLOYMENT.STATUS")]
            public string EMPLOYMENTSTATUS { get; set; }
            [JsonProperty("NET.MONTHLY.IN")]
            public object NETMONTHLYIN { get; set; }
            [JsonProperty("EMPLOYMENT.START")]
            public object EMPLOYMENTSTART { get; set; }
            [JsonProperty("MARITAL.STATUS")]
            public string MARITALSTATUS { get; set; }
            [JsonProperty("NO.OF.DEPENDENTS")]
            public object NOOFDEPENDENTS { get; set; }
            [JsonProperty("RESIDENCE.TYPE")]
            public object RESIDENCETYPE { get; set; }
            public string COUNTRY { get; set; }
            [JsonProperty("TOWN.COUNTRY")]
            public string TOWNCOUNTRY { get; set; }
            [JsonProperty("STATE.RESIDENCE")]
            public string STATERESIDENCE { get; set; }
            public string ADDRESS { get; set; }
            public string STREET { get; set; }
            [JsonProperty("RESIDENCE.SINCE")]
            public object RESIDENCESINCE { get; set; }
            [JsonProperty("ADDR.LANDMARK")]
            public List<string> ADDRLANDMARK { get; set; }
            public object ROLE { get; set; }
            [JsonProperty("SHORT.NAME")]
            public string SHORTNAME { get; set; }
            [JsonProperty("PHONE.1")]
            public string PHONE1 { get; set; }
            [JsonProperty("SMS.1")]
            public string SMS1 { get; set; }
            [JsonProperty("EMAIL.1")]
            public string EMAIL1 { get; set; }
            [JsonProperty("NAME.1")]
            public string NAME1 { get; set; }
            [JsonProperty("LEGAL.ID.DOC.NAME")]
            public string LEGALIDDOCNAME { get; set; }
            public string BVN { get; set; }
            [JsonProperty("TAX.ID")]
            public object TAXID { get; set; }
            [JsonProperty("CUSTOMER.STATUS")]
            public string CUSTOMERSTATUS { get; set; }
            [JsonProperty("RC.NUMBER")]
            public object RCNUMBER { get; set; }
            [JsonProperty("MOTHER.NAME")]
            public string MOTHERNAME { get; set; }
            public string RESIDENCE { get; set; }
            [JsonProperty("RESIDENCE.NAME")]
            public string RESIDENCENAME { get; set; }
            [JsonProperty("LEGAL.ISS.AUTH")]
            public string LEGALISSAUTH { get; set; }
            [JsonProperty("LEGAL.ISS.DATE")]
            public string LEGALISSDATE { get; set; }
            [JsonProperty("LEGAL.EXP.DATE")]
            public string LEGALEXPDATE { get; set; }
            [JsonProperty("ACCT.OFFICER.NAME")]
            public string ACCTOFFICERNAME { get; set; }
            [JsonProperty("ACCT.OFFICER.BR")]
            public string ACCTOFFICERBR { get; set; }
            [JsonProperty("ACCT.OFFICER.PHONE")]
            public object ACCTOFFICERPHONE { get; set; }
            [JsonProperty("POST.CODE")]
            public object POSTCODE { get; set; }
            [JsonProperty("OFF.PHONE")]
            public string OFFPHONE { get; set; }
            public object FAX { get; set; }
            [JsonProperty("KIN.NAME")]
            public string KINNAME { get; set; }
            [JsonProperty("KIN.EMAIL")]
            public object KINEMAIL { get; set; }
            [JsonProperty("KIN.PHONE.NO")]
            public string KINPHONENO { get; set; }
            [JsonProperty("NATURE.BUSS")]
            public object NATUREBUSS { get; set; }
            [JsonProperty("EMPLOYER.PHONE")]
            public object EMPLOYERPHONE { get; set; }
            [JsonProperty("EMPLOYERS.ADD")]
            public object EMPLOYERSADD { get; set; }
            [JsonProperty("JOB.TITLE")]
            public object JOBTITLE { get; set; }
        }

        public class CustomerServiceModel 
        {
            public CustomerInfo Record { get; set; }
        }
    }
}
