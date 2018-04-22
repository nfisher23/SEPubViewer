using Newtonsoft.Json;
using SECCommunication.Interfaces;
using SECCommunication.Models;
using SEPubViewer.Infrastructure;
using SEPubViewer.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
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

        private string systemMessage;
        public string SystemMessage
        {
            get { return systemMessage; }
            set
            {
                systemMessage = value;
                OnPropertyChanged("SystemMessage");
            }
        }

        private QueryViewModel queryVM;
        public QueryViewModel QueryVM
        {
            get { return queryVM; }
            set
            {
                queryVM = value;
                OnPropertyChanged("QueryVM");
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
            queryVM = new QueryViewModel();
            edgarRetrieval = retrieval;
        }

        public async void GetFilings()
        {
            ErrorMessage = "";
            SystemMessage = "Working...";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var query = BuildBaseQuery();

                    var page = edgarRetrieval.GetTickerLandingPage(query);
                    Filings = page.Filings;

                    LastPage = page;
                    OnTickerRetrieval();
                    SystemMessage = "Retrieved!";
                }
                catch (Exception e)
                {
                    ErrorMessage = $"Error: Could not find {Ticker}";
                    SystemMessage = "";
                }
            }).ConfigureAwait(false);
        }

        public async void LoadMoreFilings()
        {
            SystemMessage = "Loading...";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var query = BuildBaseQuery();
                    query.RemoveQueryByKey("dateb");
                    query.AddQuery("dateb", this.LastPage.LastDateOnPage.AddDays(-1).ToString("yyyyMMdd"));

                    var page = edgarRetrieval.GetTickerLandingPage(query);
                    AddToFilings(page);

                    LastPage = page;
                    SystemMessage = "Loaded!";
                }
                catch (Exception e)
                {
                    ErrorMessage = "Failed to load more documents";
                    SystemMessage = "";
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

        public async void DownloadDocument()
        {
            SystemMessage = "Downloading...";

            await Task.Factory.StartNew(() =>
            {
                KnownFolder k = new KnownFolder(KnownFolderType.Downloads);
                DownloadAndSave(SelectedDoc, k.Path);
                SystemMessage = "Downloaded!";
            });
        }

        public async void DownloadAllDocuments()
        {
            SystemMessage = "Downloading...";

            await Task.Factory.StartNew(() =>
            {
                KnownFolder k = new KnownFolder(KnownFolderType.Downloads);
                string pathToDir = $"{k.Path}\\{Ticker}_({SelectedFiling.FilingDate.Month}-" +
                    $"{SelectedFiling.FilingDate.Day})_{SelectedFiling.FilingName}";
                Directory.CreateDirectory(pathToDir);

                foreach (var page in DocLinks)
                {
                    DownloadAndSave(page, pathToDir);
                }

                SystemMessage = "Downloaded!";
            });
        }

        private void DownloadAndSave(SECSingleFileLink link, string basePath)
        {
            var html = new System.Net.WebClient().DownloadString(link.FileLink);
            var description = string.IsNullOrEmpty(link.Description) ? link.DocumentTitle : link.Description;
            string title = Ticker.ToUpper() + "_" + description + ".html";
            File.WriteAllText(basePath + "\\" + title, html);
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

        private QueryBuilder BuildBaseQuery()
        {
            QueryBuilder query = new QueryBuilder();
            query.AddQuery("action", "getcompany");
            query.AddQuery("CIK", Ticker);
            if (!string.IsNullOrEmpty(QueryVM.FilingType))
                query.AddQuery("type", queryVM.FilingType);

            if (QueryVM.DateBefore > new DateTime())
                query.AddQuery("dateb", QueryVM.DateBefore.ToString("yyyyMMdd"));

            return query;
        }

    }

    public class QueryViewModel : Notifier
    {
        private string filingType;
        public string FilingType
        {
            get { return filingType; }
            set
            {
                filingType = value;
                OnPropertyChanged("FilingType");
            }
        }

        private DateTime dateBefore = DateTime.Now;
        public DateTime DateBefore
        {
            get { return dateBefore; }
            set
            {
                dateBefore = value;
                OnPropertyChanged("DateBefore");
            }
        }
    }
}
