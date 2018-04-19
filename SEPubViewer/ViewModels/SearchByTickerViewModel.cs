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

        private List<TopLevelFiling> filings;

        public List<TopLevelFiling> Filings
        {
            get { return filings; }
            set
            {
                filings = value;
                OnPropertyChanged("Filings");
            }
        }

        private TopLevelFiling selectedFiling;

        public TopLevelFiling SelectedFiling
        {
            get { return selectedFiling; }
            set
            {
                selectedFiling = value;
                OnPropertyChanged("SelectedFiling");
            }
        }

        private SECSingleFileLink selectedDoc;

        public SECSingleFileLink SelectedDoc
        {
            get { return selectedDoc; }
            set
            {
                selectedDoc = value;
                OnPropertyChanged("SelectedDoc");
            }
        }

        private List<SECSingleFileLink> docLinks;

        public List<SECSingleFileLink> DocLinks
        {
            get { return docLinks; }
            set
            {
                docLinks = value;
                OnPropertyChanged("DocLinks");
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
                QueryBuilder query = new QueryBuilder();
                query.AddQuery("action", "getcompany");
                query.AddQuery("CIK", Ticker);

                var page = edgarRetrieval.GetTickerLandingPage(query);
                Filings = page.Filings;
            });
        }

        public async void GetDocsInReport()
        {
            await Task.Factory.StartNew(() =>
            {
                var dets = edgarRetrieval.GetSubmissionDetails(SelectedFiling);
                DocLinks = dets.HTMLLinks;
                if (DocLinks.Count > 0)
                {
                    var maxVal = dets.HTMLLinks.Max(l => l.Size);
                    SelectedDoc = dets.HTMLLinks.Where(l => l.Size == maxVal).FirstOrDefault();
                }
            });
        }
    }
}
