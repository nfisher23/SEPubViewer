using Newtonsoft.Json;
using SECCommunication.Interfaces;
using SECCommunication.Models;
using SEPubViewer.Infrastructure;
using SEPubViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SEPubViewer.ViewModels
{
    public class SearchByTickerViewModel : Notifier
    {
        #region Exposed Properties
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
                if (value == null)
                    return; // weird, currently inexplainable bug causes it to send null on every other selection

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

        // ick
        private string[] recentTickers;
        public List<string> RecentTickers
        {
            get { return recentTickers.ToList(); }
            set
            {
                recentTickers = value.ToArray();
                OnPropertyChanged("RecentTickers");
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }


        #endregion

        private IEdgarRetrieval edgarRetrieval;
        private TickerLandingPage LastPage;

        public SearchByTickerViewModel() : this (DIResolver.ResolveEdgar())
        { }

        public SearchByTickerViewModel(IEdgarRetrieval retrieval)
        {
            RecentTickers = new List<string>();
            edgarRetrieval = retrieval;
        }

        public async void GetFilings()
        {
            ErrorMessage = "";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    QueryBuilder query = new QueryBuilder();
                    query.AddQuery("action", "getcompany");
                    query.AddQuery("CIK", Ticker);

                    var page = edgarRetrieval.GetTickerLandingPage(query);
                    Filings = page.Filings;

                    LastPage = page;
                    OnTickerRetrieval();
                }
                catch (Exception e)
                {
                    ErrorMessage = $"Error: Could not find {Ticker}";
                }
            }).ConfigureAwait(false);
        }

        public async void LoadMoreFilings()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    QueryBuilder query = new QueryBuilder();
                    query.AddQuery("action", "getcompany");
                    query.AddQuery("CIK", Ticker);
                    query.AddQuery("dateb", this.LastPage.LastDateOnPage.ToString("yyyyMMdd"));

                    var page = edgarRetrieval.GetTickerLandingPage(query);
                    AddToFilings(page);

                    LastPage = page;
                }
                catch (Exception e)
                {
                    ErrorMessage = "TBD on load more failure";
                }
            });
        }

        public async void GetDocsInReport()
        {
            await Task.Factory.StartNew(() =>
            {
                if (selectedFiling == null)
                    return;

                var dets = edgarRetrieval.GetSubmissionDetails(SelectedFiling);
                DocLinks = dets.HTMLLinks;
                if (DocLinks.Count > 0)
                {
                    var maxVal = dets.HTMLLinks.Max(l => l.Size);
                    SelectedDoc = dets.HTMLLinks.Where(l => l.Size == maxVal).FirstOrDefault();
                }
                
            }).ConfigureAwait(false);
        }

        private void OnTickerRetrieval()
        {
            // todo: implement a collectionchanged interface so this isn't so ugly
            var l = recentTickers.Take(10).ToList();
            if (!l.Contains(Ticker))
            {
                l.Insert(0,Ticker);
            }
            else
            {
                l.Remove(Ticker);
                l.Insert(0, Ticker);
            }
            RecentTickers = l.ToList();
        }

        private void AddToFilings(TickerLandingPage page)
        {
            // still ugly, but we'll avoid the boilerplate code and take on some technical debt 
            // for a small project like this.
            var newF = new List<TopLevelFiling>();
            newF.AddRange(Filings);
            newF.AddRange(page.Filings);
            Filings = newF;
        }



    }
}
