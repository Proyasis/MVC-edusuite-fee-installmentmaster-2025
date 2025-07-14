using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IMailMessagesServices
    {
        List<MailMessagesViewModel> GetMessages(MailMessagesViewModel model);
        MailMessagesViewModel CreateMessage(MailMessagesViewModel model);
        //MailMessagesViewModel CreateMailOutBox(MailMessagesViewModel model);
        MailMessagesViewModel UpdateTrash(MailMessagesViewModel model);
        MailMessagesViewModel UpdateStar(MailMessagesViewModel model);
        MailMessagesViewModel DeleteMailMessage(MailMessagesViewModel model);
        List<MailMessagesSenderListViewModel> GetEmployeeByText(MailMessagesViewModel model);
        MailMessagesViewModel GetMessagesContent(MailMessagesViewModel model);
        MailMessagesViewModel GetMailMessageById(MailMessagesViewModel model);

        void GetEmployees(MailMessagesViewModel model);
        void GetHigherEmployees(MailMessagesViewModel model);
    }
}
