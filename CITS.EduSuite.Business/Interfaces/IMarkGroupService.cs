using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
public interface    IMarkGroupService
    {
        MarkGroupViewModel GetMarkGroupById(int? id);
        MarkGroupViewModel CreateMarkGroup(MarkGroupViewModel model);
        MarkGroupViewModel UpdateMarkGroup(MarkGroupViewModel model);
        MarkGroupViewModel DeleteMarkGroup(MarkGroupViewModel model);
        List<MarkGroupViewModel> GetMarkGroup(string searchText);
    }
}
