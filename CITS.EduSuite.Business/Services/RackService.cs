using CITS.EduSuite.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using CITS.EduSuite.Data;
using CITS.EduSuite.Business.Common;
using CITS.EduSuite.Business.Models.ViewModels;
using CITS.EduSuite.Business.Models.Resources;

namespace CITS.EduSuite.Business.Services
{
    public class RackService : IRackService
    {
        private EduSuiteDatabase dbContext;

        public RackService(EduSuiteDatabase objDB)
        {
            this.dbContext = objDB;
        }

        public List<RackViewModel> GetRack(string searchText, short? BranchKey)
        {
            try
            {
                var rackList = (from r in dbContext.Racks
                                join u in dbContext.AppUsers
                                on r.RowKey equals u.RoleKey into joined
                                from j in joined.DefaultIfEmpty()
                                orderby r.RowKey descending
                                where (r.RackName.Contains(searchText))
                                select new RackViewModel
                                {
                                    MasterRowKey = r.RowKey,
                                    RackName = r.RackName,
                                    Remarks = r.Remarks,
                                    BranchName = r.Branch.BranchName,
                                    RackCode = r.RackCode,
                                    BranchKey = r.BranchKey,
                                    SubRackCount = r.SubRacks.Select(x => x.RowKey).Count(),
                                    IsActive = r.IsActive ?? false
                                }).ToList();
                if (BranchKey != 0)
                {
                    rackList = rackList.Where(x => x.BranchKey == BranchKey).ToList();
                }

                return rackList.GroupBy(x => x.MasterRowKey).Select(y => y.First()).ToList<RackViewModel>();
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.View, DbConstants.LogType.Error, null, ex.GetBaseException().Message);
                return new List<RackViewModel>();

            }
        }

