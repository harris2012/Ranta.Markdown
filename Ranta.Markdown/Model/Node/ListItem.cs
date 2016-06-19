using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranta.Markdown.Model
{
    internal class ListItem : NodeBase
    {
        public IEnumerable<ItemBase> ItemList { get; set; }
    }
}
