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
	public class CourseController : Controller
	{
		DBHelper ObjDB = new DBHelper();

		// GET: Course/courseWD
		public ActionResult courseWD(int? editId)
		{
			// Guard session
			if (Session["UserId"] == null)
				return RedirectToAction("Index", "Home");

			int userId = Convert.ToInt32(Session["UserId"]);

			// --- LISTING ---
			var list = new List<CourseWork>();
			DataSet ds = ObjDB.ExecuteDataSet(
				CommandType.StoredProcedure,
				"sp_GetCourseWorks",
				new SqlParameter("@FkUserId", userId)
			);
			if (ds.Tables.Count > 0)
			{
				foreach (DataRow r in ds.Tables[0].Rows)
				{
					list.Add(new CourseWork
					{
						Id = (int)r["Id"],
						Mode = (string)r["Mode"],
						Status = (string)r["Status"],
						StudyCenter = (string)r["StudyCenter"],
						Subject = (string)r["Subject"],
						CertificatePath = r["CertificatePath"] as string,
						FkUserId = (int)r["FkUserId"]
					});
				}
			}
			ViewBag.CourseWorks = list;

			// --- EDIT FETCH (if requested) ---
			var model = new CourseWork();
			if (editId.HasValue)
			{
				DataSet ds1 = ObjDB.ExecuteDataSet(
					CommandType.StoredProcedure,
					"sp_GetCourseWorkById",
					new SqlParameter("@Id", editId.Value),
					new SqlParameter("@FkUserId", userId)
				);
				if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
				{
					var r = ds1.Tables[0].Rows[0];
					model = new CourseWork
					{
						Id = (int)r["Id"],
						Mode = (string)r["Mode"],
						Status = (string)r["Status"],
						StudyCenter = (string)r["StudyCenter"],
						Subject = (string)r["Subject"],
						CertificatePath = r["CertificatePath"] as string,
						FkUserId = (int)r["FkUserId"]
					};
				}
			}

			return View(model);
		}

		// POST: /Course/courseWD
		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult courseWD(CourseWork model, HttpPostedFileBase certificate)
		{
			int userId = Convert.ToInt32(Session["UserId"]);

			if (!ModelState.IsValid)
			{
				ViewBag.CourseWorks = LoadForUser(userId);
				return View(model);
			}

			// --- FILE UPLOAD ---
			if (certificate != null && certificate.ContentLength > 0)
			{
				var fn = Path.GetFileName(certificate.FileName);
				var dir = Server.MapPath("~/Uploads");
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				var fp = Path.Combine(dir, fn);
				certificate.SaveAs(fp);
				model.CertificatePath = Url.Content("~/Uploads/" + fn);
			}
			else
			{
				// If no new file is uploaded, keep the existing CertificatePath
				var existingCourseWork = GetCourseWorkById(model.Id, userId);
				if (existingCourseWork != null)
				{
					model.CertificatePath = existingCourseWork.CertificatePath;
				}
			}

			// --- CALL SP ---
			string sp = model.Id == 0 ? "sp_AddCourseWork" : "sp_UpdateCourseWork";
			var parms = new List<SqlParameter>();
			if (model.Id != 0) parms.Add(new SqlParameter("@Id", model.Id));

			parms.Add(new SqlParameter("@Mode", model.Mode));
			parms.Add(new SqlParameter("@Status", model.Status));
			parms.Add(new SqlParameter("@StudyCenter", model.StudyCenter));
			parms.Add(new SqlParameter("@Subject", model.Subject));
			parms.Add(new SqlParameter(
				"@CertificatePath",
				(object)model.CertificatePath ?? DBNull.Value
			));
			parms.Add(new SqlParameter("@FkUserId", userId));

			ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, sp, parms.ToArray());

			return RedirectToAction("courseWD");
		}

		// POST: /Course/DeleteCourseWD
		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult DeleteCourseWD(int id)
		{
			int userId = Convert.ToInt32(Session["UserId"]);
			ObjDB.ExecuteNonQuery(
				CommandType.StoredProcedure,
				"sp_DeleteCourseWork",
				new SqlParameter("@Id", id),
				new SqlParameter("@FkUserId", userId)
			);
			return RedirectToAction("courseWD");
		}

		// Helper to reload listing for validation errors
		private List<CourseWork> LoadForUser(int userId)
		{
			var list = new List<CourseWork>();
			DataSet ds = ObjDB.ExecuteDataSet(
				CommandType.StoredProcedure,
				"sp_GetCourseWorks",
				new SqlParameter("@FkUserId", userId)
			);
			if (ds.Tables.Count > 0)
			{
				foreach (DataRow r in ds.Tables[0].Rows)
				{
					list.Add(new CourseWork
					{
						Id = (int)r["Id"],
						Mode = (string)r["Mode"],
						Status = (string)r["Status"],
						StudyCenter = (string)r["StudyCenter"],
						Subject = (string)r["Subject"],
						CertificatePath = r["CertificatePath"] as string,
						FkUserId = (int)r["FkUserId"]
					});
				}
			}
			return list;
		}

		// Helper to get existing CourseWork by Id
		private CourseWork GetCourseWorkById(int id, int userId)
		{
			CourseWork courseWork = null;
			DataSet ds = ObjDB.ExecuteDataSet(
				CommandType.StoredProcedure,
				"sp_GetCourseWorkById",
				new SqlParameter("@Id", id),
				new SqlParameter("@FkUserId", userId)
			);

			if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				var r = ds.Tables[0].Rows[0];
				courseWork = new CourseWork
				{
					Id = (int)r["Id"],
					Mode = (string)r["Mode"],
					Status = (string)r["Status"],
					StudyCenter = (string)r["StudyCenter"],
					Subject = (string)r["Subject"],
					CertificatePath = r["CertificatePath"] as string,
					FkUserId = (int)r["FkUserId"]
				};
			}

			return courseWork;
		}
	}
}

