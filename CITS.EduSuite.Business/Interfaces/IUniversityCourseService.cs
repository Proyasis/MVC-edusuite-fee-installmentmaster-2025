using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IUniversityCourseService
    {
        UniversityCourseViewModel GetUniversityCourseById(int? id);

        UniversityCourseViewModel CreateUniversityCourse(UniversityCourseViewModel model);

        UniversityCourseViewModel UpdateUniversityCourse(UniversityCourseViewModel model);

        UniversityCourseViewModel DeleteUniversityCourse(UniversityCourseViewModel model);

        List<UniversityCourseViewModel> GetUniversityCourse(UniversityCourseViewModel model, out long TotalRecords);

        UniversityCourseViewModel FillCourse(UniversityCourseViewModel model);
        UniversityCourseViewModel FillAcademicTerm(UniversityCourseViewModel model);

        #region Class Details
        UniversityCourseViewModel UpdateClassModule(UniversityCourseViewModel model);

        UniversityCourseViewModel GetClassDetailsById(UniversityCourseViewModel model);

        UniversityCourseViewModel DeleteClassDetails(ClassDetailsModel model);

        UniversityCourseViewModel CheckClassCodeExists(string ClassCode, long RowKey);

        UniversityCourseViewModel CheckRoomNoExists(long? BuildingDetailsKey, long RowKey);

        List<UniversityCourseViewModel> GetClassDetailsList(UniversityCourseViewModel model);
        #endregion Class Details

        #region University Course Fee
        UniversityCourseViewModel UpdateUniversityCourseFeeModule(UniversityCourseViewModel model);
        UniversityCourseViewModel GetUniversityCourseFeeById(UniversityCourseViewModel model);
        UniversityCourseViewModel DeleteUniversityCourseFee(UniversityCourseFeeModel model);
        UniversityCourseViewModel ResetUniversityCourseFee(UniversityCourseFeeModel model);

        #endregion University Course Fee

        #region University Course Fee Installment
        UniversityCourseFeeInstallmentModel GetFeeInstallmentById(UniversityCourseFeeInstallmentModel model);


        UniversityCourseFeeInstallmentModel UpdateFeeInstallment(UniversityCourseFeeInstallmentModel model);

        UniversityCourseFeeInstallmentModel DeleteFeeInstallment(FeeInstallmentModel model);
        #endregion University Course Fee Installment
    }
}
