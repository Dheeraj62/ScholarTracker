using ScholarTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScholarTracker.Controllers
{
    public class GuideController : Controller
    {
        DBHelper ObjDB = new DBHelper();
        // GET: Guide
        public ActionResult Index()
		{
			return RedirectToAction("Index", "Student");
		}

        public ActionResult ProfileodPhDScholar()
        {
            return View();
        }

        public ActionResult courseWD()
		{
			return RedirectToAction("courseWD", "Course");
		}

        public ActionResult Objectives()
        {
            return View();
        }

        public ActionResult PublicationD()
		{
			return RedirectToAction("PublicationD", "Publication");
		}

            public ActionResult PrePhD()
        {
            return View();
        }

        public ActionResult Mentored()
        {
            return View();
        }


        #region Crud
        [HttpGet]
        public ActionResult courseWD(int? editId)
        {
            // 1) guard session
            if (Session["UserId"] == null)
                return RedirectToAction("Index", "Home");

            int userId = Convert.ToInt32(Session["UserId"]);

            // — LISTING —
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

            // — EDIT FETCH (if requested) —
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

        // POST: /Guide/courseWD
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult courseWD(CourseWork model, HttpPostedFileBase certificate)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            if (!ModelState.IsValid)
            {
                ViewBag.CourseWorks = LoadForUser(userId);
                return View(model);
            }

            // — FILE UPLOAD —
            if (certificate != null && certificate.ContentLength > 0)
            {
                var fn = Path.GetFileName(certificate.FileName);
                var dir = Server.MapPath("~/Uploads");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var fp = Path.Combine(dir, fn);
                certificate.SaveAs(fp);
                model.CertificatePath = Url.Content("~/Uploads/" + fn);
            }

            // — CALL SP —
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

        // POST: /Guide/DeleteCourseWD
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

        // reload listing for validation errors
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

        #endregion
    }

}