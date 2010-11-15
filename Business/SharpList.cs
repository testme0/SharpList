using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business
{
    public class SharpList
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<SharpItem> Items { get; set; }

        public SharpList(string name)
        {
            Name = name;
            Date = DateTime.Now;
            Items = new List<SharpItem>();
        }
    }
}
