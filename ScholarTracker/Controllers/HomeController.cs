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
        public ActionResult Index(ST_User model)
        {
            try
            {
                if (model.UserName != null && model.Password != null)
                {
                    int UT = 0;
                    string AC = "";
                    if (model.UserType == "HODCOD")
                    {
                        UT = 1;
                        AC = "HodCod";
                    }
                    else if (model.UserType == "GUIDE")
                    {
                        UT = 2;
                        AC = "Guide";
                    }
                    else if (model.UserType == "SCHOLAR")
                    {
                        UT = 3;
                        AC = "Scholar";
                    }
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Clear();
                    lstParam.Clear();
                    lstParam.Add(new SqlParameter("@Action", UT));
                    lstParam.Add(new SqlParameter("@ID", model.UserName));
                    lstParam.Add(new SqlParameter("@PW", model.Password));
                    DataSet dsSTUser = ObjDB.ExecuteDataSet(CommandType.StoredProcedure, "ExistUser", lstParam.ToArray());
                    DataTable dtSTUSer = dsSTUser.Tables[0];
                    var item = (dtSTUSer.Rows.Count > 0) ? "Success" : "User Does not Exists";

                    if (item == "Success")
                    {
                        model.Password = "";
                        Session.Clear();
                        Session["UserID"] = dtSTUSer.Rows[0]["ID"].ToString();
                        Session["UserName"] = dtSTUSer.Rows[0]["UserName"].ToString(); 
                        Session["UserPass"] = dtSTUSer.Rows[0]["Password"].ToString();
                        return RedirectToAction("Index", AC);
                        //return Redirect("~/SearchReference.aspx");

                    }
                    else if (item == "User Does Not Exists")
                    {
                        model.Password = "";
                        ViewBag.NotValidUser = item;
                        return View();
                    }

                    else
                    {
                        model.Password = "";
                        ViewBag.FailedCount = item;
                        return View();
                    }
                }
                else
                {
                    model.Password = "";
                    ViewBag.FailedCount = "User Name & Password is required";
                    return View();
                }

            }
            catch (Exception ex)
            {
                model.Password = "";
                ViewBag.FailedCount = ex.ToString();
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

