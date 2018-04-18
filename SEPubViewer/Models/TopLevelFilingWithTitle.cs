using SECCommunication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEPubViewer.Models
{
    public class TopLevelFilingWithTitle : TopLevelFiling
    {
        public string DisplayTitle
        {
            get
            {
                return $"({this.FilingDate.ToShortDateString()}) - {this.FilingName}";
            }
        }

        public TopLevelFilingWithTitle() : base ()
        { }
    }
}
