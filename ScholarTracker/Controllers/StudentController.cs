using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ScholarTracker.Models;

namespace ScholarTracker.Controllers
{
	public class StudentController : Controller
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

		public ActionResult Index()
		{
			var students = GetAllStudents();
			return View(students);
		}

		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StudentModel student)
		{
			if (ModelState.IsValid)
			{
				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_CreateStudent", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@FullName", student.FullName);
					cmd.Parameters.AddWithValue("@FatherName", student.FatherName);
					cmd.Parameters.AddWithValue("@MotherName", student.MotherName);
					cmd.Parameters.AddWithValue("@AdmissionDate", student.AdmissionDate);
					cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				return RedirectToAction("Index");
			}
			return View(student);
		}

		public ActionResult Edit(int id)
		{
			var student = GetStudentById(id);
			return student == null ? HttpNotFound() : (ActionResult)View(student);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StudentModel student)
		{
			if (ModelState.IsValid)
			{
				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_UpdateStudent", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", student.Id);
					cmd.Parameters.AddWithValue("@FullName", student.FullName);
					cmd.Parameters.AddWithValue("@FatherName", student.FatherName);
					cmd.Parameters.AddWithValue("@MotherName", student.MotherName);
					cmd.Parameters.AddWithValue("@AdmissionDate", student.AdmissionDate);
					cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				return RedirectToAction("Index");
			}
			return View(student);
		}
		public ActionResult Details(int id)
		{
			var student = GetStudentById(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			return View(student);
		}


		public ActionResult Delete(int id)
		{
			var student = GetStudentById(id);
			return student == null ? HttpNotFound() : (ActionResult)View(student);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
				using (var con = new SqlConnection(_connectionString))
				using (var cmd = new SqlCommand("sp_DeleteStudent", con))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Id", id);
					con.Open();
					cmd.ExecuteNonQuery();
				}
				return RedirectToAction("Index");
		}

		private List<StudentModel> GetAllStudents()
		{
			var list = new List<StudentModel>();
			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetAllStudents", con))
			{
				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						list.Add(new StudentModel
						{
							Id = Convert.ToInt32(reader["Id"]),
							FullName = reader["FullName"].ToString(),
							FatherName = reader["FatherName"].ToString(),
							MotherName = reader["MotherName"].ToString(),
							AdmissionDate = Convert.ToDateTime(reader["AdmissionDate"]),
							EnrollmentNo = reader["EnrollmentNo"].ToString()
						});
					}
				}
			}
			return list;
		}

		private StudentModel GetStudentById(int id)
		{
			StudentModel student = null;

			using (var con = new SqlConnection(_connectionString))
			using (var cmd = new SqlCommand("sp_GetStudentById", con))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", id);

				con.Open();
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						student = new StudentModel
						{
							Id = Convert.ToInt32(reader["Id"]),
							FullName = reader["FullName"].ToString(),
							FatherName = reader["FatherName"].ToString(),
							MotherName = reader["MotherName"].ToString(),
							AdmissionDate = Convert.ToDateTime(reader["AdmissionDate"]),
							EnrollmentNo = reader["EnrollmentNo"].ToString()
						};
					}
				}
			}

			return student;
		}

	}
}
