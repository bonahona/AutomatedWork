using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedWork
{
    public class ToolsConfig
    {
        public Dictionary<String, ExportTools> ExportTools { get; set; }
    }

    public class ExportTools
    {
        public String Command { get; set; }
        public String TargetFileEnding { get; set; }
    }
}
