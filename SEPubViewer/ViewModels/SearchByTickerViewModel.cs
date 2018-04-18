using Newtonsoft.Json;
using SECCommunication.Interfaces;
using SECCommunication.Models;
using SEPubViewer.Infrastructure;
using SEPubViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SEPubViewer.ViewModels
{
    public class SearchByTickerViewModel : Notifier
    {
        #region Properties
        private string ticker;

        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                OnPropertyChanged("Ticker");
            }
        }

        private List<TopLevelFilingWithTitle> filings;

        public List<TopLevelFilingWithTitle> Filings
        {
            get { return filings; }
            set
            {
                filings = value;
                OnPropertyChanged("Filings");
            }
        }

        private IEdgarRetrieval edgarRetrieval;
        

        #endregion

        public SearchByTickerViewModel() : this (DIResolver.ResolveEdgar())
        { }

        public SearchByTickerViewModel(IEdgarRetrieval retrieval)
        {
            edgarRetrieval = retrieval;
        }

        public async void GetFilings()
        {
            await Task.Factory.StartNew(() =>
            {
                Filings = new List<TopLevelFilingWithTitle>();
                QueryBuilder query = new QueryBuilder();
                query.AddQuery("action", "getcompany");
                query.AddQuery("CIK", Ticker);

                var page = edgarRetrieval.GetTickerLandingPage(query);
                foreach (var f in page.Filings)
                    Filings.Add(GetChild(f));

            });
        }

        ///<summary>Convert TopLevelFiling into child element</summary>
        private TopLevelFilingWithTitle GetChild(TopLevelFiling filing)
        {
            var parent = JsonConvert.SerializeObject(filing);
            return JsonConvert.DeserializeObject<TopLevelFilingWithTitle>(parent);
        }
    }
}
