using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EbayProject.Helping_Classes
{
    public class CardDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public string Player { get; set; }
        public string Set { get; set; }
        public string Variation { get; set; }
        public string Grade { get; set; }
        public string SalePrice { get; set; }
        public String CardDate { get; set; }
        public string Link { get; set; }
        public string Charts { get; set; }
        public string ImgPath { get; set; }
    }
}