using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranta.Markdown.Model
{
    internal class OL : NodeBase
    {
        public List<ListItem> ItemList { get; set; }
    }
}
