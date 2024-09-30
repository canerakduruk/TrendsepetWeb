using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trendsepet.Models
{
    public class ParentCategoryModel
    {
        public string Id {  get; set; }
        public string Name { get; set; }
        public List<ChildCategoryModel> ChildCategories { get; set; }
    }
}