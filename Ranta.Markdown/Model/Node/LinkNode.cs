using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranta.Markdown.Model
{
    internal class LinkNode : NodeBase
    {
        public string Text { get; set; }

        public string Href { get; set; }
    }
}
