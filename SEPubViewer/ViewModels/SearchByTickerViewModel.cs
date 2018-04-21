﻿using Newtonsoft.Json;
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

        private IEdgarRetrieval edgarRetrieval;
        
        #endregion

        public SearchByTickerViewModel() : this (DIResolver.ResolveEdgar())
        { }

        public SearchByTickerViewModel(IEdgarRetrieval retrieval)
        {
            RecentTickers = new List<string>();
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
            }).ConfigureAwait(false);
            OnTickerRetrieval();
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

    }
}
