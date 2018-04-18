using SECCommunication.Implementations;
using SECCommunication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEPubViewer.Infrastructure
{
    // Without using a predesigned mvvm framework, 
    // I'll use this hack to handle dependency injection
    // throughout the app.

    // I don't really like it, though. It is better than no DI at all so I'm keeping it for now
    static class DIResolver
    {
        public static IEdgarRetrieval ResolveEdgar()
        {
            return new HAPEdgarRetrieval();
        }
    }
}
