using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Workflows;
using intapscamis.camis.domain.Workflows.Models;
using Newtonsoft.Json;
using Stateless;

namespace intapscamis.camis.domain.Farms.StateMachines;

public class FarmWarningWorkflow : CamisService
{
    public enum States
    {
        Filing = 1,
        Reviewing = 2,
        Rejected = 3,
        Approved = -2,
        Cancelled = -3
    }

    public enum Triggers
    {
        Request = 1,
        Reject = 2,
        Approve = 3,
        Cancel = 4,
    }

    private readonly IFarmsService _service;

    private readonly IWorkflowService _workflowService;
    private readonly ILandBankService _landBankService;

    private StateMachine<States, Triggers> _machine;

    public FarmWarningWorkflow(IFarmsService service, IWorkflowService workflowService,
        ILandBankService landBankService)
    {
        _service = service;
        _workflowService = workflowService;
        _landBankService = landBankService;
    }

    public Workflow Workflow { get; set; }


    public void SetSession(UserSession session)
    {
        _service.SetSession(session);
        _workflowService.SetSession(session);
        _landBankService.SetSession(session);
    }

    public override void SetContext(CamisContext value)
    {
        Context = value;
        _service.SetContext(Context);
        _workflowService.SetContext(Context);
        _landBankService.SetContext(Context);
    }

    public void ConfigureMachine()
    {
        Workflow = _workflowService.CreateWorkflow(new WorkflowRequest
        {
            CurrentState = (int)States.Filing,
            Description = "Farm Contract Warning.",
            TypeId = (int)WorkflowTypes.FarmContractWarning
        });
        _machine = new StateMachine<States, Triggers>(States.Filing);

        DefineStateMachine();
    }

    public void ConfigureMachine(Guid workflowId)
    {
        Workflow = Context.Workflow.First(wf =>
            wf.Id == workflowId && wf.TypeId == (int)WorkflowTypes.FarmContractWarning);
        _machine = new StateMachine<States, Triggers>((States)Workflow.CurrentState);

        DefineStateMachine();
    }

    public void Fire(Guid workflowId, StateMachine<States, Triggers>.TriggerWithParameters<string, long?> trigger,
        string description, long? assignedUser)
    {
        _machine.Fire(trigger, description, assignedUser);
        _workflowService.UpdateWorkflow(workflowId, (int)_machine.State, description);
    }

    public void Fire(Guid workflowId,
        StateMachine<States, Triggers>.TriggerWithParameters<ContractWarningRequest, string, long?> trigger,
        ContractWarningRequest data,
        string description, long? assignedUser)
    {
        _machine.Fire(trigger, data, description, assignedUser);
        _workflowService.UpdateWorkflow(workflowId, (int)_machine.State, description);
    }

    private void OnRequest(ContractWarningRequest data, string description, long? assignedUser,
        StateMachine<States, Triggers>.Transition transition)
    {
        data.wfid = Workflow.Id.ToString();
        ConfigureAndAddWorkItem(UserRoles.FarmSupervisor, data, description, assignedUser, transition);
        _service.SetFarmLock(Guid.Parse(data.FarmId), true);
        if (data.SplitIndex > 0)
        {
            _landBankService.SetSubLandLock(Guid.Parse(data.LandId), data.SplitIndex, true);
        }
        else
        {
            _landBankService.SetLandLock(Guid.Parse(data.LandId), true);
        }

       
    }

    private void OnReject(string description, long? assignedUser,
        StateMachine<States, Triggers>.Transition transition)
    {
        ConfigureAndAddWorkItem(UserRoles.FarmClerk, GetData(), description, assignedUser, transition);
    }

    private void OnCancel(string description, long? assignedUser,
        StateMachine<States, Triggers>.Transition transition)
    {
        var data = GetData();
        ConfigureAndAddWorkItem(null, data, description, assignedUser, transition);
        _service.SetFarmLock(Guid.Parse(data.Id), false);

        if (data.SplitIndex > 0)
        {
            _landBankService.SetSubLandLock(Guid.Parse(data.LandId), data.SplitIndex, false);
        }
        else
        {
            _landBankService.SetLandLock(Guid.Parse(data.LandId), false);
        }
    }

    private void OnApprove(string description, long? assignedUser,
        StateMachine<States, Triggers>.Transition transition)
    {
        var data = GetData();
        ConfigureAndAddWorkItem(null, data, description, assignedUser, transition);
        _service.RegisterContractWarning(data);
        _service.SetFarmLock(Guid.Parse(data.FarmId), false);
        _service.SetFarmStatus(Guid.Parse(data.FarmId),(int)FarmStatusEnum.UnderWarning);
        if (data.SplitIndex > 0)
        {
            _landBankService.SetSubLandLock(Guid.Parse(data.LandId), data.SplitIndex, false);
        }
        else
        {
            _landBankService.SetLandLock(Guid.Parse(data.LandId), false);
        }
    }

    private ContractWarningRequest GetData()
    {
        var workItem = Context.WorkItem.Where(wi => wi.WorkflowId == Workflow.Id).OrderBy(wi => wi.SeqNo)
            .LastOrDefault();

        return workItem != null ? JsonConvert.DeserializeObject<ContractWarningRequest>(workItem.Data) : null;
    }

