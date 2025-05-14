using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ScholarTracker.Models;

namespace ScholarTracker.Controllers
{
	public class SupervisorController : Controller
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

		private int GetCurrentUserId()
		{
			if (Session["UserId"] != null)
				return Convert.ToInt32(Session["UserId"]);

			throw new UnauthorizedAccessException("User not authenticated");
		}

		public ActionResult Index()
		{
			int userId = GetCurrentUserId();
			var supervisors = GetUserSupervisors(userId);
			return View(supervisors);
		}

		[HttpGet]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Supervisor supervisor)
		{
			int userId = GetCurrentUserId();
			if (ModelState.IsValid)
			{
				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_CreateSupervisor", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@FkUserId", userId);
					cmd.Parameters.AddWithValue("@InfoAsPerRecords", supervisor.InfoAsPerRecords ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@AdhaarNo", supervisor.AdhaarNo ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@PancardNo", supervisor.PancardNo ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Title", supervisor.Title ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@FirstName", supervisor.FirstName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@MiddleName", supervisor.MiddleName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@LastName", supervisor.LastName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Designation", supervisor.Designation ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Country", supervisor.Country ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@State", supervisor.State ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@CityDistrict", supervisor.CityDistrict ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Address", supervisor.Address ?? (object)DBNull.Value);

					con.Open();
					supervisor.Id = Convert.ToInt32(cmd.ExecuteScalar());
				}

				return RedirectToAction("Index");
			}

			return View(supervisor);
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			int userId = GetCurrentUserId();
			var supervisor = GetSupervisorById(id, userId);
			if (supervisor == null)
				return HttpNotFound();

			return View(supervisor);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Supervisor supervisor)
		{
			int userId = GetCurrentUserId();
			if (ModelState.IsValid)
			{
				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_UpdateSupervisor", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", supervisor.Id);
					cmd.Parameters.AddWithValue("@FkUserId", userId);
					cmd.Parameters.AddWithValue("@InfoAsPerRecords", supervisor.InfoAsPerRecords ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@AdhaarNo", supervisor.AdhaarNo ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@PancardNo", supervisor.PancardNo ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Title", supervisor.Title ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@FirstName", supervisor.FirstName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@MiddleName", supervisor.MiddleName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@LastName", supervisor.LastName ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Designation", supervisor.Designation ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Country", supervisor.Country ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@State", supervisor.State ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@CityDistrict", supervisor.CityDistrict ?? (object)DBNull.Value);
					cmd.Parameters.AddWithValue("@Address", supervisor.Address ?? (object)DBNull.Value);

					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0)
						return HttpNotFound();
				}

				return RedirectToAction("Index");
			}

			return View(supervisor);
		}

		[HttpGet]
		public ActionResult Details(int id)
		{
			int userId = GetCurrentUserId();
			var supervisor = GetSupervisorById(id, userId);
			if (supervisor == null)
				return HttpNotFound();

			return View(supervisor);
		}

		[HttpGet]
		public ActionResult Delete(int id)
		{
			int userId = GetCurrentUserId();
			var supervisor = GetSupervisorById(id, userId);
			if (supervisor == null)
				return HttpNotFound();

			return View(supervisor);
		}

		[HttpPost, ActionName("DeleteConfirmed")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			int userId = GetCurrentUserId();

			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_DeleteSupervisor", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@FkUserId", userId);

				con.Open();
				var rows = cmd.ExecuteNonQuery();
				if (rows == 0)
					return HttpNotFound();
			}

			return RedirectToAction("Index");
		}

		#region Helper Methods

		private List<Supervisor> GetUserSupervisors(int userId)
		{
			var list = new List<Supervisor>();

			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetSupervisorsByUser", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@FkUserId", userId);

				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new Supervisor
						{
							Id = Convert.ToInt32(reader["Id"]),
							InfoAsPerRecords = reader["InfoAsPerRecords"] as string,
							AdhaarNo = reader["AdhaarNo"] as string,
							PancardNo = reader["PancardNo"] as string,
							Title = reader["Title"] as string,
							FirstName = reader["FirstName"] as string,
							MiddleName = reader["MiddleName"] as string,
							LastName = reader["LastName"] as string,
							Designation = reader["Designation"] as string,
							Country = reader["Country"] as string,
							State = reader["State"] as string,
							CityDistrict = reader["CityDistrict"] as string,
							Address = reader["Address"] as string
						});
					}
				}
			}

			return list;
		}

		private Supervisor GetSupervisorById(int id, int userId)
		{
			Supervisor supervisor = null;

			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetSupervisorById", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@FkUserId", userId);

				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						supervisor = new Supervisor
						{
							Id = Convert.ToInt32(reader["Id"]),
							InfoAsPerRecords = reader["InfoAsPerRecords"] as string,
							AdhaarNo = reader["AdhaarNo"] as string,
							PancardNo = reader["PancardNo"] as string,
							Title = reader["Title"] as string,
							FirstName = reader["FirstName"] as string,
							MiddleName = reader["MiddleName"] as string,
							LastName = reader["LastName"] as string,
							Designation = reader["Designation"] as string,
							Country = reader["Country"] as string,
							State = reader["State"] as string,
							CityDistrict = reader["CityDistrict"] as string,
							Address = reader["Address"] as string
						};
					}
				}
			}

			return supervisor;
		}
		#endregion
	}
}
