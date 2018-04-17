using NUnit.Framework;
using SECCommunication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication.UnitTests.ModelsTests
{
    [TestFixture]
    public class QueryBuilderTests
    {
        [Test]
        public void AddQuery_AddsOne()
        {
            QueryBuilder q = new QueryBuilder();

            q.AddQuery("action", "getcompany");

            Assert.AreEqual(q.HTTPGetRequest, QueryBuilder.SECBaseRoute + "?action=getcompany");
        }

        [Test]
        public void AddQuery_AddsThree()
        {
            QueryBuilder q = new QueryBuilder();

            q.AddQuery("action", "getcompany");
            q.AddQuery("otheraction", "getmorecompanies");
            q.AddQuery("third", "three");

            Assert.AreEqual(q.HTTPGetRequest, 
                QueryBuilder.SECBaseRoute + "?action=getcompany&otheraction=getmorecompanies" +
                "&third=three");
        }


        [Test]
        public void RemoveQuery_RemovesOne()
        {
            QueryBuilder q = new QueryBuilder();
            q.AddQuery("action", "getcompany");

            q.RemoveQueryByKey("action");

            Assert.AreEqual(QueryBuilder.SECBaseRoute, q.HTTPGetRequest);
        }

        [Test]
        public void RemoveQuery_RemovesSome()
        {
            QueryBuilder q = new QueryBuilder();
            q.AddQuery("action", "getcompany");
            q.AddQuery("otheraction", "getmorecompanies");
            q.AddQuery("third", "three");

            q.RemoveQueryByKey("action");
            q.RemoveQueryByKey("third");

            Assert.AreEqual(q.HTTPGetRequest,
                QueryBuilder.SECBaseRoute + "?otheraction=getmorecompanies");
        }


    }
}