    private void DefineStateMachine()
    {
        ParameterizedTriggers.ConfigureParameters(_machine);

        _machine.Configure(States.Filing)
            .Permit(Triggers.Request, States.Reviewing);

        _machine.Configure(States.Reviewing)
            .OnEntryFrom(ParameterizedTriggers.Request, OnRequest)
            .Permit(Triggers.Reject, States.Rejected)
            .Permit(Triggers.Approve, States.Approved);

        _machine.Configure(States.Rejected)
            .OnEntryFrom(ParameterizedTriggers.Reject, OnReject)
            .Permit(Triggers.Request, States.Reviewing)
            .Permit(Triggers.Cancel, States.Cancelled);

        _machine.Configure(States.Approved)
            .OnEntryFrom(ParameterizedTriggers.Approve, OnApprove);
        _machine.Configure(States.Cancelled)
            .OnEntryFrom(ParameterizedTriggers.Cancel, OnCancel);
    }

    private void ConfigureAndAddWorkItem(long? role, ContractWarningRequest data, string description,
        long? assignedUser,
        StateMachine<States, Triggers>.Transition transition)
    {
        var workItemId = Guid.NewGuid();
        var fileDirectory = Context.SysConfigs.First(e => e.Name.Equals("file_directory")).Value ??
                            "/usr/bin/CAMIS/data/docs";
        var fileSavePath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory, workItemId.ToString());
        if (!Directory.Exists(fileSavePath))
        {
            Directory.CreateDirectory(fileSavePath);
        }


        if (data?.SupportiveDocument != null)
        {
            const string pathPrefix = "/api/Farms/InWorkItemContractWarningDoc/";

            foreach (var doc in data.SupportiveDocument)
            {
                if (doc == null) continue;

                if (doc.OverrideFilePath != null &&
                    doc.OverrideFilePath.Contains(pathPrefix))
                {
                    var previousWorkItem = _workflowService.GetLastWorkItem(Workflow.Id);
                    if (previousWorkItem != null)
                    {
                        var previousPath = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                            previousWorkItem.Id.ToString());
                        var sourceFilePath = Path.Combine(previousPath, $"{doc.Id}");
                        var destFilePath = Path.Combine(fileSavePath, $"{doc.Id}");

                        if (File.Exists(sourceFilePath))
                        {
                            File.Copy(sourceFilePath, destFilePath, true);
                        }
                    }
                }
                else if (doc.File != null)
                {
                    doc.Id = doc.Id ?? Guid.NewGuid();
                    var filePath = Path.Combine(fileSavePath, $"{doc.Id}");

                    var fileBytes = Convert.FromBase64String(doc.File);
                    File.WriteAllBytes(filePath, fileBytes);
                    doc.File = null;
                }
                else
                {
                    if (doc.Id != null)
                    {
                        var filePath2 = $"{doc.Id.ToString()}";
                        if (!Path.IsPathRooted(filePath2))
                        {
                            filePath2 = Path.Combine(Directory.GetCurrentDirectory(), fileDirectory,
                                Path.GetFileName(filePath2));
                        }

                        if (File.Exists(filePath2))
                        {
                            var fileBytes2 = File.ReadAllBytes(filePath2);
                            doc.File = Convert.ToBase64String(fileBytes2);
                        }
                    }

                    doc.Id = doc.Id ?? Guid.NewGuid();
                    var filePath = Path.Combine(fileSavePath, $"{doc.Id}");

                    var fileBytes = Convert.FromBase64String(doc.File);
                    File.WriteAllBytes(filePath, fileBytes);
                    doc.File = null;
                }

                doc.OverrideFilePath = $"{pathPrefix}{workItemId}?documentId={doc.Id}";
            }
        }

        _workflowService.CreateWorkItem(new WorkItemRequest
        {
            Id = workItemId,
            WorkflowId = Workflow.Id.ToString(),
            FromState = (int)transition.Source,
            ToState = (int)transition.Destination,
            Trigger = (int)transition.Trigger,
            DataType = typeof(ContractWarningRequest).ToString(),
            Data = data != null ? JsonConvert.SerializeObject(data) : null,
            Description = description,
            AssignedRole = role,
            AssignedUser = assignedUser
        });
    }

    public static class ParameterizedTriggers
    {
        public static StateMachine<States, Triggers>.TriggerWithParameters<ContractWarningRequest, string, long?> Request;

        public static StateMachine<States, Triggers>.
            TriggerWithParameters<string, long?> Reject;

        public static StateMachine<States, Triggers>.
            TriggerWithParameters<string, long?> Approve;

        public static StateMachine<States, Triggers>.
            TriggerWithParameters<string, long?> Cancel;

        public static void ConfigureParameters(
            StateMachine<States, Triggers> machine)
        {
            Request = machine.SetTriggerParameters<ContractWarningRequest, string, long?>(
                Triggers.Request);
            Reject = machine.SetTriggerParameters<string, long?>(Triggers.Reject);
            Approve = machine.SetTriggerParameters<string, long?>(Triggers.Approve);
            Cancel = machine.SetTriggerParameters<string, long?>(Triggers.Cancel);
        }
    }
}