using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Models.ViewModels;
namespace CITS.EduSuite.Business.Services
{
    public class CommonSubjectService : ICommonSubjectService
    {
        private EduSuiteDatabase dbContext;

        public CommonSubjectService(EduSuiteDatabase objEduSuiteDatabse)
        {
            this.dbContext = objEduSuiteDatabse;
        }

        public CommonSubjectMasterViewModel GetCommonSubjectById(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel CreateCommonSubject(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel UpdateCommonSubject(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel DeleteCommonSubjectAll(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel DeleteCommonSubject(long Id)
        {
            throw new NotImplementedException();
        }

        public List<CommonSubjectMasterViewModel> GetCommonSubject(string SearchText)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel CheckCommonSubjectCodeExist(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel FillCourse(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }

        public CommonSubjectMasterViewModel FillUniversity(CommonSubjectMasterViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
