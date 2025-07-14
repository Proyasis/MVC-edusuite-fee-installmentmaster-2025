using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Models.ViewModels
{
    public class BookIssueSummaryReportViewModel : BaseSearchStudentsViewModel
    {
        public BookIssueSummaryReportViewModel()
        {
            BookDetailsList = new List<BookDetails>();
        }

        public int NotAvailable { get; set; }
        public int Available { get; set; }
        public int Issued { get; set; }
        public long CourseKey { get; set; }

        public List<BookDetails> BookDetailsList  {get;set;}

    }


    public class BookDetails
    {
        public string BookCode { get; set; }
        public long ApplicationKey { get; set; }
        public string SubjectName { get; set; }
        public int Year { get; set; }
        public bool  isAvailable{get;set;}
        public bool Issued { get; set; }
        public bool AvailableCount { get; set; }
    }


}
