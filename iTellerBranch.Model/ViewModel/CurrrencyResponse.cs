using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Model.ViewModel
{
    public class CurrrencyResponse
    {
        public class Denomination
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public DateTime DateCreated { get; set; }
        }

        public class Datum
        {
            public int ID { get; set; }
            public string Currency { get; set; }
            public string SubunitName { get; set; }
            public int RelationshipUnit { get; set; }
            public string Abbrev { get; set; }
            public List<Denomination> Denomination { get; set; }
        }

        public class Root
        {
            public bool success { get; set; }
            public string message { get; set; }
            public object innerError { get; set; }
            public List<Datum> data { get; set; }
        }
    }
}
