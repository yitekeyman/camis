using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Workflows.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace intapscamis.camis.domain.Workflows
{
    public interface IWorkflowService : ICamisService
    {
        void SetSession(UserSession session);

        Workflow CreateWorkflow(WorkflowRequest request);
        Workflow UpdateWorkflow(Guid id, int currentState, string description = null);
        WorkflowResponse GetWorkflow(Guid id);
        IList<WorkflowResponse> GetUserWorkflows(UserSession session);

        WorkItem CreateWorkItem(WorkItemRequest request);
        WorkItemResponse GetWorkItem(Guid workItemId);
        IList<WorkItemResponse> GetWorkItems(Guid workflowId);
        WorkItemResponse GetLastWorkItem(Guid workflowId);

        int GetWorkflowState(Guid workItemId);
        Guid GetWorkflowId(Guid workItemId);
        IList<WorkflowResponse> GetCurrentWorkItemsForUser(String userName, IList<long> role);
        WorkItem CreateWorkItemChangeState(WorkItemRequest request);
        WorkItemResponse GetLastWorkItem<T>(Guid wfid);
        List<Workflow> GetWorkflows(int prepareLand, int started);
    }

    public class WorkflowService : CamisService, IWorkflowService
    {
        private UserSession _session;

        public void SetSession(UserSession session)
        {
            _session = session;
        }


        public Workflow CreateWorkflow(WorkflowRequest request)
        {
            var workflow = new Workflow
            {
                Id = request.Id ?? Guid.NewGuid(),
                CurrentState = request.CurrentState,
                Description = request.Description,
                TypeId = request.TypeId
            };

            Context.Workflow.Add(workflow);

            workflow.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.AddWorkflow).Id;
            Context.Workflow.Update(workflow);
            Context.SaveChanges();
            return workflow;
        }

        public Workflow UpdateWorkflow(Guid id, int currentState, string description = null)
        {
            var workflow = Context.Workflow.First(wf => wf.Id == id);

            workflow.CurrentState = currentState;
            workflow.Description = description;

            Context.Workflow.Update(workflow);

            workflow.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.UpdateWorkflow).Id;
            Context.Workflow.Update(workflow);
            Context.SaveChanges();

            return workflow;
        }

        public WorkflowResponse GetWorkflow(Guid id)
        {
            var workflow = Context.Workflow.First(wf => wf.Id == id);
            workflow.Type = Context.WorkflowType.First(x => x.Id == workflow.TypeId);
            return new WorkflowResponse
            {
                Id = workflow.Id,
                CurrentState = workflow.CurrentState,
                Description = workflow.Description,
                TypeId = workflow.TypeId,

                Type = new WorkflowTypeResponse
                {
                    Id = workflow.Type.Id,
                    Name = workflow.Type.Name,
                    Description = workflow.Type.Description
                }
            };
        }

        public IList<WorkflowResponse> GetCurrentWorkItemsForUser(String userName, IList<long> role)
        {
            var us = new UserService(Context, new UserActionService(Context));
            var user = us.GetUser(userName);

            String roles = null;
            var dbRoles = Context.UserRole.Where(x => x.UserId == user.Id);
            foreach (var r in role)
            {
                if (!dbRoles.Where(ur => ur.RoleId == r).Any())
                    throw new InvalidOperationException($"User doesn't have role number {r}");
                roles = roles == null ? r.ToString() : role + "," + r;
            }

            var sql = $@"
Select  wi.* from wf.work_item wi
inner join (Select workflow_id,max(seq_no) as seq_no from wf.work_item group by workflow_id) mx
on mx.workflow_id=wi.workflow_id and mx.seq_no=wi.seq_no
where 
(wi.assigned_user={user.Id} or wi.assigned_role in ({roles}))
";
            var workItems = Context.WorkItem.FromSqlRaw(sql).ToList();
            var ret = new List<WorkflowResponse>();
            foreach (var workItem in workItems)
            {
                var workflow = Context.Workflow.First(wf => wf.Id == workItem.WorkflowId);
                var workflowType = Context.WorkflowType.First(wt => wt.Id == workflow.TypeId);
                var action = Context.UserAction.First(ua => ua.Id == workflow.Aid);
                ret.Add(new WorkflowResponse
                {
                    Id = workflow.Id,
                    CurrentState = workflow.CurrentState,
                    Description = workflow.Description,
                    TypeId = workflow.TypeId,
                    Aid = workflow.Aid,

                    Type = new WorkflowTypeResponse
                    {
                        Id = workflowType.Id,
                        Name = workflowType.Name,
                        Description = workflowType.Description
                    },
                    Action = new WorkflowActionResponse
                    {
                        Id = action.Id,
                        Username = action.Username,
                        TimeStr = action.Timestamp != null
                            ? new DateTime(action.Timestamp.Value).ToString(CultureInfo.InvariantCulture)
                            : null
                    },
                    WorkItem = new WorkItemResponse
                    {
                        Id = workItem.Id,
                        AssignedUser = workItem.AssignedUser,
                        AssignedRole = workItem.AssignedRole,
                        Data = workItem.Data,
                        DataType = workItem.DataType,
                        Description = workItem.Description,
                        FromState = workItem.FromState,
                        SeqNo = workItem.SeqNo,
                        ToState = workItem.ToState,
                        Trigger = workItem.Trigger,
                        WorkflowId = workItem.WorkflowId,
                        Aid = workItem.Aid.Value,
                    },
                });
            }

            return ret;
        }

        public IList<WorkflowResponse> GetUserWorkflows2(UserSession session)
        {
            var ret = new List<WorkflowResponse>();

            var workflowIds = new List<Guid>();

            var workItems = Context.WorkItem.Where(wi =>
                wi.Workflow.CurrentState >= 0 && ( // the convention: final states have negative number values
                    wi.AssignedUser != null &&
                    wi.AssignedUser == Context.User.First(u => u.Username == session.Username).Id ||
                    wi.AssignedRole != null && wi.AssignedRole == session.Role)
            );

            foreach (var workItem in workItems)
            {
                if (workflowIds.Contains(workItem.WorkflowId)) continue;

                var workflow = Context.Workflow.First(wf => wf.Id == workItem.WorkflowId);

                var workflowType = Context.WorkflowType.First(wt => wt.Id == workflow.TypeId);
                var action = Context.UserAction.First(ua => ua.Id == workflow.Aid);

                workflowIds.Add(workflow.Id);

                ret.Add(new WorkflowResponse
                {
                    Id = workflow.Id,
                    CurrentState = workflow.CurrentState,
                    Description = workflow.Description,
                    TypeId = workflow.TypeId,
                    Aid = workflow.Aid,

                    Type = new WorkflowTypeResponse
                    {
                        Id = workflowType.Id,
                        Name = workflowType.Name,
                        Description = workflowType.Description
                    },
                    Action = new WorkflowActionResponse
                    {
                        Id = action.Id,
                        Username = action.Username,
                        TimeStr = action.Timestamp != null
                            ? new DateTime(action.Timestamp.Value).ToString(CultureInfo.InvariantCulture)
                            : null
                    },
                });
            }

            return ret.OrderBy(wf => Context.UserAction.FirstOrDefault(ua => ua.Id == wf.Aid)?.Timestamp)
                .ToList();
        }

        public IList<WorkflowResponse> GetUserWorkflows(UserSession session)
        {
            // Fetch user first to avoid nested queries
            var user = Context.User.FirstOrDefault(u => u.Username == session.Username);
            if (user == null) return new List<WorkflowResponse>();
            var userId = user.Id;

            // Get distinct workflow IDs in a single query
            var workflowIds = Context.WorkItem
                .Where(wi => wi.Workflow.CurrentState >= 0 &&
                             (wi.AssignedUser == userId ||
                              wi.AssignedRole == session.Role))
                .Select(wi => wi.WorkflowId)
                .Distinct()
                .ToList();

            if (!workflowIds.Any())
                return new List<WorkflowResponse>();

            // Fetch all required data in bulk
            var workflows = Context.Workflow
                .Where(wf => workflowIds.Contains(wf.Id))
                .ToList();

            var typeIds = workflows.Select(w => w.TypeId).Distinct().ToList();
            var actionIds = workflows.Select(w => w.Aid).Distinct().ToList();

            var workflowTypes = Context.WorkflowType
                .Where(wt => typeIds.Contains(wt.Id))
                .ToDictionary(wt => wt.Id);

            var userActions = Context.UserAction
                .Where(ua => actionIds.Contains(ua.Id))
                .ToDictionary(ua => ua.Id);

            // Build responses
            var ret = new List<WorkflowResponse>();
            foreach (var wf in workflows)
            {
                if (!workflowTypes.TryGetValue(wf.TypeId, out var workflowType))continue;
                if (!userActions.TryGetValue(wf.Aid??0, out var action)) continue;

                ret.Add(new WorkflowResponse
                {
                    Id = wf.Id,
                    CurrentState = wf.CurrentState,
                    Description = wf.Description,
                    TypeId = wf.TypeId,
                    Aid = wf.Aid,
                    Type = new WorkflowTypeResponse
                    {
                        Id = workflowType.Id,
                        Name = workflowType.Name,
                        Description = workflowType.Description
                    },
                    Action = new WorkflowActionResponse
                    {
                        Id = action.Id,
                        Username = action.Username,
                        TimeStr = action.Timestamp.HasValue
                            ? new DateTime(action.Timestamp.Value).ToString(CultureInfo.InvariantCulture)
                            : null
                    }
                });
            }

            // Order by action timestamp
            return ret
                .OrderBy(wf =>
                    userActions.TryGetValue(wf.Aid??0, out var action)
                        ? action.Timestamp ?? long.MaxValue
                        : long.MaxValue)
                .ToList();
        }

        public WorkItem CreateWorkItem(WorkItemRequest request)
        {
            var workItem = new WorkItem
            {
                Id = request.Id ?? Guid.NewGuid(),
                WorkflowId = request.WorkflowId.ToGuid(),
                SeqNo = GetMaxSeq(request.WorkflowId.ToGuid()) + 1,
                FromState = request.FromState,
                ToState = request.ToState,
                Trigger = request.Trigger,
                DataType = request.DataType,
                Data = request.Data,
                Description = request.Description,
                AssignedRole = request.AssignedRole,
                AssignedUser = request.AssignedUser
            };

            Context.WorkItem.Add(workItem);

            workItem.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.AddWorkItem).Id;
            Context.WorkItem.Update(workItem);
            Context.SaveChanges();
            return workItem;
        }

        public WorkItem CreateWorkItemChangeState(WorkItemRequest request)
        {
            var workItem = new WorkItem
            {
                Id = request.Id ?? Guid.NewGuid(),
                WorkflowId = request.WorkflowId.ToGuid(),
                SeqNo = GetMaxSeq(request.WorkflowId.ToGuid()) + 1,
                FromState = request.FromState,
                ToState = request.ToState,
                Trigger = request.Trigger,
                DataType = request.DataType,
                Data = request.Data,
                Description = request.Description,
                AssignedRole = request.AssignedRole,
                AssignedUser = request.AssignedUser
            };

            Context.WorkItem.Add(workItem);
            var workflow = Context.Workflow.First(wf => wf.Id == workItem.WorkflowId);
            workflow.CurrentState = workItem.ToState;
            Context.Workflow.Update(workflow);
            workItem.Aid = Context.SaveChanges(_session.Username, (int)UserActionType.AddWorkItem).Id;
            Context.WorkItem.Update(workItem);
            Context.SaveChanges();
            return workItem;
        }

        public WorkItemResponse GetWorkItem(Guid workItemId)
        {
            var workItem = Context.WorkItem.Find(workItemId);

            return new WorkItemResponse
            {
                Id = workItem.Id,
                WorkflowId = workItem.WorkflowId,
                SeqNo = GetMaxSeq(workItem.WorkflowId) + 1,
                FromState = workItem.FromState,
                ToState = workItem.ToState,
                Trigger = workItem.Trigger,
                DataType = workItem.DataType,
                Data = workItem.Data != null ? JsonConvert.DeserializeObject(workItem.Data) : null,
                Description = workItem.Description,
                AssignedRole = workItem.AssignedRole,
                AssignedUser = workItem.AssignedUser
            };
        }

        public IList<WorkItemResponse> GetWorkItems(Guid workflowId)
        {
            var workItems = Context.WorkItem.Where(wi => wi.WorkflowId == workflowId)
                .OrderBy(wi => wi.SeqNo);

            return workItems.Select(workItem => new WorkItemResponse
                {
                    Id = workItem.Id,
                    WorkflowId = workItem.WorkflowId,
                    SeqNo = GetMaxSeq(workItem.WorkflowId) + 1,
                    FromState = workItem.FromState,
                    ToState = workItem.ToState,
                    Trigger = workItem.Trigger,
                    DataType = workItem.DataType,
                    Data = workItem.Data != null ? JsonConvert.DeserializeObject(workItem.Data) : null,
                    Description = workItem.Description,
                    AssignedRole = workItem.AssignedRole,
                    AssignedUser = workItem.AssignedUser
                })
                .ToList();
        }

        public WorkItemResponse GetLastWorkItem(Guid workflowId)
        {
            var workItem = Context.WorkItem.Where(wi => wi.WorkflowId == workflowId)
                .OrderBy(wi => wi.SeqNo).LastOrDefault();

            if (workItem == null) return null;

            return new WorkItemResponse
            {
                Id = workItem.Id,
                WorkflowId = workItem.WorkflowId,
                SeqNo = GetMaxSeq(workItem.WorkflowId) + 1,
                FromState = workItem.FromState,
                ToState = workItem.ToState,
                Trigger = workItem.Trigger,
                DataType = workItem.DataType,
                Data = workItem.Data != null ? JsonConvert.DeserializeObject(workItem.Data) : null,
                Description = workItem.Description,
                AssignedRole = workItem.AssignedRole,
                AssignedUser = workItem.AssignedUser
            };
        }


        public int GetWorkflowState(Guid workItemId)
        {
            return Context.Workflow.First(wf => wf.Id == workItemId).CurrentState;
        }

        public Guid GetWorkflowId(Guid workItemId)
        {
            return Context.WorkItem.First(wi => wi.Id == workItemId).WorkflowId;
        }


        private int GetMaxSeq(Guid id)
        {
            var seqs = Context.WorkItem.Where(wi => wi.WorkflowId == id);

            return !seqs.Any() ? 0 : seqs.Select(wi => wi.SeqNo).Max();
        }

        public WorkItemResponse GetLastWorkItem<T>(Guid wfid)
        {
            var workItem = Context.WorkItem
                .Where(wi => wi.WorkflowId == wfid && wi.DataType.Equals(typeof(T).ToString()))
                .OrderBy(wi => wi.SeqNo).LastOrDefault();

            if (workItem == null) return null;

            return new WorkItemResponse
            {
                Id = workItem.Id,
                WorkflowId = workItem.WorkflowId,
                SeqNo = GetMaxSeq(workItem.WorkflowId) + 1,
                FromState = workItem.FromState,
                ToState = workItem.ToState,
                Trigger = workItem.Trigger,
                DataType = workItem.DataType,
                Data = workItem.Data != null ? (object)JsonConvert.DeserializeObject<T>(workItem.Data) : null,
                Description = workItem.Description,
                AssignedRole = workItem.AssignedRole,
                AssignedUser = workItem.AssignedUser
            };
        }

        public List<Workflow> GetWorkflows(int type, int state)
        {
            return Context.Workflow.Where(x => x.TypeId == type && x.CurrentState == state).ToList();
        }
    }
}