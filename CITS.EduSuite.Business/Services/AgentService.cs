using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Interfaces;
using CITS.EduSuite.Business.Models.Resources;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITS.EduSuite.Business.Services
{
    public class AgentService : IAgentService
    {
        private EduSuiteDatabase dbContext;
        public AgentService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<AgentViewModel> GetAgent(string searchText)
        {
            try
            {
                var AgentList = (from p in dbContext.Agents
                                 orderby p.RowKey descending
                                 where (p.AgentName.Contains(searchText) || p.AgentCode.Contains(searchText))
                                 select new AgentViewModel
                                 {
                                     RowKey = p.RowKey,
                                     AgentName = p.AgentName,
                                     AgentAddress = p.AgentAddress,
                                     AgentMobile = p.AgentMobile,
                                     AgentEmail = p.AgentEmail,
                                     AgentCode = p.AgentCode,
                                     AgentPhone = p.AgentPhone,
                                     IsActive = p.AgentActive ?? false

                                 }).ToList();
                return AgentList.GroupBy(x => x.RowKey).Select(y => y.First()).ToList<AgentViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<AgentViewModel>();


            }
        }
        public AgentViewModel GetAgentById(int? id)
        {
            try
            {
                AgentViewModel model = new AgentViewModel();
                model = dbContext.Agents.Select(row => new AgentViewModel
                {
                    RowKey = row.RowKey,
                    AgentName = row.AgentName,
                    AgentAddress = row.AgentAddress,
                    AgentMobile = row.AgentMobile,
                    AgentEmail = row.AgentEmail,
                    AgentCode = row.AgentCode,
                    AgentPhone = row.AgentPhone,
                    IsActive = row.AgentActive ?? false
                }).Where(x => x.RowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new AgentViewModel();
                }
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new AgentViewModel();

            }
        }
        public AgentViewModel CreateAgent(AgentViewModel model)
        {
            var AgentCheck = dbContext.Agents.Where(row => row.AgentCode.ToLower() == model.AgentCode.ToLower()).Count();
            Agent AgentModel = new Agent();
            if (AgentCheck != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Agent);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    int MaxKey = dbContext.Agents.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    AgentModel.RowKey = Convert.ToInt16(MaxKey + 1);
                    AgentModel.AgentName = model.AgentName;
                    AgentModel.AgentCode = model.AgentCode;
                    AgentModel.AgentAddress = model.AgentAddress;
                    AgentModel.AgentMobile = model.AgentMobile;
                    AgentModel.AgentEmail = model.AgentEmail;
                    AgentModel.AgentPhone = model.AgentPhone;
                    AgentModel.AgentActive = model.IsActive;
                    dbContext.Agents.Add(AgentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Add, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Agent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Add, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);
                }
            }
            return model;
        }
        public AgentViewModel UpdateAgent(AgentViewModel model)
        {
            var AgentCheck = dbContext.Agents.Where(row => row.AgentCode.ToLower() == model.AgentCode.ToLower()
               && row.RowKey != model.RowKey).ToList();

            Agent AgentModel = new Agent();
            if (AgentCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Agent);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    AgentModel = dbContext.Agents.SingleOrDefault(x => x.RowKey == model.RowKey);
                    AgentModel.AgentName = model.AgentName;
                    AgentModel.AgentCode = model.AgentCode;
                    AgentModel.AgentAddress = model.AgentAddress;
                    AgentModel.AgentMobile = model.AgentMobile;
                    AgentModel.AgentEmail = model.AgentEmail;
                    AgentModel.AgentPhone = model.AgentPhone;
                    AgentModel.AgentActive = model.IsActive;
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;

                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Edit, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Agent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Edit, DbConstants.LogType.Error, model.RowKey, ex.GetBaseException().Message);

                }
            }
            return model;

        }
        public AgentViewModel DeleteAgent(AgentViewModel model)
        {
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Agent AgentModel = dbContext.Agents.SingleOrDefault(row => row.RowKey == model.RowKey);
                    dbContext.Agents.Remove(AgentModel);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Delete, DbConstants.LogType.Info, model.RowKey, model.Message);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Agent);
                        model.IsSuccessful = false;
                        ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Agent);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Agent, ActionConstants.Delete, DbConstants.LogType.Debug, model.RowKey, ex.GetBaseException().Message);
                }
            }

            return model;
        }

    }
}
