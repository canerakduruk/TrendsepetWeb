using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trendsepet.Models
{
    [Serializable]
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public string Count { get; set; }
    }
}