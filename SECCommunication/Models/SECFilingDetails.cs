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
                return AllLinks.Where(l => l.FileLink.AbsoluteUri.EndsWith(".htm")
                    || l.FileLink.AbsoluteUri.EndsWith(".html")
                    || l.FileLink.AbsoluteUri.EndsWith(".txt")).ToList();
            }
        }
        public DateTime TimeAccepted { get; set; }
        public int NumberOfDocuments { get; set; }
    }
}
