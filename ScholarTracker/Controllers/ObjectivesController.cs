using ScholarTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScholarTracker.Controllers
{
    public class ObjectivesController : Controller
    {
        DBHelper ObjDB = new DBHelper();
        #region  Objectives
        // GET: /Guide/Objectives?scholarId=5[&editId=3]
        [HttpGet]
        public ActionResult Objectives(int? scholarId, int? editId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            // 1) Load Scholar dropdown
            var scholars = new List<SelectListItem>();
            var dsSch = ObjDB.ExecuteDataSet(
                CommandType.StoredProcedure,
                "sp_GetAllScholars"
            );
            if (dsSch.Tables.Count > 0)
            {
                foreach (DataRow r in dsSch.Tables[0].Rows)
                    scholars.Add(new SelectListItem
                    {
                        Value = r["Id"].ToString(),
                        Text = r["Name"].ToString(),
                        Selected = (scholarId.HasValue && (int)r["Id"] == scholarId.Value)
                    });
            }
            ViewBag.Scholars = scholars;

            // 2) If no scholar selected, just show dropdown
            ViewBag.Objectives = new List<Objectives>();
            Objectives model = new Objectives();

            if (scholarId.HasValue)
            {
                // 3) Load listing
                var list = new List<Objectives>();
                var ds = ObjDB.ExecuteDataSet(
                    CommandType.StoredProcedure,
                    "sp_GetObjectives",
                    new SqlParameter("@ScholarId", scholarId.Value),
                    new SqlParameter("@FkUserId", userId)
                );
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                        list.Add(new Objectives
                        {
                            Id = (int)r["Id"],
                            ScholarId = (int)r["ScholarId"],
                            Objective1 = (int)r["Objective1"],
                            Objective2 = (int)r["Objective2"],
                            Objective3 = (int)r["Objective3"],
                            Objective4 = (int)r["Objective4"]
                        });
                }
                ViewBag.Objectives = list;

                // 4) If editId, load that record
                if (editId.HasValue)
                {
                    var ds2 = ObjDB.ExecuteDataSet(
                        CommandType.StoredProcedure,
                        "sp_GetObjectiveById",
                        new SqlParameter("@Id", editId.Value),
                        new SqlParameter("@FkUserId", userId)
                    );
                    if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                    {
                        var r = ds2.Tables[0].Rows[0];
                        model = new Objectives
                        {
                            Id = (int)r["Id"],
                            ScholarId = (int)r["ScholarId"],
                            Objective1 = (int)r["Objective1"],
                            Objective2 = (int)r["Objective2"],
                            Objective3 = (int)r["Objective3"],
                            Objective4 = (int)r["Objective4"]
                        };
                    }
                }
                else
                {
                    // default new: set ScholarId so form dropdown retains selection
                    model.ScholarId = scholarId.Value;
                }
            }

            return View(model);
        }

        // POST: /Guide/Objectives
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Objectives(Objectives model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            // validation
            if (!ModelState.IsValid)
            {
                // re-load dropdown & list
                return RedirectToAction("Objectives",
                    new { scholarId = model.ScholarId });
            }

            // choose SP
            bool isUpdate = model.Id != 0;
            string sp = model.Id == 0
                ? "sp_AddObjective"
                : "sp_UpdateObjective";

            var parms = new List<SqlParameter>();

            // **ONLY for update**, add @Id first**
            if (isUpdate)
            {
                parms.Add(new SqlParameter("@Id", model.Id));
            }

            // now add the shared parameters
            parms.AddRange(new[]{
                new SqlParameter("@ScholarId",  model.ScholarId),
                new SqlParameter("@Objective1", model.Objective1),
                new SqlParameter("@Objective2", model.Objective2),
                new SqlParameter("@Objective3", model.Objective3),
                new SqlParameter("@Objective4", model.Objective4),
                new SqlParameter("@FkUserId",   userId)
            });

            ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, sp, parms.ToArray());

            return RedirectToAction("Objectives",
                new { scholarId = model.ScholarId });
        }

        // POST: /Guide/DeleteObjective
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteObjective(int id, int scholarId)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            ObjDB.ExecuteNonQuery(
                CommandType.StoredProcedure,
                "sp_DeleteObjective",
                new SqlParameter("@Id", id),
                new SqlParameter("@FkUserId", userId)
            );
            return RedirectToAction("Objectives", new { scholarId });
        }
        #endregion
    }
}