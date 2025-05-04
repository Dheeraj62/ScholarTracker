

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScholarTracker.Models
{
    public class MultiData
    {
        public List<Dashbord1> Dashbord1List { get; set; }
        public List<ST_User> ST_UserList { get; set; }
        public List<PhDScholar> PhDScholarList { get; set; }
    }
    public class Dashbord1
    {
        public string TotalScholar { get; set; }
    }
    public class ST_User
    {
        public int UID { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string EmpID { get; set; }
        public string DID { get; set; }
        public string DepartmentName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        public string Designation { get; set; }
        public string Department { get; set; }
        public string UserRoll { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string TransUser { get; set; }
        public DateTime TransDate { get; set; }
        public bool IsActive { get; set; }
    }
    public class PhDScholar
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Required]
        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9]+", ErrorMessage = "Only letters and numbers allowed")]
        [Display(Name = "Enrollment No.")]
        public string EnrollmentNo { get; set; }

        
        public string Department { get; set; }

        public string Password { get; set; }
    }
    public class PersonalDetails
    {
        [Required]
        public string Gender { get; set; }

        [Required]
        public string Religion { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{10}", ErrorMessage = "Enter valid 10-digit number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Invalid PAN format")]
        public string Pan { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{12}", ErrorMessage = "Enter valid 12-digit Aadhar number")]
        public string Aadhar { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"[0-9]{6}", ErrorMessage = "Enter valid 6-digit Pin Code")]
        public string Pincode { get; set; }

        public string Country { get; set; } = "India";
        public string State { get; set; } = "Uttar Pradesh";
        public string City { get; set; } = "Lucknow";
    }
    public class AcademicDetail
    {
        [Required]
        [Display(Name = "Degree Type")]
        public string DegreeType { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        public string Branch { get; set; }

        [Required]
        public string University { get; set; }

        [Required]
        [Range(1900, 2099)]
        [Display(Name = "Year Of Passing")]
        public int YearOfPassing { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Percentage { get; set; }
    }
    public class ResearchDetail
    {
        [Required]
        [Display(Name = "Research Place")]
        public string ResearchPlace { get; set; }

        [Required]
        [Display(Name = "Study Center")]
        public string StudyCenter { get; set; }

        [Required]
        public string Field { get; set; }

        [Required]
        public string Faculty { get; set; }

        [Required]
        public string Topic { get; set; }
    }
    public class CourseWork
    {
        [Required]
        public string Mode { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string StudyCenter { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Certificate { get; set; }

    }
    public class SupervisorDetails
    {
        [Required]
        public string Info { get; set; }

        [Required]
        [RegularExpression(@"\d{12}", ErrorMessage = "Enter a valid 12-digit Aadhaar number.")]
        public string Adhar { get; set; }

        [Required]
        [RegularExpression(@"[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Enter a valid PAN number.")]
        public string Pan { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"\d{10}", ErrorMessage = "Enter a valid 10-digit phone number.")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Designation { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Address { get; set; }
    }
    public class CoSupervisorDetails
    {
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhaar must be 12 digits.")]
        public string AadhaarNo { get; set; }

        [RegularExpression(@"[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Enter valid PAN number.")]
        public string PanNo { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string PhoneNo { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Address { get; set; }
    }
    public class PublicationDetails
    {
        [Required]
        public string JournalIndex { get; set; }

        [Required]
        public string JournalName { get; set; }

        [Required]
        public string TitleOfPaper { get; set; }

        [Required]
        public string PublisherName { get; set; }

        [Required]
        [Range(0, 1000)]
        public double ImpactFactor { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfIssue { get; set; }

        [Required]
        public int Volume { get; set; }
        [Required]
        public int Issue { get; set; }

        [Required]
        //[StringLength(100)]
        public string Doi { get; set; }

        //[Required]
        //public string DOI { get; set; }

        [Required]
        public string PageNo { get; set; }

        [Required]
        public List<PublicationDetails> Publications { get; set; }

    }

}

