using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CITS.Validations;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class EmployeePersonalViewModel : BaseModel
    {
        public EmployeePersonalViewModel()
        {

            Religions = new List<SelectListModel>();
            Salutations = new List<SelectListModel>();
            BloodGroups = new List<SelectListModel>();
            MaritalStatuses = new List<SelectListModel>();
            EmployeeStatuses = new List<SelectListModel>();
            Grades = new List<SelectListModel>();
            Designations = new List<SelectListModel>();
            Nationalities = new List<SelectListModel>();
            Branches = new List<SelectListModel>();
            Departments = new List<SelectListModel>();
            EmployeeCategories = new List<SelectListModel>();
            HigherEmployees = new List<SelectListModel>();
            TelephoneCodes = new List<SelectListModel>();
            JoiningDate = DateTimeUTC.Now;
            EmployeeStatusKey = DbConstants.EmployeeStatus.Working;
            Countries = new List<SelectListModel>();
            SalaryType = new List<SelectListModel>();
            AttendanceCategories = new List<SelectListModel>();
            AttendanceConfigTypes = new List<SelectListModel>();
            Shifts = new List<SelectListModel>();
        }
        public long RowKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Salutation_Required")]
        public byte SalutationKey { get; set; }
        public string SalutationName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [System.Web.Mvc.Remote("CheckEmployeeCodeExists", "EmployeePersonal", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeCodeExists")]
        [Display(Name = "Code", ResourceType = typeof(EduSuiteUIResources))]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "FirstName", ResourceType = typeof(EduSuiteUIResources))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "LastName", ResourceType = typeof(EduSuiteUIResources))]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RegularExpressionErrorMessage")]
        [Display(Name = "MiddleName", ResourceType = typeof(EduSuiteUIResources))]
        public string MiddleName { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [LessThan("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DateOfBirthCompareErrorMessage")]
        [Display(Name = "DateOfBirth", ResourceType = typeof(EduSuiteUIResources))]

        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Gender", ResourceType = typeof(EduSuiteUIResources))]
        public byte Gender { get; set; }

        public DateTime DateTimeNow { get { return DateTimeUTC.Now; } }

        [StringLength(15, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PhoneNumberExpressionErrorMessage")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExpressionErrorMessage")]
        //[System.Web.Mvc.Remote("CheckMobileNumberExists", "EmployeePersonal", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MobileNumberExists")]
        [Display(Name = "MobileNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string MobileNumber { get; set; }

        [RequiredIf("NotificationByEmail", "true", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExpressionErrorMessage")]
        //[System.Web.Mvc.Remote("CheckEmailAddressExists", "EmployeePersonal", AdditionalFields = "RowKey", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmailAddressExists")]
        [Display(Name = "EmailAddress", ResourceType = typeof(EduSuiteUIResources))]
        public string EmailAddress { get; set; }
        public short TelephoneCodeKey { get; set; }
        public string TelephoneCodeName { get; set; }
        public bool NotificationByEmail { get; set; }
        public bool NotificationBySMS { get; set; }
        public byte? BloodGroupKey { get; set; }
        public string BloodGroupName { get; set; }
        public short? NationalityKey { get; set; }
        public string NationalityName { get; set; }
        public DateTime? LastActive { get; set; }
        public short? ReligionKey { get; set; }
        public string ReligionName { get; set; }
        public short? MaritalStatusKey { get; set; }
        public string MaritalStatusName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "BranchRequired")]
        public short BranchKey { get; set; }
        public string BranchName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DepartmentRequired")]
        public short DepartmentKey { get; set; }
        public string DepartmentName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeCategoryRequired")]
        public short EmployeeCategoryKey { get; set; }
        public string EmployeeCategoryName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "DesignationRequired")]
        public short DesignationKey { get; set; }
        public string DesignationName { get; set; }
        
        //[RequiredIf("SalaryTypeKey", DbConstants.SalaryType.Monthly, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "GradeRequired")]
        public int? GradeKey { get; set; }
        public string GradeName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmployeeStatusRequired")]
        public short EmployeeStatusKey { get; set; }
        public string EmployeeStatusName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JoiningDateRequired")]
        [LessThan("DateTimeNow", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "JoiningDateCompareErrorMessage")]
        public DateTime JoiningDate { get; set; }
        public DateTime? ReleiveDate { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [Display(Name = "BiomatricID", ResourceType = typeof(EduSuiteUIResources))]
        public string BiomatricID { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmployeePhotoPath { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmergencyContactPersonLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "EmergencyContactPersonRegularExpressionErrorMessage")]
        public string EmergencyContactPerson { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonRelationshipLengthErrorMessage")]
        [RegularExpression(@"^[0-9 a-zA-Z *()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonRelationshipRegularExpressionErrorMessage")]
        public string ContactPersonRelationship { get; set; }

        [StringLength(10, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthErrorMessage")]
        [RegularExpression(@"^[0-9*()&-/]+$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "ContactPersonNumberRegularExpressionErrorMessage")]
        [Display(Name = "PhoneNumber", ResourceType = typeof(EduSuiteUIResources))]
        public string ContactPersonNumber { get; set; }                
        public long? HigherEmployeeUserKey { get; set; }
        public string HigherEmployeeName { get; set; }
        public long? AppUserKey { get; set; }
        public string AppUserName { get; set; }
        public string PasswordHint { get; set; }
       
        public byte? SalaryTypeKey { get; set; }

        //[RequiredIfNotWithNull("SalaryTypeKey", DbConstants.SalaryType.Monthly, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "MonthlySalaryRequired")]
        [RegularExpression(@"^\d{0,10}(\.\d{1,2})?$", ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "Decimal10RegularExpressionErrorMessage")]
        public decimal? MonthlySalary { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "WorkTypeRequired")]
        public byte? EmployeeWorkTypeKey { get; set; }
        public string EmployeeWorkTypeName { get; set; }
        public short? AttendanceCategoryKey { get; set; }
        public string AttendanceCategoryName { get; set; }
        public byte? AttendanceConfigTypeKey { get; set; }
        public string AttendanceConfigTypeName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequiredErrorMessage")]
        [Display(Name = "Shift", ResourceType = typeof(EduSuiteUIResources))]
        public int? ShiftKey { get; set; }
        public string ShiftName { get; set; }
        public long? EmployeeKey { get; set; }
        public bool IsTeacher { get; set; }
        public List<SelectListModel> Religions { get; set; }
        public List<SelectListModel> Salutations { get; set; }
        public List<SelectListModel> BloodGroups { get; set; }
        public List<SelectListModel> MaritalStatuses { get; set; }
        public List<SelectListModel> EmployeeStatuses { get; set; }
        public List<SelectListModel> Grades { get; set; }
        public List<SelectListModel> Designations { get; set; }
        public List<SelectListModel> Nationalities { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> Departments { get; set; }
        public List<SelectListModel> EmployeeCategories { get; set; }
        public List<SelectListModel> HigherEmployees { get; set; }
        public List<SelectListModel> TelephoneCodes { get; set; }
        public List<SelectListModel> Countries { get; set; }
        public List<SelectListModel> SalaryType { get; set; }
        public List<SelectListModel> WorkType { get; set; }
        public List<SelectListModel> AttendanceCategories { get; set; }
        public List<SelectListModel> AttendanceConfigTypes { get; set; }
        public List<SelectListModel> Shifts { get; set; }
    }


}

