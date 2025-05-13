using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScholarTracker.Models
{
    public class Objectives
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a scholar.")]
        [Display(Name = "Scholar")]
        public int ScholarId { get; set; }

        [Required(ErrorMessage = "Objective 1 is required.")]
        [Range(0, 100, ErrorMessage = "Enter a valid number.")]
        [Display(Name = "Objective 1")]
        public int Objective1 { get; set; }

        [Required(ErrorMessage = "Objective 2 is required.")]
        [Range(0, 100, ErrorMessage = "Enter a valid number.")]
        [Display(Name = "Objective 2")]
        public int Objective2 { get; set; }

        [Required(ErrorMessage = "Objective 3 is required.")]
        [Range(0, 100, ErrorMessage = "Enter a valid number.")]
        [Display(Name = "Objective 3")]
        public int Objective3 { get; set; }

        [Required(ErrorMessage = "Objective 4 is required.")]
        [Range(0, 100, ErrorMessage = "Enter a valid number.")]
        [Display(Name = "Objective 4")]
        public int Objective4 { get; set; }

        /// <summary>
        /// Populated from Session["UserId"] in the controller; not shown in the form.
        /// </summary>
        public int FkUserId { get; set; }
    }
}