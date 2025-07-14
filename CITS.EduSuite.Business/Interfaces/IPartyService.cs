using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IPartyService
    {
        PartyViewModel GetPartyById(long? id);

        PartyViewModel CreateParty(PartyViewModel model);

        PartyViewModel UpdateParty(PartyViewModel model);

        PartyViewModel DeleteParty(PartyViewModel model);

        List<PartyViewModel> GetParty(string searchText, PartyViewModel model);

        PartyViewModel GetPartyOrdersId(Int32? Id);

        PartyViewModel FillPartyType(PartyViewModel model);
        PartyViewModel FillBranches(PartyViewModel model);
        PartyViewModel CheckGSTINNumberExists(string GSTINNumber, byte? PartyTypeKey, long? RowKey);
        PartyViewModel FillPartyTypeById(PartyViewModel model);
    }
}
