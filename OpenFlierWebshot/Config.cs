using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFlierWebshot
{
    public class Config
    {
        public string? BrowserExecutablePath { get; set; }
        public List<JavaScriptMatchEntry>? JavaScriptMatchEntries { get; set; }
    }

    public class JavaScriptMatchEntry
    {
        public string? DomainFragment { get; set; }
        public string? Script { get; set; }
    }
}
