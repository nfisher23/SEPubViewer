using SECCommunication.Interfaces;
using SECCommunication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SECCommunication.Implementations
{
    public class HAPEdgarRetrieval : IEdgarRetrieval
    {
        public HtmlWeb Web { get; private set; } = new HtmlWeb();

        const string SECBaseLink = "http://www.sec.gov";

        public TickerLandingPage GetTickerLandingPage(QueryBuilder query)
        {
            TickerLandingPage page = new TickerLandingPage { Query = query };
            var doc = Web.Load(query.HTTPGetRequest);
            page.Filings = GetFilingsFromLandingPage(doc);
            page.LastDateOnPage = page.Filings.Last().FilingDate;

            return page;
        }

        public SECFilingDetails GetSubmissionDetails(TopLevelFiling filing)
        {
            SECFilingDetails dets = new SECFilingDetails();
            var doc = Web.Load(filing.LinkToDocs.AbsoluteUri);
            var dtAccepted = TryGetDateTimeAccepted(doc);
            if (dtAccepted.HasValue)
                dets.TimeAccepted = dtAccepted.Value;

            dets.NumberOfDocuments = TryGetNumberOfDocuments(doc);
            dets.AllLinks = GetFilingDetailsLinks(doc);

            return dets;
        }

        private List<TopLevelFiling> GetFilingsFromLandingPage(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//tr");
            List<TopLevelFiling> filings = new List<TopLevelFiling>();
            foreach (var row in rows)
            {
                var data = row.SelectNodes("td");
                if (data == null || data.Count < 2)
                    continue;

                TopLevelFiling filing = null;


                try {
                    filing = new TopLevelFiling
                    {
                        FilingName = data[0].InnerText,
                        LinkToDocs = new Uri(SECBaseLink + data[1].FirstChild.Attributes["href"].Value),
                    };
                } catch { }

                try {
                    filing.Description = data[2].InnerText;
                    filing.FilingDate = DateTime.ParseExact(data[3].InnerText, "yyyy-MM-dd", null);
                } catch { }

                try
                {
                    filing.FileNumber = data[4].FirstChild.InnerText;
                    long val = 0;
                    Int64.TryParse(data[4].ChildNodes.Last().InnerText, out val);
                    filing.FilmNumber = val;
                } catch { }

                if (filing != null)
                    filings.Add(filing);
            }
            return filings;
        }

        private DateTime? TryGetDateTimeAccepted(HtmlDocument doc)
        {
            DateTime? dt = null;
            try
            {
                dt = Convert.ToDateTime(doc.DocumentNode
                    .SelectSingleNode("/html/body/div[4]/div[1]/div[2]/div[1]/div[4]").InnerText);
            }
            catch { }

            return dt;
        }

        ///<summary>Returns zero on failure</summary>
        private int TryGetNumberOfDocuments(HtmlDocument doc)
        {
            int numDocs = 0;
            try
            {
                numDocs = Convert.ToInt32(doc.DocumentNode
                    .SelectSingleNode("/html/body/div[4]/div[1]/div[2]/div[1]/div[6]").InnerText);
            } catch { }

            return numDocs;
        }

        private List<SECSingleFileLink> GetFilingDetailsLinks(HtmlDocument doc)
        {
            var tables = doc.DocumentNode.Descendants("table");
            List<SECSingleFileLink> links = new List<SECSingleFileLink>();
            foreach (var table in tables)
            {
                var rows = table.Descendants("tr");
                foreach (var row in rows)
                {
                    SECSingleFileLink link = null;
                    var data = row.Descendants("td").ToList();
                    try
                    {
                        int seq = 0;
                        Int32.TryParse(data[0].InnerText, out seq);
                        link = new SECSingleFileLink
                        {
                            Seq = seq,
                            Description = data[1].InnerText,
                            DocumentTitle = data[2].InnerText,
                            FileLink = new Uri(SECBaseLink + data[2].FirstChild.Attributes["href"].Value)
                        };
                    } catch (Exception e)
                    {
                        // for debugging
                    }
                    try
                    {
                        link.FileType = data[3].InnerText;
                        link.Size = Convert.ToInt32(string.IsNullOrEmpty(data[4].InnerText) ? "0" : data[4].InnerText);
                    }
                    catch { }
                    if (link != null)
                        links.Add(link);
                }
            }
            return links;
        }
    }
}
