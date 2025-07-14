using CITS.EduSuite.Business.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Interfaces
{
    public interface IAgentService
    {
        AgentViewModel GetAgentById(int? id);
        AgentViewModel CreateAgent(AgentViewModel model);
        AgentViewModel UpdateAgent(AgentViewModel model);
        AgentViewModel DeleteAgent(AgentViewModel model);
        List<AgentViewModel> GetAgent(string searchText);
    }
}
