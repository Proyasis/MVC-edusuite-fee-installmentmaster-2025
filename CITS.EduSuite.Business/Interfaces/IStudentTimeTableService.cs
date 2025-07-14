using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IStudentTimeTableService
    {

        List<StudentTimeTableViewModel> GetTimeTable(StudentTimeTableViewModel model);
        StudentTimeTableViewModel GetTimeTableById(StudentTimeTableViewModel model);
        void FillDropDown(StudentTimeTableViewModel model);
        StudentTimeTableViewModel UpdateStudentTimeTableList(List<StudentTimeTableViewModel> modelList);
    }
}
