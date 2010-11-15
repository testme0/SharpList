using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpList
{
    public class Item
    {
        public string Name { get; set; }
        public bool Checked { get; set; }

        public Item(string name)
        {
            Name = name;
            Checked = false;
        }
    }
}
