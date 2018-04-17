using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Models
{
    public class SECFilingDetails
    {
        public IEnumerable<SECSingleFileLink> AllLinks { get; set; }
        public DateTime TimeAccepted { get; set; }
        public int NumberOfDocuments { get; set; }
    }
}
