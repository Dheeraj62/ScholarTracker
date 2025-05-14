using System;
using System.ComponentModel.DataAnnotations;

namespace ScholarTracker.Models
{
	public class RDCMinutesModel
	{
		public int Id { get; set; }

		[Required, Display(Name = "Scholar")]
		public int FkScholarId { get; set; }

		public string ScholarName { get; set; }

		[Required, Display(Name = "Supervisor")]
		public int FkSupervisorId { get; set; }

		public string SupervisorName { get; set; }

		[Required, DataType(DataType.Date), Display(Name = "Meeting Date")]
		public DateTime MeetingDate { get; set; }

		[Display(Name = "Agenda")]
		public string MeetingAgenda { get; set; }

		[Display(Name = "Discussion Points")]
		public string DiscussionPoints { get; set; }

		[Display(Name = "Decisions Taken")]
		public string DecisionsTaken { get; set; }

		[Display(Name = "Action Items")]
		public string ActionItems { get; set; }

		[Display(Name = "Next Meeting Date"), DataType(DataType.Date)]
		public DateTime? NextMeetingDate { get; set; }

		public DateTime CreatedOn { get; set; }
		public DateTime? LastUpdated { get; set; }
	}
}
