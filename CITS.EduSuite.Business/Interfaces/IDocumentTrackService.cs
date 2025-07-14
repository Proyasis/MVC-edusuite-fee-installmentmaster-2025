using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IDocumentTrackService
    {
        DocumentTrackViewModel CreateDocumentTrack(DocumentTrackViewModel model);
        List<DocumentTrackViewModel> GetDocumentTrackList(DocumentTrackViewModel model, out long TotalRecords);
    }
}
