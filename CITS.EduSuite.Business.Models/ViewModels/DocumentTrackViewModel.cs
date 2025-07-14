using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class DocumentTrackViewModel : BaseModel
    {
        public long RowKey { get; set; }
        public long AppUserKey { get; set; }
        public long RowDataKey { get; set; }
        public DateTime? Date { get; set; }
        public short? DocumentType { get; set; }
        public string FilePath { get; set; }
        public bool IfDownload { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public string SearchText { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Course { get; set; }
        public string Batch { get; set; }
        public short? BatchKey { get; set; }
        public short? BranchKey { get; set; }
        public string Branch { get; set; }
        public string MobileNumber { get; set; }
        public string ApplicationStatusName { get; set; }
        public short? AcademicTermKey { get; set; }
        public short? CurrentYear { get; set; }
        public int? CourseDuration { get; set; }
        public int? DocumentViewCount { get; set; }
        public int? DocumentDownloadCount { get; set; }

    }
}
