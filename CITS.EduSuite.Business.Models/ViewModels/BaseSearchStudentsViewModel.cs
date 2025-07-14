using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BaseSearchStudentsViewModel
    {
        public BaseSearchStudentsViewModel()
        {
            AcademicTerms = new List<SelectListModel>();
            CourseTypes = new List<SelectListModel>();
            Courses = new List<SelectListModel>();
            UniversityMasters = new List<SelectListModel>();
            Modes = new List<SelectListModel>();
            Batches = new List<SelectListModel>();
            ClassModes = new List<SelectListModel>();
            Religions = new List<SelectListModel>();
            SecondLanguages = new List<SelectListModel>();
            Mediums = new List<SelectListModel>();
            Incomes = new List<SelectListModel>();
            NatureOfEnquiries = new List<SelectListModel>();
            Agents = new List<SelectListModel>();
            StudentStatuses = new List<SelectListModel>();
            Classes = new List<SelectListModel>();
            Columns = new List<SelectListModel>();
            CourseYears = new List<SelectListModel>();
            RegistrationStatus = new List<SelectListModel>();
            Castes = new List<SelectListModel>();
            CommunityTypes = new List<SelectListModel>();
            BloodGroups = new List<SelectListModel>();
            CourseTypeKeys = new List<long>();
            CourseKeys = new List<long>();
            UniversityMasterKeys = new List<long>();
            CourseYearsKeys = new List<long>();
            BatchKeys = new List<long>();
            SecondLanguageKeys = new List<long>();
            ReligionKeys = new List<long>();
            ModeKeys = new List<long>();
            ClassModeKeys = new List<long>();
            NatureOfEnquiryKeys = new List<long>();
            BranchKeys = new List<long>();
            AgentKeys = new List<long>();
            StudentStatusKeys = new List<long>();
            MeadiumKeys = new List<long>();
            ClassKeys = new List<long>();
            IncomeGroupKeys = new List<long>();
            RegistrationCatagoryKeys = new List<long>();
            CasteKeys = new List<long>();
            CommunityTypeKeys = new List<long>();
            BloodGroupKeys = new List<long>();

        }

        public List<SelectListModel> AcademicTerms { get; set; }
        public List<SelectListModel> CourseTypes { get; set; }
        public List<SelectListModel> Courses { get; set; }
        public List<SelectListModel> UniversityMasters { get; set; }
        public List<SelectListModel> Modes { get; set; }
        public List<SelectListModel> Batches { get; set; }
        public List<SelectListModel> Branches { get; set; }
        public List<SelectListModel> ClassModes { get; set; }
        public List<SelectListModel> Religions { get; set; }
        public List<SelectListModel> SecondLanguages { get; set; }
        public List<SelectListModel> Mediums { get; set; }
        public List<SelectListModel> Incomes { get; set; }
        public List<SelectListModel> NatureOfEnquiries { get; set; }
        public List<SelectListModel> Agents { get; set; }
        public List<SelectListModel> StudentStatuses { get; set; }
        public List<SelectListModel> Classes { get; set; }
        public List<SelectListModel> CourseYears { get; set; }
        public List<SelectListModel> Columns { get; set; }
        public List<SelectListModel> Employees { get; set; }
        public List<SelectListModel> RegistrationStatus { get; set; }
        public List<SelectListModel> Castes { get; set; }
        public List<SelectListModel> CommunityTypes { get; set; }
        public List<SelectListModel> BloodGroups { get; set; }
        public List<long> RegistrationCatagoryKeys { get; set; }
        public List<long> CasteKeys { get; set; }
        public List<long> CommunityTypeKeys { get; set; }
        public List<long> CourseTypeKeys { get; set; }
        public List<long> CourseKeys { get; set; }
        public List<long> UniversityMasterKeys { get; set; }
        public List<long> CourseYearsKeys { get; set; }
        public List<long> BatchKeys { get; set; }
        public List<long> SecondLanguageKeys { get; set; }
        public List<long> ReligionKeys { get; set; }
        public List<long> ModeKeys { get; set; }
        public List<long> ClassModeKeys { get; set; }
        public List<long> NatureOfEnquiryKeys { get; set; }
        public List<long> BranchKeys { get; set; }
        public List<long> AgentKeys { get; set; }
        public List<long> StudentStatusKeys { get; set; }
        public List<long> MeadiumKeys { get; set; }
        public List<long> ClassKeys { get; set; }
        public List<long> IncomeGroupKeys { get; set; }
        public List<long> BloodGroupKeys { get; set; }

        public int? ClassRequiredKey { get; set; }
        public int? IsTaxKey { get; set; }
        public int? IsInstallmentKey { get; set; }
        public int? IsConsessionKey { get; set; }
        public string IsEnquiryKey { get; set; }
        public int? GenderKey { get; set; }
        public short? AcademicTermKey { get; set; }

        public DateTime? DateAddedFrom { get; set; }
        public DateTime? DateAddedTo { get; set; }
        public DateTime? DateAdded { get; set; }
        public int? AddedBy { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
        public int? page { get; set; }
        public int? rows { get; set; }
        public int? draw { get; set; }
        //public int PageSize { get; set; }
        //public int PageIndex { get; set; }
        public long? TotalRecords { get; set; }
        public string SearchAnyText { get; set; }
        public short? BranchKey { get; set; }
        public short? BatchKey { get; set; }
    }



}
