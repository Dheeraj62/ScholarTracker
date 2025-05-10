using ScholarTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace ScholarTracker.Controllers
{
	public class HomeController : Controller
	{
		DBHelper ObjDB = new DBHelper();
		public ActionResult Index()
		{
			Session.Abandon();
			Session.Clear();
			return View();
		}


        [HttpPost]
        public JsonResult Index(ST_User model)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
                {
                    int roleId = model.UserType == "HODCOD" ? 1 :
                                 model.UserType == "GUIDE" ? 2 :
                                 model.UserType == "SCHOLAR" ? 3 : 0;

                    string controllerName = model.UserType == "HODCOD" ? "HodCod" :
                                            model.UserType == "GUIDE" ? "Guide" :
                                            model.UserType == "SCHOLAR" ? "Scholar" : "";

                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@RoleId", roleId),
                new SqlParameter("@UserName", model.UserName),
                new SqlParameter("@Password", model.Password)
            };

                    var ds = ObjDB.ExecuteDataSet(CommandType.StoredProcedure, "PROC_ExistUser", parameters.ToArray());
                    var dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        Session["UserID"] = dt.Rows[0]["UserId"].ToString();
                        Session["UserName"] = dt.Rows[0]["UserName"].ToString();
                        Session["UserPass"] = dt.Rows[0]["Password"].ToString();

                        // ✅ Return the redirect URL instead of redirecting
                        return Json("/" + controllerName + "/Index");
                    }

                    return Json(new { error = "Invalid username or password." });
                }

                return Json(new { error = "Username, password, and user type are required." });
            }
            catch (Exception ex)
            {
                return Json(new { error = "Login failed: " + ex.Message });
            }
        }



        public ActionResult SignUp()
		{
			ViewBag.Message = "SignUp redirection page.";
			return View();
		}

		[HttpPost]
		public ActionResult SignUp(ST_User model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					int roleId = 0;
					if (model.UserType == "HODCOD") roleId = 1;
					else if (model.UserType == "GUIDE") roleId = 2;
					else if (model.UserType == "SCHOLAR") roleId = 3;
					else
					{
						ViewBag.Message = "Invalid user type.";
						return View(model);
					}

					List<SqlParameter> parameters = new List<SqlParameter>
			{
				new SqlParameter("@UserName", model.UserName),
				new SqlParameter("@FirstName", model.FirstName),
				new SqlParameter("@MiddleName", model.MiddleName ?? string.Empty),
				new SqlParameter("@LastName", model.LastName),
				new SqlParameter("@Password", model.Password),
				new SqlParameter("@Mobile", model.Mobile),
				new SqlParameter("@Email", model.Email),
				new SqlParameter("@FkRoleId", roleId),
				new SqlParameter("@IsActive", true)
			};

					int result = ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, "PROC_AddUser", parameters.ToArray());

					if (result > 0)
					{
						TempData["SuccessMessage"] = "User registered successfully. Please log in.";
						return RedirectToAction("Index");
					}
					else
					{
						ViewBag.Message = "Failed to register user. Try again.";
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.Message = "Error occurred: " + ex.Message;
			}

			return View(model);
		}

	}
}

