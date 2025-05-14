using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScholarTracker.Models
{
	public class StudentModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Full Name is required")]
		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Father Name is required")]
		[Display(Name = "Father Name")]
		public string FatherName { get; set; }

		[Required(ErrorMessage = "Mother Name is required")]
		[Display(Name = "Mother Name")]
		public string MotherName { get; set; }

		[Required(ErrorMessage = "Admission Date is required")]
		[Display(Name = "Admission Date")]
		[DataType(DataType.Date)]
		public DateTime AdmissionDate { get; set; }

		[Required(ErrorMessage = "Enrollment No is required")]
		[Display(Name = "Enrollment No")]
		public string EnrollmentNo { get; set; }
	}
}