using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpList
{
    public class List
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<Item> Items { get; set; }

        public List(string name)
        {
            Name = name;
            Date = DateTime.Now;
            Items = new List<Item>();
        }
    }
}
