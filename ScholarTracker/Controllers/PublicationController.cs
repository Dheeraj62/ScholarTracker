using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ScholarTracker.Models;

namespace ScholarTracker.Controllers
{
	public class PublicationController : Controller
	{
		private readonly string _connectionString = "Data Source=PAVILION\\SQLEXPRESS;Initial Catalog=ScholarDb;Integrated Security=True";
		private const string UploadPath = "~/Uploads/Publications/";

		private int GetCurrentUserId()
		{
			if (Session["UserId"] != null)
				return Convert.ToInt32(Session["UserId"]);

			throw new UnauthorizedAccessException("User not authenticated");
		}

		public ActionResult PublicationD()
		{
			int userId = GetCurrentUserId();
			var publications = GetUserPublications(userId);
			return View(publications);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Publication publication)
		{
			int userId = GetCurrentUserId();
			publication.FkUserId = userId;

			if (ModelState.IsValid)
			{
				string filePath = null;
				if (publication.PublicationFile != null && publication.PublicationFile.ContentLength > 0)
				{
					filePath = SaveFile(publication.PublicationFile);
				}

				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_CreatePublication", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@FkUserId", userId);
					cmd.Parameters.AddWithValue("@JournalIndex", publication.JournalIndex);
					cmd.Parameters.AddWithValue("@JournalName", publication.JournalName);
					cmd.Parameters.AddWithValue("@Title", publication.Title);
					cmd.Parameters.AddWithValue("@Publisher", publication.Publisher);
					cmd.Parameters.AddWithValue("@ImpactFactor", publication.ImpactFactor);
					cmd.Parameters.AddWithValue("@DateOfIssue", publication.DateOfIssue);
					cmd.Parameters.AddWithValue("@Volume", publication.Volume);
					cmd.Parameters.AddWithValue("@Issue", publication.Issue);
					cmd.Parameters.AddWithValue("@DOI", publication.DOI);
					cmd.Parameters.AddWithValue("@PageNo", publication.PageNo);
					cmd.Parameters.AddWithValue("@FilePath", filePath ?? (object)DBNull.Value);

					con.Open();
					publication.Id = Convert.ToInt32(cmd.ExecuteScalar());
				}

				return RedirectToAction("PublicationD");
			}

			return View("PublicationD", GetUserPublications(userId));
		}

		public ActionResult Edit(int id)
		{
			int userId = GetCurrentUserId();
			var publication = GetPublicationById(id, userId);
			if (publication == null)
				return HttpNotFound();

			return View(publication);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Publication publication)
		{
			int userId = GetCurrentUserId();
			publication.FkUserId = userId;

			if (ModelState.IsValid)
			{
				var oldFilePath = GetPublicationFilePath(publication.Id, userId);
				string newFilePath = oldFilePath;

				if (publication.PublicationFile != null && publication.PublicationFile.ContentLength > 0)
				{
					if (!string.IsNullOrEmpty(oldFilePath))
					{
						var oldPhysicalPath = Server.MapPath(oldFilePath);
						if (System.IO.File.Exists(oldPhysicalPath))
							System.IO.File.Delete(oldPhysicalPath);
					}
					newFilePath = SaveFile(publication.PublicationFile);
				}

				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_UpdateUserPublication", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", publication.Id);
					cmd.Parameters.AddWithValue("@UserId", userId);
					cmd.Parameters.AddWithValue("@JournalIndex", publication.JournalIndex);
					cmd.Parameters.AddWithValue("@JournalName", publication.JournalName);
					cmd.Parameters.AddWithValue("@Title", publication.Title);
					cmd.Parameters.AddWithValue("@Publisher", publication.Publisher);
					cmd.Parameters.AddWithValue("@ImpactFactor", publication.ImpactFactor);
					cmd.Parameters.AddWithValue("@DateOfIssue", publication.DateOfIssue);
					cmd.Parameters.AddWithValue("@Volume", publication.Volume);
					cmd.Parameters.AddWithValue("@Issue", publication.Issue);
					cmd.Parameters.AddWithValue("@DOI", publication.DOI);
					cmd.Parameters.AddWithValue("@PageNo", publication.PageNo);
					cmd.Parameters.AddWithValue("@FilePath", newFilePath ?? (object)DBNull.Value);

					con.Open();
					var rows = cmd.ExecuteNonQuery();
					if (rows == 0)
						return HttpNotFound();
				}
				return RedirectToAction("PublicationD");
			}
			return View(publication);
		}

		public ActionResult Delete(int id)
		{
			int userId = GetCurrentUserId();
			var publication = GetPublicationById(id, userId);
			if (publication == null)
				return HttpNotFound();
			return View(publication);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			int userId = GetCurrentUserId();
			string filePath = null;

			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_DeleteUserPublication", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@UserId", userId);
				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
						filePath = reader["DeletedFilePath"] as string;
				}
			}

			if (!string.IsNullOrEmpty(filePath))
			{
				var physicalPath = Server.MapPath(filePath);
				if (System.IO.File.Exists(physicalPath))
					System.IO.File.Delete(physicalPath);
			}
			return RedirectToAction("PublicationD");
		}

		#region Helper Methods

		private List<Publication> GetUserPublications(int userId)
		{
			var list = new List<Publication>();
			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetPublicationsByUser", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId", userId);
				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new Publication
						{
							Id = Convert.ToInt32(reader["Id"]),
							FkUserId = userId,
							JournalIndex = reader["JournalIndex"].ToString(),
							JournalName = reader["JournalName"].ToString(),
							Title = reader["Title"].ToString(),
							Publisher = reader["Publisher"].ToString(),
							ImpactFactor = Convert.ToDecimal(reader["ImpactFactor"]),
							DateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]),
							Volume = Convert.ToInt32(reader["Volume"]),
							Issue = Convert.ToInt32(reader["Issue"]),
							DOI = reader["DOI"].ToString(),
							PageNo = reader["PageNo"].ToString(),
							FilePath = reader["FilePath"] as string
						});
					}
				}
			}
			return list;
		}

		private Publication GetPublicationById(int id, int userId)
		{
			Publication pub = null;
			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetUserPublicationById", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@UserId", userId);
				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						pub = new Publication
						{
							Id = Convert.ToInt32(reader["Id"]),
							FkUserId = userId,
							JournalIndex = reader["JournalIndex"].ToString(),
							JournalName = reader["JournalName"].ToString(),
							Title = reader["Title"].ToString(),
							Publisher = reader["Publisher"].ToString(),
							ImpactFactor = Convert.ToDecimal(reader["ImpactFactor"]),
							DateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]),
							Volume = Convert.ToInt32(reader["Volume"]),
							Issue = Convert.ToInt32(reader["Issue"]),
							DOI = reader["DOI"].ToString(),
							PageNo = reader["PageNo"].ToString(),
							FilePath = reader["FilePath"] as string
						};
					}
				}
			}
			return pub;
		}

		private string GetPublicationFilePath(int id, int userId)
		{
			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("SELECT FilePath FROM Publications WHERE Id = @Id AND FkUserId = @UserId", con))
			{
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.Parameters.AddWithValue("@UserId", userId);
				con.Open();
				var result = cmd.ExecuteScalar();
				return result as string;
			}
		}

		private string SaveFile(HttpPostedFileBase file)
		{
			var uploadDir = Server.MapPath(UploadPath);
			if (!Directory.Exists(uploadDir))
				Directory.CreateDirectory(uploadDir);

			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
			var path = Path.Combine(uploadDir, fileName);
			file.SaveAs(path);

			return UploadPath.TrimStart('~') + fileName;
		}

		#endregion
	}
}
