using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScholarTracker.Models
{
		public class Supervisor
		{
			public int Id { get; set; }

			[Display(Name = "Info. as per records")]
			public string InfoAsPerRecords { get; set; }

			[Display(Name = "Adhaar No")]
			[StringLength(12, MinimumLength = 12, ErrorMessage = "Adhaar No must be 12 digits")]
			public string AdhaarNo { get; set; }

			[Display(Name = "Pancard No")]
			[StringLength(10, MinimumLength = 10, ErrorMessage = "Pancard No must be 10 characters")]
			public string PancardNo { get; set; }

			public string Title { get; set; }

			[Display(Name = "First Name")]
			[Required(ErrorMessage = "First Name is required")]
			public string FirstName { get; set; }

			[Display(Name = "Middle Name")]
			public string MiddleName { get; set; }

			[Display(Name = "Last Name")]
			[Required(ErrorMessage = "Last Name is required")]
			public string LastName { get; set; }

			public string Designation { get; set; }

			public string Country { get; set; }

			public string State { get; set; }

			[Display(Name = "City/District")]
			public string CityDistrict { get; set; }

			public string Address { get; set; }
		}


}