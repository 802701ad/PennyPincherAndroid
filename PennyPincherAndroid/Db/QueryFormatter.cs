using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


    public class QueryFormatter
    {
        private Dictionary<string, object> _params = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        public object this[string index]
        {
            get
            {
                return _params[index];
            }

            set
            {
                _params[index]=value;
            }
        }

        public string src = "";

        public string Format()
        {
            string result=src;
            foreach (string param in _params.Keys)
            {
                result=Regex.Replace(result, "#" + param + "#", Convert.ToString(_params[param]).Replace("'", "''"), RegexOptions.IgnoreCase);
            }
            return result;
        }
    }
