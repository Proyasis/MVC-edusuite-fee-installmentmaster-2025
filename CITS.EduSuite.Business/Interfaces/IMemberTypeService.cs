using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IMemberTypeService
    {
        List<MemberTypeViewModel> GetMemberTypes(string searchText);
        MemberTypeViewModel GetMemberTypeById(byte? id);
        MemberTypeViewModel CreateMemberType(MemberTypeViewModel model);
        MemberTypeViewModel UpdateMemberType(MemberTypeViewModel model);
        MemberTypeViewModel DeleteMemberType(MemberTypeViewModel model);
        MemberTypeViewModel CheckMemberTypeCodeExist(MemberTypeViewModel model);
    }
}
