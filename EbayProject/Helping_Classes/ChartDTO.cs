using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbayProject.Helping_Classes
{
    public class ChartDTO
    {
        
        public int Id { get; set; }
        public string date { get; set; }
        public double price { get; set; }
        public double average { get; set; }

        public int Id2 { get; set; }
        public double price2 { get; set; }
        public double average2 { get; set; }

        public int Id3 { get; set; }
        public double price3 { get; set; }
        public double average3 { get; set; }
    }
}