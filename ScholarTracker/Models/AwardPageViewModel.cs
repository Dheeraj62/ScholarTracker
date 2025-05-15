using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScholarTracker.Models
{
    public class AwardPageViewModel
    {
        public Award Award { get; set; }
        public List<Award> Awards { get; set; } = new List<Award>();
    public List<SelectListItem> Scholars { get; set; } = new List<SelectListItem>();
    }
}