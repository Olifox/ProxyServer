using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class SitesPool
    {
        private List<string> Address;

        public SitesPool(string Path)
        {
            Address = new List<string>();
            var f = new StreamReader(Path);
            while (!f.EndOfStream)
                Address.Add(f.ReadLine());
        }

        public bool Validate(string uri)
        {
            foreach(var s in Address)
                if (uri.Contains(s))
                    return false;
            return true;
        }
    }
}
