using SECCommunication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.Interfaces
{
    public interface IEdgarRetrieval
    {
        TickerLandingPage GetTickerLandingPage(QueryBuilder query);
    }
}
