using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCommunication
{
    public class QueryBuilder
    {
        public const string SECBaseRoute = "https://www.sec.gov/cgi-bin/browse-edgar";

        public string HTTPGetRequest { get
            {
                return BaseRoute + RequestString;
            }
        }

        public string RequestString { get
            {
                return ConstructReqString();
            } }

        private string BaseRoute;
        private Dictionary<string, string> KeyValuePairs = new Dictionary<string, string>();

        public QueryBuilder(string baseRoute = SECBaseRoute)
        {
            BaseRoute = baseRoute;
        }

        public void AddQuery(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            if (KeyValuePairs.ContainsKey(key))
                return;

            KeyValuePairs.Add(key, value);
        }

        public void RemoveQueryByKey(string key)
        {
            KeyValuePairs.Remove(key);
        }

        private string ConstructReqString()
        {
            if (KeyValuePairs.Count > 0)
            {
                string req = "?";
                int count = 0;
                foreach (var pair in KeyValuePairs)
                {
                    req += $"{pair.Key}={pair.Value}";
                    if (++count == KeyValuePairs.Count)
                        return req;
                    else
                        req += "&";
                }
                return req;
            }
            else
                return "";
        }

    }
}
