using System;
using System.Linq;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Extensions;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects.Models;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Newtonsoft.Json;
using Stateless;

namespace intapscamis.camis.domain.Projects.Workflows
{
    public enum ProgressReportStates
    {
        Start = -1,
        Requested = 0,
        Accepted = 1,
        Surveying = 2,
        Ready = 3,
        Reviewing = 4,
        Reported = 5,
        Cancelled = -2,
        Approved = -3
    }

    public enum ProgressReportTriggers
    {
        Request = 1,
        Accept = 2,
        Survey = 3,
        Surveyed = 4,
        Encode = 5,
        Reject = 6,
        Report = 7,
        Cancel = 8,
        Approve = 9,
        Submit = 10,
    }
    
    public interface IProgressReportWorkflow : ICamisWorkflow
    {
        Workflow Workflow { get; }
        
        void SetSession(UserSession session);
        
        void Fire(Guid workflowId, StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> trigger,
            string description, long? assignedUser);

        void Fire(Guid workflowId,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> trigger,
            ActivityPlanRequest data, string description, long? assignedUser);
    }
    
    public class ProgressReportWorkflow : CamisWorkflow, IProgressReportWorkflow
    {
        private UserSession _session;
        
        private readonly IProjectService _service;
        private readonly IWorkflowService _workflowService;

        public ProgressReportWorkflow(IProjectService service, IWorkflowService workflowService)
        {
            _service = service;
            _workflowService = workflowService;
        }

        public void SetSession(UserSession session)
        {
            _session = session;
            _service.SetSession(session);
            _workflowService.SetSession(session);
        }

        public override void SetContext(CamisContext context)
        {
            base.SetContext(context);
            _service.SetContext(Context);
            _workflowService.SetContext(Context);
        }


        public Workflow Workflow { get; private set; }
        private StateMachine<ProgressReportStates, ProgressReportTriggers> _machine;

        // create new workflow
        public override void ConfigureMachine()
        {
            const ProgressReportStates initialState = ProgressReportStates.Start;
            Workflow = _workflowService.CreateWorkflow(new WorkflowRequest
            {
                CurrentState = (int) initialState,
                Description = "New business plan progress report.",
                TypeId = (int) WorkflowTypes.ProgressReport
            });
            _machine = new StateMachine<ProgressReportStates, ProgressReportTriggers>(initialState);

            DefineStateMachine();
        }

        // access existing workflow
        public override void ConfigureMachine(Guid workflowId)
        {
            Workflow = Context.Workflow.First(wf =>
                wf.Id == workflowId && wf.TypeId == (int) WorkflowTypes.ProgressReport);
            _machine = new StateMachine<ProgressReportStates, ProgressReportTriggers>((ProgressReportStates) Workflow.CurrentState);

            DefineStateMachine();
        }

        private void DefineStateMachine()
        {
            ParameterizedTriggers.ConfigureParameters(_machine);

            _machine.Configure(ProgressReportStates.Start)
                .Permit(ProgressReportTriggers.Request, ProgressReportStates.Requested)
                .Permit(ProgressReportTriggers.Submit, ProgressReportStates.Reported);

            _machine.Configure(ProgressReportStates.Requested)
                .OnEntryFrom(ParameterizedTriggers.Request, OnRequest)
                .Permit(ProgressReportTriggers.Cancel, ProgressReportStates.Cancelled)
                .Permit(ProgressReportTriggers.Accept, ProgressReportStates.Accepted);

            _machine.Configure(ProgressReportStates.Accepted)
                .OnEntryFrom(ParameterizedTriggers.Accept, OnAccept)
                .Permit(ProgressReportTriggers.Submit, ProgressReportStates.Reported)
                .Permit(ProgressReportTriggers.Cancel, ProgressReportStates.Cancelled)
                .Permit(ProgressReportTriggers.Survey, ProgressReportStates.Surveying);

            _machine.Configure(ProgressReportStates.Surveying)
                .Permit(ProgressReportTriggers.Submit, ProgressReportStates.Reported)
                .OnEntryFrom(ParameterizedTriggers.Survey, OnSurvey)
                .Permit(ProgressReportTriggers.Cancel, ProgressReportStates.Cancelled)
                .Permit(ProgressReportTriggers.Surveyed, ProgressReportStates.Ready);

            _machine.Configure(ProgressReportStates.Ready)
                .Permit(ProgressReportTriggers.Submit, ProgressReportStates.Reported)
                .OnEntryFrom(ParameterizedTriggers.Surveyed, OnSurveyed)
                .OnEntryFrom(ParameterizedTriggers.Reject, OnReject)
                .Permit(ProgressReportTriggers.Encode, ProgressReportStates.Reviewing);

            _machine.Configure(ProgressReportStates.Reviewing)
                .OnEntryFrom(ParameterizedTriggers.Encode, OnEncode)
                .Permit(ProgressReportTriggers.Cancel, ProgressReportStates.Cancelled)
                .Permit(ProgressReportTriggers.Reject, ProgressReportStates.Ready)
                .Permit(ProgressReportTriggers.Report, ProgressReportStates.Reported);

            _machine.Configure(ProgressReportStates.Reported)
                .OnEntryFrom(ParameterizedTriggers.Submit, OnSubmit)
                .OnEntryFrom(ParameterizedTriggers.Report, OnReport)
                .Permit(ProgressReportTriggers.Cancel, ProgressReportStates.Cancelled)
                .Permit(ProgressReportTriggers.Approve, ProgressReportStates.Approved);

            _machine.Configure(ProgressReportStates.Cancelled)
                .OnEntryFrom(ParameterizedTriggers.Cancel, OnCancel);
            
            _machine.Configure(ProgressReportStates.Approved)
                .OnEntryFrom(ParameterizedTriggers.Approve, OnApprove);
        }


