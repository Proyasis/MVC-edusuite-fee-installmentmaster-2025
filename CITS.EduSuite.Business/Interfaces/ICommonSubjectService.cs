using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface ICommonSubjectService
    {
        CommonSubjectMasterViewModel GetCommonSubjectById(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel CreateCommonSubject(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel UpdateCommonSubject(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel DeleteCommonSubjectAll(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel DeleteCommonSubject(long Id);

        List<CommonSubjectMasterViewModel> GetCommonSubject(string SearchText);

        CommonSubjectMasterViewModel CheckCommonSubjectCodeExist(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel FillCourse(CommonSubjectMasterViewModel model);

        CommonSubjectMasterViewModel FillUniversity(CommonSubjectMasterViewModel model);
    }
}
