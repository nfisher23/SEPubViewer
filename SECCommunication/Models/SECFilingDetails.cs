using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Models
{
    public class SECFilingDetails
    {
        public List<SECSingleFileLink> AllLinks { get; set; }
        public List<SECSingleFileLink> HTMLLinks {
            get {
                return AllLinks.Where(l => l.DocumentTitle.EndsWith(".htm")
                    || l.DocumentTitle.EndsWith(".html")).ToList();
            }
        }
        public DateTime TimeAccepted { get; set; }
        public int NumberOfDocuments { get; set; }
    }
}
