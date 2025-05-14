using ScholarTracker.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ScholarTracker.Controllers
{
	public class RDCMinutesController : Controller
	{
		private readonly string _cs = ConfigurationManager
			.ConnectionStrings["DefaultConnection"].ConnectionString;

		private int CurrentUserId() =>
			Convert.ToInt32(Session["UserId"]
				?? throw new UnauthorizedAccessException("Not logged in"));

		// GET: /RDCMinutes
		public ActionResult Index()
		{
			var list = new List<RDCMinutesModel>();
			var userId = CurrentUserId();

			using (var con = new SqlConnection(_cs))
			using (var cmd = new SqlCommand("sp_GetAllRDCMinutes", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@CreatedBy", userId);
				con.Open();
				using (var rdr = cmd.ExecuteReader())
				{
					while (rdr.Read())
					{
						list.Add(new RDCMinutesModel
						{
							Id = (int)rdr["Id"],
							FkScholarId = (int)rdr["FkScholarId"],
							ScholarName = (string)rdr["ScholarName"],
							FkSupervisorId = (int)rdr["FkSupervisorId"],
							SupervisorName = (string)rdr["SupervisorName"],
							MeetingDate = (DateTime)rdr["MeetingDate"],
							MeetingAgenda = rdr["MeetingAgenda"] as string,
							DiscussionPoints = rdr["DiscussionPoints"] as string,
							DecisionsTaken = rdr["DecisionsTaken"] as string,
							ActionItems = rdr["ActionItems"] as string,
							NextMeetingDate = rdr["NextMeetingDate"] as DateTime?,
							CreatedOn = (DateTime)rdr["CreatedOn"],
							LastUpdated = rdr["LastUpdated"] as DateTime?
						});
					}
				}
			}

			return View(list);
		}

		// GET: /RDCMinutes/Create
		public ActionResult Create()
		{
			ViewBag.Scholars = LoadDropdownScholars("sp_GetScholarsForDropdown");
			ViewBag.Supervisors = LoadDropdownSupervisors("sp_GetSupervisorsForDropdown");
			return View();
		}

		// POST: /RDCMinutes/Create
		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult Create(RDCMinutesModel m)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.Scholars = LoadDropdownScholars("sp_GetScholarsForDropdown");
				ViewBag.Supervisors = LoadDropdownSupervisors("sp_GetSupervisorsForDropdown");
				return View(m);
			}

			using (var con = new SqlConnection(_cs))
			using (var cmd = new SqlCommand("sp_InsertRDCMinutes", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@FkScholarId", m.FkScholarId);
				cmd.Parameters.AddWithValue("@FkSupervisorId", m.FkSupervisorId);
				cmd.Parameters.AddWithValue("@MeetingDate", m.MeetingDate);
				cmd.Parameters.AddWithValue("@MeetingAgenda", (object)m.MeetingAgenda ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@DiscussionPoints", (object)m.DiscussionPoints ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@DecisionsTaken", (object)m.DecisionsTaken ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@ActionItems", (object)m.ActionItems ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@NextMeetingDate", (object)m.NextMeetingDate ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@CreatedBy", CurrentUserId());

				con.Open();
				cmd.ExecuteNonQuery();
			}

			return RedirectToAction("Index");
		}

		// GET: /RDCMinutes/Details/5
		public ActionResult Details(int id)
		{
			RDCMinutesModel m = null;
			var userId = CurrentUserId();

			using (var con = new SqlConnection(_cs))
			using (var cmd = new SqlCommand("sp_GetRDCMinuteById", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@CreatedBy", userId);
				con.Open();
				using (var rdr = cmd.ExecuteReader())
				{
					if (rdr.Read())
					{
						m = new RDCMinutesModel
						{
							Id = rdr["Id"] != DBNull.Value ? (int)rdr["Id"] : 0,
							FkScholarId = rdr["FkScholarId"] != DBNull.Value ? (int)rdr["FkScholarId"] : 0,
							ScholarName = rdr["ScholarName"] != DBNull.Value ? (string)rdr["ScholarName"] : string.Empty,
							FkSupervisorId = rdr["FkSupervisorId"] != DBNull.Value ? (int)rdr["FkSupervisorId"] : 0,
							SupervisorName = rdr["SupervisorName"] != DBNull.Value ? (string)rdr["SupervisorName"] : string.Empty,
							MeetingDate = rdr["MeetingDate"] != DBNull.Value ? (DateTime)rdr["MeetingDate"] : DateTime.MinValue,
							MeetingAgenda = rdr["MeetingAgenda"] != DBNull.Value ? (string)rdr["MeetingAgenda"] : string.Empty,
							DiscussionPoints = rdr["DiscussionPoints"] != DBNull.Value ? (string)rdr["DiscussionPoints"] : string.Empty,
							DecisionsTaken = rdr["DecisionsTaken"] != DBNull.Value ? (string)rdr["DecisionsTaken"] : string.Empty,
							ActionItems = rdr["ActionItems"] != DBNull.Value ? (string)rdr["ActionItems"] : string.Empty,
							NextMeetingDate = rdr["NextMeetingDate"] != DBNull.Value ? (DateTime?)rdr["NextMeetingDate"] : null,
							CreatedOn = rdr["CreatedOn"] != DBNull.Value ? (DateTime)rdr["CreatedOn"] : DateTime.MinValue,
							LastUpdated = rdr["LastUpdated"] != DBNull.Value ? (DateTime?)rdr["LastUpdated"] : null
						};

					}
				}
			}

			if (m == null) return HttpNotFound();
			return View(m);
		}

		// Utility to load any “Id/Name” dropdown SP 
		private SelectList LoadDropdownScholars(string proc)
		{
			var items = new List<dynamic>();
			using (var con = new SqlConnection(_cs))
			using (var cmd = new SqlCommand(proc, con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				con.Open();
				using (var rdr = cmd.ExecuteReader())
				{
					if (rdr.HasRows)
					{
						while (rdr.Read())
						{
							// Using column names directly
							int idIndex = rdr.GetOrdinal("Id");
							int nameIndex = rdr.GetOrdinal("Name");

							items.Add(new
							{
								Id = rdr.IsDBNull(idIndex) ? 0 : rdr.GetInt32(idIndex),
								Name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex)
							});
						}
					}
				}
			}

			// Handle empty list (optional: you can return null or a default value)
			if (items.Count == 0)
			{
				items.Add(new { Id = 0, Name = "No Data Available" });
			}

			return new SelectList(items, "Id", "Name");
		}
		private SelectList LoadDropdownSupervisors(string proc)
		{
			var items = new List<dynamic>();
			using (var con = new SqlConnection(_cs))
			using (var cmd = new SqlCommand(proc, con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				con.Open();
				using (var rdr = cmd.ExecuteReader())
				{
					if (rdr.HasRows)
					{
						while (rdr.Read())
						{
							// Using column names directly
							int idIndex = rdr.GetOrdinal("Id");
							int nameIndex = rdr.GetOrdinal("Name");

							items.Add(new
							{
								Id = rdr.IsDBNull(idIndex) ? 0 : rdr.GetInt32(idIndex),
								Name = rdr.IsDBNull(nameIndex) ? string.Empty : rdr.GetString(nameIndex)
							});
						}
					}
				}
			}

			// Handle empty list (optional: you can return null or a default value)
			if (items.Count == 0)
			{
				items.Add(new { Id = 0, Name = "No Data Available" });
			}

			return new SelectList(items, "Id", "Name");
		}

	}
}
