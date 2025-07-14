using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDashBoardService
    {

        #region Enquiry DashBoard
        List<dynamic> EnquiryCommon(DashBoardViewModel model);
        List<dynamic> EnquiryEmployeeCount(DashBoardViewModel model);

        #endregion Enquiry DashBoard


        #region Application DashBoard

        List<dynamic> StudentsCommon(DashBoardViewModel model);

        #endregion Application DashBoard

        #region Account DashBoard
        List<dynamic> AccountsCommon(DashBoardViewModel model);
        #endregion Account DashBoard

        #region Library DashBoard
        List<dynamic> LibraryCommon(DashBoardViewModel model);
        #endregion Library DashBoard
    }
}
