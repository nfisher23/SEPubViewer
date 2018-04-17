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
        public HtmlWeb Web { get; set; } = new HtmlWeb();

        private string SECBaseLink = "http://www.sec.gov";


        public TickerLandingPage GetTickerLandingPage(QueryBuilder query)
        {
            TickerLandingPage page = new TickerLandingPage { Query = query };
            var doc = Web.Load(query.HTTPGetRequest);
            page.Filings = GetFilingsFromLandingPage(doc);
            page.LastDateOnPage = page.Filings.Last().FilingDate;

            return page;
        }

        private IEnumerable<TopLevelFiling> GetFilingsFromLandingPage(HtmlDocument doc)
        {
            var rows = doc.DocumentNode.SelectNodes("//tr");
            foreach (var row in rows)
            {
                var data = row.SelectNodes("td");
                TopLevelFiling filing = null;
                try
                {
                    filing = new TopLevelFiling
                    {
                        FilingName = data[0].InnerText,
                        LinkToDocs = new Uri(SECBaseLink + data[1].FirstChild.Attributes["href"].Value),
                        Description = data[2].InnerText,
                        FilingDate = DateTime.ParseExact(data[3].InnerText, "yyyy-MM-dd", null),
                        FileNumber = data[4].FirstChild.InnerText,
                        FilmNumber = Convert.ToInt64(data[4].ChildNodes.Last().InnerText)
                    };
                }
                catch { }
                if (filing != null)
                    yield return filing;
            }
        }
    }
}
