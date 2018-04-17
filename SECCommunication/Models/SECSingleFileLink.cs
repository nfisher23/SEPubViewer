using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Models
{
    public class SECSingleFileLink
    {
        public int Seq { get; set; }
        public string Description { get; set; }
        public string DocumentTitle { get; set; }
        public Uri FileLink { get; set; }
        public string FileType { get; set; }
        public Int64 Size { get; set; }
    }
}
