using ScholarTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace ScholarTracker.Controllers
{
    public class HodCodController : Controller
    {

        DBHelper ObjDB = new DBHelper();
        // GET: HodCod
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult courseWD()
        {
            return View();
        }

        public ActionResult ProfileodPhDScholar()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ProfileodPhDScholar(PhDScholar model)
        {
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();

                lstParam.Clear();
                lstParam.Add(new SqlParameter("@EnrollNo", model.EnrollmentNo));
                DataSet dsScholarExist = ObjDB.ExecuteDataSet(CommandType.StoredProcedure, "sp_ScholarExist", lstParam.ToArray());
                DataTable dtScholarExist = dsScholarExist.Tables[0];

                if (dtScholarExist.Rows.Count > 0)
                {
                    Session.Clear();
                    Session["UserReg"] = "Scholar Already Exist. Please Try Other Enrollment Number."; ;
                }
                else
                {
                    lstParam.Clear();
                    lstParam.Add(new SqlParameter("@Action", 1));
                    lstParam.Add(new SqlParameter("@EnrollNo", model.EnrollmentNo));
                    lstParam.Add(new SqlParameter("@FullName", model.FullName));
                    lstParam.Add(new SqlParameter("@FatherName", model.FatherName));
                    lstParam.Add(new SqlParameter("@MotherName", model.MotherName));
                    lstParam.Add(new SqlParameter("@DepName", "CA (Computer Applicatin)"));
                    lstParam.Add(new SqlParameter("@AdmissionDate", model.AdmissionDate));
                    lstParam.Add(new SqlParameter("@PW", model.EnrollmentNo.ToString()));
                    lstParam.Add(new SqlParameter("@IsActive", 1));
                    int r = ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, "crudScholarProfile", lstParam.ToArray());
                    var item = (r > 0) ? "Success" : "User Does not Exists";

                    if (item == "Success")
                    {
                        Session.Clear();
                        Session["UserReg"] = "Success";
                    }
                    else
                    {
                        Session.Clear();
                        Session["UserReg"] = "Fail To Register Scholar. Please Check And Try Again.";
                    }
                }

            }
            catch (Exception ex)
            {
                Session.Clear();
                Session["UserReg"] = ex.Message.ToString();
            }

            ModelState.Clear();
            //return RedirectToAction("ProfileodPhDScholar");

            return View();

        }

        public ActionResult ViewScholar()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            MultiData multiData = new MultiData();

            try
            {

                List<PhDScholar> listScholar = new List<PhDScholar>();
                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Clear();
                lstParam.Add(new SqlParameter("@Action", "0"));
                DataSet dsScholarView = ObjDB.ExecuteDataSet(CommandType.StoredProcedure, "crudScholarProfile", lstParam.ToArray());
                DataTable dtScholarView = dsScholarView.Tables[0];


                if (dtScholarView.Rows.Count > 0)
                {
                    Session.Clear();
                    Session["UserView"] = "Success";

                    for (int i = 0; i < dtScholarView.Rows.Count; i++)
                    {
                        PhDScholar Obj = new PhDScholar();
                        Obj.EnrollmentNo = dtScholarView.Rows[i]["EnrollNo"].ToString();
                        Obj.FullName = dtScholarView.Rows[i]["FullName"].ToString();
                        Obj.Department = dtScholarView.Rows[i]["DepName"].ToString();
                        Obj.FatherName = dtScholarView.Rows[i]["FatherName"].ToString();
                        Obj.MotherName = dtScholarView.Rows[i]["MotherName"].ToString();
                        Obj.Password = dtScholarView.Rows[i]["Password"].ToString();
                        Obj.AdmissionDate = Convert.ToDateTime(dtScholarView.Rows[i]["AdmissionDate"].ToString());
                        //Obj.IsActive = bool.Parse(dt.Rows[i]["IsActive"].ToString());
                        listScholar.Add(Obj);
                    }
                    multiData.PhDScholarList = listScholar;
                }
                else
                {
                    Session.Clear();
                    Session["UserView"] = "No Record Found.";
                }
            }
            catch (Exception ex)
            {
                Session.Clear();
                Session["UserView"] = ex.Message.ToString();
            }


            return View(multiData);
        }

        [HttpPost]
        public ActionResult ViewScholar(PhDScholar model)
        {


            return View();
        }

        [HttpPost]
        public ActionResult EditScholar(string txtEnrNo, string txtPass, string txtActive, string txtAdDate, string txtFullName, string txtDepName, string txtFatherName, string txtMotherName)
        {
            try
            {
                List<SqlParameter> lstParam = new List<SqlParameter>();
                // EnrollNo		AdmissionDate	FatherName	MotherName	DepName	Password	IsActive
                lstParam.Clear();
                lstParam.Add(new SqlParameter("@Action", "2"));
                lstParam.Add(new SqlParameter("@FullName", txtFullName));
                lstParam.Add(new SqlParameter("@EnrollNo", txtEnrNo));
                lstParam.Add(new SqlParameter("@AdmissionDate", Convert.ToDateTime(txtAdDate)));
                lstParam.Add(new SqlParameter("@DepName", txtDepName));
                lstParam.Add(new SqlParameter("@FatherName", txtFatherName));
                lstParam.Add(new SqlParameter("@MotherName", txtMotherName));
                lstParam.Add(new SqlParameter("@PW", txtPass));
                lstParam.Add(new SqlParameter("@IsActive", txtActive));
                int i = ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, "crudScholarProfile", lstParam.ToArray());
                if (i == 1)
                {
                    Session["UserView"] = "Success";
                }
                else
                {
                    Session["UserView"] = "Category is not Updated. Please Try Again...";
                }
            }
            catch (Exception ex)
            {
                Session["UserView"] = ex.Message.ToString();
            }
            return RedirectToAction("ViewScholar", "HodCod");
        }



        [HttpPost]
        public ActionResult DeleteScholar(string txtID, string txtPass)
        {
            try
            {
                if (txtPass.Equals("Admin@123"))
                {
                    List<SqlParameter> lstParam = new List<SqlParameter>();

                    lstParam.Clear();
                    lstParam.Add(new SqlParameter("@Action", "3"));
                    lstParam.Add(new SqlParameter("@EnrollNo", txtID));
                    int i = ObjDB.ExecuteNonQuery(CommandType.StoredProcedure, "crudScholarProfile", lstParam.ToArray());
                    if (i == 1)
                    {
                        Session["UserView"] = "Deleted";
                    }
                    else
                    {
                        Session["UserView"] = "Deletion Fail, Please Try Again.";
                    }

                }
                else
                {
                    Session["UserView"] = "Invalid Password.";
                }
            }
            catch (Exception ex)
            {
                Session["UserView"] = ex.Message.ToString();
            }
            return RedirectToAction("ViewScholar", "HodCod");
        }

        public ActionResult PersonalD()
        {
            return View();
        }

        public ActionResult academicdetails()
        {
            return View();
        }

        public ActionResult ResearchD()
        {
            return View();
        }



       


        public ActionResult svD()
        {
            return View();
        }

        public ActionResult csvD()
        {
            return View();
        }

        public ActionResult PublicationD()
        {
            return View();
        }

    }
}