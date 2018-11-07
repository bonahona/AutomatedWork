using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedWork
{
    public class AssetConfig
    {
        public Dictionary<String, Asset> Assets { get; set; }
    }

    public class Asset
    {
        public String File { get; set; }
        public String Tool { get; set; }
        public String Target { get; set; }
    }
}
