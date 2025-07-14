using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IPublisherService
    {
        List<PublisherViewModel> GetPublisher(PublisherViewModel model, out long TotalRecords);

        PublisherViewModel GetPublisherById(Int32? id);

        PublisherViewModel CreatePublisher(PublisherViewModel model);

        PublisherViewModel UpdatePublisher(PublisherViewModel model);

        PublisherViewModel DeletePublisher(PublisherViewModel model);
    }
}
