using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Models
{
    public class TickerLandingPage
    {
        public IEnumerable<TopLevelFiling> Filings { get; set; }
        public QueryBuilder Query { get; set; }

        public DateTime LastDateOnPage { get; set; }
    }
}
