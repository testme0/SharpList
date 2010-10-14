using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    public class SharpItem
    {
        public string Name { get; set; }
        public bool Checked { get; set; }

        public SharpItem(string name)
        {
            Name = name;
            Checked = false;
        }
    }
}
