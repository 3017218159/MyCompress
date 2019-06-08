using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress
{
    class HuffmanTreeInfo
    {
        public string ZipCode { get; set; }
        public string ZipCodeRemainder { get; set; }
        public Dictionary<string, char> UnZipDictionary { get; set; }

    }
}
