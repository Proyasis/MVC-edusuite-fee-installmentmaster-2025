using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;


namespace CITS.EduSuite.Business.Interfaces
{
    public interface IJournalService
    {
        JournalViewModel GetJournalById(JournalViewModel model);
        JournalViewModel CreateJournal(JournalViewModel model);
        JournalViewModel UpdateJournal(JournalViewModel model);
        JournalViewModel DeleteJournal(JournalViewModel model);
        List<JournalViewModel> GetJournal(JournalViewModel model, out long TotalRecords);
        JournalViewModel FillBranches(JournalViewModel model);
        JournalDetailsViewModel FillAcountHead(JournalDetailsViewModel model);
        JournalViewModel DeleteJournalItem(JournalViewModel model);
    }
}
