using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace ScholarTracker.Models
{
	public class Publication
	{
		public int Id { get; set; }

		public int FkUserId { get; set; }

		[Required(ErrorMessage = "Journal Index is required")]
		public string JournalIndex { get; set; }

		[Required(ErrorMessage = "Journal Name is required")]
		public string JournalName { get; set; }

		[Required(ErrorMessage = "Title is required")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Publisher is required")]
		public string Publisher { get; set; }

		[Required(ErrorMessage = "Impact Factor is required")]
		public decimal ImpactFactor { get; set; }

		[Required(ErrorMessage = "Date of Issue is required")]
		[DataType(DataType.Date)]
		public DateTime DateOfIssue { get; set; }

		[Required(ErrorMessage = "Volume is required")]
		public int Volume { get; set; }

		[Required(ErrorMessage = "Issue is required")]
		public int Issue { get; set; }

		[Required(ErrorMessage = "DOI is required")]
		public string DOI { get; set; }

		[Required(ErrorMessage = "Page numbers are required")]
		public string PageNo { get; set; }

		public string FilePath { get; set; }

		[NotMapped]
		public HttpPostedFileBase PublicationFile { get; set; }
	}
}