        public RackViewModel GetRackById(int? id)
        {
            try
            {
                RackViewModel model = new RackViewModel();
                model = dbContext.Racks.Select(row => new RackViewModel
                {
                    MasterRowKey = row.RowKey,
                    RackName = row.RackName,
                    RackCode = row.RackCode,
                    Remarks = row.Remarks,
                    IsActive = row.IsActive ?? false,
                    BranchKey = row.BranchKey,

                }).Where(x => x.MasterRowKey == id).FirstOrDefault();
                if (model == null)
                {
                    model = new RackViewModel();
                }
                FillSubRackDetails(model);
                FillDropdownList(model);
                return model;
            }
            catch (Exception ex)
            {
                ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.View, DbConstants.LogType.Error, id, ex.GetBaseException().Message);
                return new RackViewModel();


            }
        }

        public RackViewModel CreateRack(RackViewModel model)
        {
            FillDropdownList(model);
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var RackNameCheck = dbContext.Racks.Where(row => row.RackName.ToLower() == model.RackName.ToLower()).ToList();
            Rack RackModel = new Rack();

            if (RackNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Rack);
                model.IsSuccessful = false;
                return model;
            }

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Int32 maxKey = dbContext.Racks.Select(p => p.RowKey).DefaultIfEmpty().Max();

                    RackModel.RowKey = Convert.ToInt16(maxKey + 1);
                    RackModel.RackName = model.RackName;

                    RackModel.Remarks = model.Remarks;
                    RackModel.IsActive = model.IsActive;
                    RackModel.RackCode = model.RackCode;
                    RackModel.BranchKey = model.BranchKey;
                    dbContext.Racks.Add(RackModel);

                    CreateSubRack(model.SubRackDetailsModel.Where(x => x.RowKey == 0).ToList(), RackModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Add, DbConstants.LogType.Info, model.MasterRowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Rack);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Add, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public RackViewModel UpdateRack(RackViewModel model)
        {
            FillDropdownList(model);
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            var rackNameCheck = dbContext.Racks.Where(row => row.RackName.ToLower() == model.RackName.ToLower() && row.RowKey != model.MasterRowKey).ToList();
            Rack rackModel = new Rack();

            if (rackNameCheck.Count != 0)
            {
                model.Message = String.Format(EduSuiteUIResources.ExistsErrorMessage, EduSuiteUIResources.Rack);
                model.IsSuccessful = false;
                return model;
            }


            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    rackModel = dbContext.Racks.SingleOrDefault(row => row.RowKey == model.MasterRowKey);
                    rackModel.RackName = model.RackName;

                    rackModel.Remarks = model.Remarks;
                    rackModel.IsActive = model.IsActive;
                    rackModel.BranchKey = model.BranchKey;
                    CreateSubRack(model.SubRackDetailsModel.Where(x => x.RowKey == 0).ToList(), rackModel.RowKey);
                    UpdateSubRack(model.SubRackDetailsModel.Where(x => x.RowKey != 0).ToList(), rackModel.RowKey);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Edit, DbConstants.LogType.Info, model.MasterRowKey, model.Message);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToSaveErrorMessage, EduSuiteUIResources.Rack);
                    model.IsSuccessful = false;
                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Edit, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }

        public RackViewModel DeleteRack(RackViewModel model)
        {
            //using (cPOSEntities dbContext = new cPOSEntities())
            //{
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    List<SubRack> rackCatagoryList = dbContext.SubRacks.Where(x => x.RackKey == model.MasterRowKey).ToList();
                    Rack rack = dbContext.Racks.SingleOrDefault(row => row.RowKey == model.MasterRowKey);

                    dbContext.SubRacks.RemoveRange(rackCatagoryList);
                    dbContext.Racks.Remove(rack);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Delete, DbConstants.LogType.Info, model.MasterRowKey, model.Message);

                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Rack);
                        model.IsSuccessful = false;

                        ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Rack);
                    model.IsSuccessful = false;

                    ActivityLog.CreateActivityLog(MenuConstants.Rack, ActionConstants.Delete, DbConstants.LogType.Error, model.MasterRowKey, ex.GetBaseException().Message);
                }
            }
            //}
            return model;
        }


        private void FillSubRackDetails(RackViewModel model)
        {
            model.SubRackDetailsModel = (from row in dbContext.SubRacks.Where(x => x.RackKey == model.MasterRowKey)
                                         select new SubRackDetailsModel
                                         {
                                             RowKey = row.RowKey,
                                             RackKey = row.RackKey,
                                             SubRackName = row.SubRackName,
                                             SubRackCode = row.SubRackCode
                                         }).ToList();
            if (model.SubRackDetailsModel.Count == 0)
            {
                model.SubRackDetailsModel.Add(new SubRackDetailsModel());
            }
        }

        private void FillDropdownList(RackViewModel model)
        {
            FillBranches(model);

        }

        private void CreateSubRack(List<SubRackDetailsModel> modelList, int RackKey)
        {


            long Maxkey = dbContext.SubRacks.Select(x => x.RowKey).DefaultIfEmpty().Max();



            foreach (SubRackDetailsModel model in modelList)
            {

                //var SubRackCheck = dbContext.RackCatgories.Where(row => row.SubRackName.ToLower() == model.SubRackName.ToLower() && row.SubRackCode.ToLower() == model.SubRackCode.ToLower()).ToList();
                //RackViewModel objmodel = new RackViewModel();

                //if (SubRackCheck.Count != 0)
                //{
                //    objmodel.Message = ApplicationResources.ErrorRackExists;
                //    objmodel.IsSuccessful = false;

                //}

                SubRack rackCatagorymodel = new SubRack();
                rackCatagorymodel.RowKey = Maxkey + 1;
                rackCatagorymodel.SubRackName = model.SubRackName;
                rackCatagorymodel.SubRackCode = model.SubRackCode;
                rackCatagorymodel.RackKey = RackKey;
                dbContext.SubRacks.Add(rackCatagorymodel);
                Maxkey++;

            }
        }
        private void UpdateSubRack(List<SubRackDetailsModel> modelList, int RackKey)
        {

            foreach (SubRackDetailsModel model in modelList)
            {
                SubRack rackCatagorymodel = new SubRack();
                rackCatagorymodel = dbContext.SubRacks.SingleOrDefault(x => x.RowKey == model.RowKey);
                rackCatagorymodel.SubRackName = model.SubRackName;
                rackCatagorymodel.SubRackCode = model.SubRackCode;
                rackCatagorymodel.RackKey = RackKey;
            }
        }

        public RackViewModel DeleteSubRack(long id)
        {
            RackViewModel model = new RackViewModel();
            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    SubRack rackcatagory = dbContext.SubRacks.SingleOrDefault(row => row.RowKey == id);
                    dbContext.SubRacks.Remove(rackcatagory);
                    dbContext.SaveChanges();
                    transaction.Commit();
                    model.Message = EduSuiteUIResources.Success;
                    model.IsSuccessful = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    if (ex.GetBaseException().Message.ToUpper().Contains(EduSuiteUIResources.ForeignKeyError.ToUpper()))
                    {
                        model.Message = String.Format(EduSuiteUIResources.CantDeleteErrorMessage, EduSuiteUIResources.Rack);
                        model.IsSuccessful = false;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    model.Message = String.Format(EduSuiteUIResources.FailedToDeleteErrorMessage, EduSuiteUIResources.Rack);
                    model.IsSuccessful = false;
                }
            }
            //}
            return model;
        }

        public RackViewModel CheckRackCodeExists(string RackCode, int RowKey)
        {
            RackViewModel model = new RackViewModel();

            if (dbContext.Racks.Where(row => row.RackCode.ToLower() == RackCode.ToLower() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public RackViewModel CheckSubRackCodeExists(string SubRackCode, long RowKey)
        {
            RackViewModel model = new RackViewModel();

            if (dbContext.SubRacks.Where(row => row.SubRackCode.ToLower() == SubRackCode.ToLower() && row.RowKey != RowKey).Any())
            {
                model.IsSuccessful = false;
            }
            else
            {
                model.IsSuccessful = true;
            }
            return model;
        }
        public void FillBranches(RackViewModel model)
        {

            IQueryable<SelectListModel> BranchQuery = dbContext.vwBranchSelectActiveOnlies.OrderBy(row => row.DisplayOrder).Select(row => new SelectListModel
            {
                RowKey = row.RowKey,
                Text = row.BranchName
            });

            if (!DbConstants.Role.AdminUserTypes.Contains(DbConstants.User.RoleKey))
            {
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                        model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                        //model.BranchKey = Employee.BranchKey;
                    }
                    else
                    {
                        model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                        //model.BranchKey = Employee.BranchKey;
                    }
                }
                else
                {
                    model.Branches = BranchQuery.ToList();
                    if (model.Branches.Count == 1)
                    {
                        long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                        model.BranchKey = Convert.ToInt16(branchkey);

                    }
                }
            }
            else
            {

                model.Branches = BranchQuery.ToList();
                Employee Employee = dbContext.Employees.Where(x => x.AppUserKey == DbConstants.User.UserKey).SingleOrDefault();
                if (Employee != null)
                {
                    if (Employee.BranchAccess != null)
                    {
                        List<long> Branches = Employee.BranchAccess.Split(',').Select(Int64.Parse).ToList();
                        model.Branches = BranchQuery.Where(row => Branches.Contains(row.RowKey)).ToList();
                        //model.BranchKey = Employee.BranchKey;

                    }
                    else
                    {
                        model.Branches = BranchQuery.Where(x => x.RowKey == Employee.BranchKey).ToList();
                        //model.BranchKey = Employee.BranchKey;
                    }
                }
                if (model.Branches.Count == 1)
                {
                    long? branchkey = model.Branches.Select(x => x.RowKey).FirstOrDefault();
                    model.BranchKey = Convert.ToInt16(branchkey);

                }
            }
        }
    }
}
