using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
   public interface IStudentDiaryService
    {
        StudentDiaryViewModel GetStudentDiaryById(StudentDiaryViewModel model);
        StudentDiaryViewModel CreateStudentDiaryDate(StudentDiaryViewModel model);
        StudentDiaryViewModel UpdateStudentDiaryDate(StudentDiaryViewModel model);
        StudentDiaryViewModel DeleteStudentDiary(long? Id);
        List<StudentDiaryViewModel> GetStudentDiaryDetails(long ApplicationKey);
    }
}
