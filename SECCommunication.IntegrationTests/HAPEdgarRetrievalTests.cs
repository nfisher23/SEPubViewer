using NUnit.Framework;
using SECCommunication.Implementations;
using SECCommunication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.IntegrationTests
{
    [TestFixture]
    public class HAPEdgarRetrievalTests
    {
        HAPEdgarRetrieval edgarRetrieval;
        TickerLandingPage LandingPage;
        QueryBuilder query;

        [SetUp]
        public void SetUp()
        {
            edgarRetrieval = new HAPEdgarRetrieval();
            query = new QueryBuilder();
            query.AddQuery("action", "getcompany");
            query.AddQuery("CIK", "aapl");
            query.AddQuery("dateb", "20180417");
            query.AddQuery("owner", "exclude");
            query.AddQuery("count", "40");

            LandingPage = edgarRetrieval.GetTickerLandingPage(query);

        }

        // these tests take longer, so it's in our interest to make them 
        // go faster rather than make them easier to dissect.

        // Note that these tests are confirming what an external source is providing us with.
        // As such, truly random failings would most likely mean the external resource has changed,
        // and probably isn't a bug in the code (in that case)
        [Test]
        public void CollectionOfIntegrationTests()
        {
            EnsureFilingsExist();
            EnsureSublinksWorkOut();
        }

        [TearDown]
        public void TearDown()
        {

        }

        private void EnsureFilingsExist()
        {
            Assert.AreEqual(LandingPage.Filings.Count(), 40);
            Assert.AreNotEqual(LandingPage.LastDateOnPage, new DateTime());
            foreach (var filing in LandingPage.Filings)
            {
                Assert.IsNotNull(filing.Description);
                Assert.AreNotEqual(filing.FileNumber, 0);
                Assert.AreNotEqual(filing.FilingDate, new DateTime());
                Assert.AreNotEqual(filing.FilmNumber, 0);
                Assert.IsNotNull(filing.LinkToDocs);
            }
        }

        private void EnsureSublinksWorkOut()
        {
            var firstSubPages = edgarRetrieval.GetSubmissionDetails(LandingPage.Filings.First());

            Assert.IsNotNull(firstSubPages);
            Assert.AreEqual(firstSubPages.AllLinks.Count(), 7);
            Assert.AreEqual(firstSubPages.NumberOfDocuments, 6); // set by page, not parsed
            Assert.AreEqual(firstSubPages.TimeAccepted.Date, new DateTime(2018, 3, 7));
        }
    }
}
