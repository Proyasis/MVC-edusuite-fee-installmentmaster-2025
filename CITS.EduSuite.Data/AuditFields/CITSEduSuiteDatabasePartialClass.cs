using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CITS.EduSuite.Business.Models.Security;
using CITS.EduSuite.Business.Models.ViewModels;

namespace CITS.EduSuite.Data
{
    public partial class EduSuiteDatabase
    {
        public override int SaveChanges()
        {
            long UserKey = 0;
            foreach (var auditableEntity in ChangeTracker.Entries<IAuditable>())
            {

                if ((Thread.CurrentPrincipal as CITSEduSuitePrincipal) != null)
                {
                    UserKey = (Thread.CurrentPrincipal as CITSEduSuitePrincipal).UserKey;
                }
                else
                {
                    UserKey = 1;
                }

                //UserKey = (Thread.CurrentPrincipal as CITSEduSuitePrincipal).UserKey;

                if (auditableEntity.State == EntityState.Added ||
                    auditableEntity.State == EntityState.Modified)
                {
                    // implementation may change based on the useage scenario, this
                    // sample is for forma authentication.

                    //Harshal TO DO - Uncomment below line and use it in auditing fields
                    //string currentUser = HttpContext.Current.User.Identity.Name;

                    // modify updated date and updated by column for 
                    // adds of updates.
                    if (auditableEntity.State == EntityState.Modified)
                    {
                        auditableEntity.Entity.DateModified = DateTimeUTC.Now;
                        auditableEntity.Entity.ModifiedBy = UserKey;
                    }
                    // pupulate created date and created by columns for
                    // newly added record.
                    if (auditableEntity.State == EntityState.Added)
                    {
                        auditableEntity.Entity.DateAdded = DateTimeUTC.Now;
                        auditableEntity.Entity.AddedBy = UserKey;
                    }
                    else
                    {
                        // we also want to make sure that code is not inadvertly
                        // modifying created date and created by columns 
                        auditableEntity.Property(p => p.DateAdded).IsModified = false;
                        auditableEntity.Property(p => p.AddedBy).IsModified = false;
                    }
                }
            }
            AuditableLog(UserKey);
            return base.SaveChanges();
        }

        private void AuditableLog(long UserKey)
        {
            List<string> AvoidEntities = new List<string> { "AccountFlow" };

            this.Audits.AddRange(ChangeTracker.Entries().Where(p => p.State == EntityState.Modified && !AvoidEntities.Contains(ObjectContext.GetObjectType(p.Entity.GetType()).Name)).Select(x => new Audit
            {
                TableName = ObjectContext.GetObjectType(x.Entity.GetType()).Name,
                ActionName = x.State.ToString(),
                PrimaryKey = GetPrimaryKeyValue(x).ToString(),
                ChangedValues = GetValuesString(x, x.OriginalValues, x.CurrentValues),
                AddedBy = UserKey,
                DateAdded = DateTimeUTC.Now
            }).ToList());
            this.Audits.AddRange(ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted && !AvoidEntities.Contains(ObjectContext.GetObjectType(p.Entity.GetType()).Name)).Select(x => new Audit
            {
                TableName = ObjectContext.GetObjectType(x.Entity.GetType()).Name,
                ActionName = x.State.ToString(),
                PrimaryKey = GetPrimaryKeyValue(x).ToString(),
                ChangedValues = GetValuesString(x, x.OriginalValues, null),
                AddedBy = UserKey,
                DateAdded = DateTimeUTC.Now
            }).ToList());

        }

        object GetPrimaryKeyValue(DbEntityEntry entry)
        {
            var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        }

        string GetValuesString(DbEntityEntry entry, DbPropertyValues oldValues, DbPropertyValues newValues)
        {
            List<string> AvoidColumns = typeof(IAuditable).GetProperties().Select(x => x.Name).ToList();
            var sb = oldValues.PropertyNames.Where(x => !AvoidColumns.Contains(x) && (entry.Property(x).IsModified || entry.State == EntityState.Deleted)).Select(x => new { Name = x, OldValue = oldValues[x], NewValue = (newValues != null ? newValues[x] : null) }).ToList();
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(sb);
            return json;
        }
    }
    public static class GenericRepository
    {

        public static void AddToContext<T>(this DbContext context, T entity, int count)
    where T : class
        {
            int commitCount = 100;
            context.Set<T>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                //if (recreateContext)
                //{
                //    context.Dispose();
                //    context = new EduSuiteDatabase();
                //    context.Configuration.AutoDetectChangesEnabled = false;
                //}
            }

        }
        public static void AttachToContext<T>(this DbContext context, T entity, int count) where T : class
        {
            int commitCount = 100;
            context.Set<T>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            if (count % commitCount == 0)
            {
                context.SaveChanges();
                //if (recreateContext)
                //{
                //    context.Dispose();
                //    context = new EduSuiteDatabase();
                //    context.Configuration.AutoDetectChangesEnabled = false;
                //}
            }

        }
    }
}
