using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace camis.aggregator.data.Entities
{
    public partial class aggregatorContext : DbContext
    {
        private DbConnection _connection;

        public aggregatorContext(DbConnection connection)
        {
            _connection = connection;
            ContextOwnsConnection = false;
        }

        public bool ContextOwnsConnection { get; } = true;
        public DbConnection Connection { get => _connection; }

        public void SaveChanges(string username, UserAction userAction)
        {
            UserAction.Add(userAction);
            base.SaveChanges();
            var auditEntries = OnBeforeSaveChanges(username, userAction.Id);
            base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
        }

        public UserAction SaveChanges(string username, int actionType)
        {
            var userAction = new UserAction
            {
                ActionTypeId = actionType,
                Username = username,
                Timestamp = DateTime.Now.Ticks
            };
            UserAction.Add(userAction);
            var auditEnries = OnBeforeSaveChanges(username, userAction.Id);
            base.SaveChanges();
            OnAfterSaveChanges(auditEnries);

            return userAction;
        }

        private List<AuditEntry> OnBeforeSaveChanges(string username, long actionId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is UserAction || entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName(),
                    UserName = username,
                    UserAction = actionId
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary) //For auto-generated properties such as id
                    {
                        //get the value after save
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    var propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }

                            break;
                    }
                }
            }

            //Save audit log for all the changes
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
                AuditLog.Add(auditEntry.ToAudit());

            //return those for which we need to get their primary keys for
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private void OnAfterSaveChanges(List<AuditEntry> entries)
        {
            if (entries == null || entries.Count == 0) return;


            foreach (var auditEntry in entries)
            {
                //Get the auto-generated values
                foreach (var prop in auditEntry.TemporaryProperties)
                    if (prop.Metadata.IsPrimaryKey())
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    else
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;

                AuditLog.Add(auditEntry.ToAudit());
            }

            base.SaveChanges();
        }
        public Npgsql.NpgsqlCommand CreateReaderCommand(String sql)
        {
            this.Database.OpenConnection();
            var con = (Npgsql.NpgsqlConnection)this.Database.GetDbConnection();

            var cmd = new Npgsql.NpgsqlCommand(sql, con);
            return cmd;

        }
        public System.Data.DataTable GetDataTable(String sql)
        {
            this.Database.OpenConnection();
            var con = (Npgsql.NpgsqlConnection)this.Database.GetDbConnection();
            using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
            using (var adapter = new Npgsql.NpgsqlDataAdapter(cmd))
            {
                var t = new System.Data.DataTable();
                adapter.Fill(t);
                return t;
            }
        }
    }
}
