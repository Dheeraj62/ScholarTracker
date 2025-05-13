using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ScholarTracker.Models
{
    public class Award
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Select a scholar")]
        public int ScholarId { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Award Title")]
        public string AwardTitle { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Awarding Body / Organization")]
        public string AwardingBody { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Award")]
        public DateTime DateOfAward { get; set; }

        [Required]
        [Display(Name = "Award Type / Level")]
        public string AwardType { get; set; }

        [Display(Name = "Certificate / Document")]
        public string CertificatePath { get; set; }

        // recorded for audit
        public int FkUserId { get; set; }
    }
}