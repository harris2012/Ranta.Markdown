using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranta.Markdown.Model
{
    internal class Headline : NodeBase
    {
        public string Text { get; set; }

        public int Level { get; set; }
    }
}