        public void Fire(Guid workflowId, StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> trigger,
            string description, long? assignedUser)
        {
            _machine.Fire(trigger, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
        }

        public void Fire(Guid workflowId,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> trigger,
            ActivityPlanRequest data, string description, long? assignedUser)
        {
            _machine.Fire(trigger, data, description, assignedUser);
            _workflowService.UpdateWorkflow(workflowId, (int) _machine.State, description);
        }


        private void OnRequest(ActivityPlanRequest data, string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEExpert, data, description, assignedUser, transition);
        }

        private void OnAccept(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEExpert, GetData(), description, assignedUser, transition);
        }

        private void OnSurvey(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEExpert, GetData(), description, assignedUser, transition);
        }

        private void OnSurveyed(ActivityPlanRequest data, string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEDataEncoder, data, description, assignedUser, transition);
        }

        private void OnEncode(ActivityPlanRequest data, string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEExpert, data, description, assignedUser, transition);
        }

        private void OnReject(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnEDataEncoder, GetData(), description, assignedUser, transition);
        }

        private void OnSubmit(ActivityPlanRequest data, string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnESupervisor, data, description, assignedUser, transition);
        }

        private void OnReport(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(UserRoles.MnESupervisor, GetData(), description, assignedUser, transition);
        }

        private void OnCancel(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            ConfigureAndAddWorkItem(null, GetData(), description, assignedUser, transition);
        }

        private void OnApprove(string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            var data = GetData();

            // the real act
            _service.CreateActivityProgressReport(data, data.RootActivityId.ToGuid());
            
            ConfigureAndAddWorkItem(null, data, description, assignedUser, transition);
        }


        private ActivityPlanRequest GetData()
        {
            var workItem = Context.WorkItem.Where(wi => wi.WorkflowId == Workflow.Id).OrderBy(wi => wi.SeqNo)
                .LastOrDefault();

            return workItem != null ? JsonConvert.DeserializeObject<ActivityPlanRequest>(workItem.Data) : null;
        }

        private void ConfigureAndAddWorkItem(long? role, ActivityPlanRequest data, string description, long? assignedUser,
            StateMachine<ProgressReportStates, ProgressReportTriggers>.Transition transition)
        {
            var workItemId = Guid.NewGuid();
            
            // tag each (ActivityPlanRequest data).ReportDocuments 
            if (data?.ReportDocuments != null)
            {
                foreach (var doc in data.ReportDocuments)
                {
                    if (doc == null) continue;
                    
                    const string pathPrefix = "/api/Projects/InWorkItemReportFile/";

                    if (doc.OverrideFilePath != null &&
                        doc.OverrideFilePath.Substring(0, pathPrefix.Length) == pathPrefix)
                    {
                        var lastWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                        if (lastWorkItem != null)
                        {
                            var file = _service
                                .InWorkItemReportFile(lastWorkItem.Id, doc.Id ?? Guid.Empty)
                                .File;
                            if (file != null) doc.File = Convert.ToBase64String(file);
                        }
                    }
                    
                    doc.Id = doc.Id ?? Guid.NewGuid();
                    doc.OverrideFilePath = $"${pathPrefix}{workItemId}?documentId={doc.Id}";
                }
            }
            
            _workflowService.CreateWorkItem(new WorkItemRequest
            {
                Id = workItemId,
                WorkflowId = Workflow.Id.ToString(),
                FromState = (int) transition.Source,
                ToState = (int) transition.Destination,
                Trigger = (int) transition.Trigger,
                DataType = typeof(ActivityPlanRequest).ToString(),
                Data = data != null ? JsonConvert.SerializeObject(data) : null,
                Description = description,
                AssignedRole = role,
                AssignedUser = assignedUser
            });
        }


        public static class ParameterizedTriggers
        {
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> Request;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Accept;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Survey;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> Surveyed;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> Encode;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Reject;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Report;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Cancel;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<string, long?> Approve;
            public static StateMachine<ProgressReportStates, ProgressReportTriggers>.TriggerWithParameters<ActivityPlanRequest, string, long?> Submit;

            public static void ConfigureParameters(StateMachine<ProgressReportStates, ProgressReportTriggers> machine)
            {
                Request = machine.SetTriggerParameters<ActivityPlanRequest, string, long?>(ProgressReportTriggers.Request);
                Accept = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Accept);
                Survey = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Survey);
                Surveyed = machine.SetTriggerParameters<ActivityPlanRequest, string, long?>(ProgressReportTriggers.Surveyed);
                Encode = machine.SetTriggerParameters<ActivityPlanRequest, string, long?>(ProgressReportTriggers.Encode);
                Reject = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Reject);
                Report = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Report);
                Cancel = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Cancel);
                Approve = machine.SetTriggerParameters<string, long?>(ProgressReportTriggers.Approve);
                Submit = machine.SetTriggerParameters<ActivityPlanRequest, string, long?>(ProgressReportTriggers.Submit);
            }
        }
    }
}