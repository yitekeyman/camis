 using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Farms;
using intapscamis.camis.domain.Farms.Models;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.Infrastructure.Architecture;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Report.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.Report
{
    public interface IReportService : ICamisService
    {
        
        void SetSession(UserSession session);
        ActivityPlan GetActivityPlan(Guid id);
        ActivityViewModel GetActivityViewModel(Guid id);
        List<ActivityPlanDetailViewModel> GetActivityPlanDetailViewModel(Guid id);
        PlanViewModel GetPlanViewModel(Guid planId);
        List<Activity> GetChildActivities(Guid id);
        ReportResponseModel GenerateReport(ReportRequestModel Model);

        //List<LandAreaReportModel> GetLandReportOfAllRegions();
        List<InverstmentType> GetAllInvestmentTypes();
        List<MoistureSource> GetMoistureSource();

        IList<FarmOperatorTypeResponse> GetFarmOperatorTypes();
        IList<FarmTypeResponse> GetFarmTypes();
        IList<FarmOperatorOriginResponse> GetFarmOperatorOrigins();

        List<TRegions> GetAllRegions();
        List<TZones> GetZones(string csaregionid);
        List<TWoredas> GetWoredas(string csaworedaid);
        List<Farm> GetAllFarms();

    }

    public partial class ReportService : CamisService, IReportService
    {
        private UserSession _session;
        private IFarmsService _farmsService;
        private IProjectFacade _projFacade;

        public ReportService(CamisContext Context, IFarmsService farmService, IProjectFacade projFacade)
        {
            _farmsService = farmService;
            _farmsService.SetContext(Context);
            _projFacade = projFacade;
            
            base.SetContext(Context);
        }

        public override void SetContext(CamisContext value)
        {
            base.SetContext(value);
        }

        public  void SetSession(UserSession session)
        {
            _session = session;
            _projFacade.SetSession(session);
        }

        public List<InverstmentType> GetAllInvestmentTypes()
        {
            return Context.InverstmentType.ToList();
        }

        public List<MoistureSource> GetMoistureSource()
        {
            return Context.MoistureSource.ToList();
        }

        public ActivityPlan GetActivityPlan(Guid id)
        {
            return Context.ActivityPlan.Find(id);
        }

        public ActivityViewModel GetActivityViewModel(Guid id)
        {
            var activity = Context.Activity.Find(id);
            var Detail = GetActivityPlanDetailViewModel(id);
            return new ActivityViewModel()
            {
                Activity = activity,
                Detail = Detail
            };
        }

        public List<ActivityPlanDetailViewModel> GetActivityPlanDetailViewModel(Guid id)
        {

            var detail = Context.ActivityPlanDetail.Where(m => m.ActivityId == id).ToList();
            List<ActivityPlanDetailViewModel> Model = new List<ActivityPlanDetailViewModel>();

            foreach(var item in detail)
            {
                var variable = Context.ActivityProgressVariable.Find(item.VariableId);
                
                Model.Add(new ActivityPlanDetailViewModel
                {
                    Detail = item,
                    Variable = variable,
                    ProgressMeasuringUnit = Context.ActivityProgressMeasuringUnit.Find(variable.DefaultUnitId),
                    VariableValueList = Context.ActivityVariableValueList.Where(vl => vl.VariableId == variable.Id).ToList(),
                    TargetVariableValueList = Context.ActivityVariableValueList.FirstOrDefault(vl => vl.Value == item.Target),
                });
            }
            return Model;
        }

        public PlanViewModel GetPlanViewModel(Guid planId)
        {
            ActivityPlan plan = GetActivityPlan(planId);
            ActivityViewModel rootActivity = GetActivityViewModel((Guid)plan.RootActivityId);
            List<ActivityViewModel> SubActivites = new List<ActivityViewModel>();
            Stack<Activity> activities = new Stack<Activity>();
            var subActs = Context.Activity.Where(m => m.ParentActivityId == rootActivity.Activity.Id).ToList();
            foreach(var acts in subActs)
            {
                activities.Push(acts);
            }
            while(activities.Count > 0)
            {
                var act = activities.Pop();
                SubActivites.Add(GetActivityViewModel(act.Id));
                var childActivites = GetChildActivities(act.Id);
                if(childActivites.Count > 0)
                {
                    foreach(var child in childActivites)
                    {
                        activities.Push(child);
                    }
                }
            }
            Dictionary<ActivityViewModel, ActivityViewModel[]> Hierarchy = new Dictionary<ActivityViewModel, ActivityViewModel[]>();
            Hierarchy[rootActivity] = SubActivites.Where(m => m.Activity.ParentActivityId == rootActivity.Activity.Id).ToArray();
            foreach (var item in SubActivites)
            {
                Hierarchy[item] = SubActivites.Where(m => m.Activity.ParentActivityId == item.Activity.Id).ToArray();
            }

            return new PlanViewModel
            {
                Plan = plan,
                RootActivity = rootActivity,
                SubActivites = SubActivites,
                Hierarchy = Hierarchy
            };
        }

        public List<Activity> GetChildActivities(Guid id)
        {
            return Context.Activity.Where(m => m.ParentActivityId == id).ToList();
        }

        public IList<FarmOperatorTypeResponse> GetFarmOperatorTypes()
        {
            return _farmsService.GetFarmOperatorTypes();
        }

        public IList<FarmTypeResponse> GetFarmTypes()
        {
            return _farmsService.GetFarmTypes();
        }

        public IList<FarmOperatorOriginResponse> GetFarmOperatorOrigins()
        {
            return _farmsService.GetFarmOperatorOrigins();
        }

        public List<TRegions> GetAllRegions()
        {
            return Context.TRegions.ToList();
        }

        public List<Farm> GetAllFarms()
        {
            var farms =  Context.Farm.Include(m => m.Operator).ToList();
            foreach (var f in farms)
            {
                f.Operator.Farm = null;
            }
            return farms;
        }

        public List<TZones> GetZones(string csaregionid)
        {
            var res = Context.TZones.Where(m => m.Csaregionid == csaregionid).ToList();
            return res;
        }

        public List<TWoredas> GetWoredas(string csaworedaid)
        {
            var res = Context.TWoredas.Where(m => m.NrlaisZoneid == csaworedaid).ToList();
            return res;
        }

        public ReferenceData GetReferenceData()
        {
            ReferenceData data = new ReferenceData();
            data.InvestmentTypes = GetAllInvestmentTypes();
            data.MoistureSources = GetMoistureSource();
            data.FarmOperatorTypes = GetFarmOperatorTypes();
            data.FarmTypes = GetFarmTypes();
            data.FarmOperatorOrigins = GetFarmOperatorOrigins();
            data.SurfaceWaterType = Context.SurfaceWaterType.ToList();
            data.GroundWaterType = Context.GroundWater.ToList();

            return data;
        }

        public Dictionary<string,string> GetLocationNames(List<string> locations,out string type)
        {
            locations = locations.Distinct().ToList();
            Dictionary<string, string> lookup = new Dictionary<string, string>();
            type = "Region";
            if (locations.Count() > 0)
            {
                var len = locations[0].Length;
                if(len == 2)
                {
                    var val = Context.TRegions.Where(m => locations.Contains(m.Csaregionid)).ToList();
                    var res = val.ToDictionary(m => m.Csaregionid, m => m.Csaregionnameeng);
                    type = "Region";
                    return res;
                }
                else if(len == 5 || len == 4)
                {
                    var val = Context.TZones.Where(m => locations.Contains(m.NrlaisZoneid)).ToList();
                    var res = val.ToDictionary(m => m.NrlaisZoneid, m => m.Csazonenameeng);
                    type = "Zone";
                    return res;
                }
                else if(len == 6 || len == 8)
                {
                    var val = Context.TWoredas.Where(m => locations.Contains(m.NrlaisWoredaid)).ToList();
                    var res = val.ToDictionary(m => m.NrlaisWoredaid, m => m.Woredanameeng);
                    type = "Woreda";
                    return res;
                }
                return lookup;
            }
            else
                return lookup;
        }
    }
}
