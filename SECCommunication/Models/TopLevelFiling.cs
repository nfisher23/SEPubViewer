using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Models
{
    public class TopLevelFiling
    {
        public string FilingName { get; set; }
        public Uri LinkToDocs { get; set; }
        public string Description { get; set; }
        public DateTime FilingDate { get; set; }

        public string FileNumber { get; set; }
        public Int64 FilmNumber { get; set; }
    }
}
