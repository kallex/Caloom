 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

			using BL=ProjectStatusReporting.BusinessLogic;
		using MVC=ProjectStatusReporting.MVCModel;
		namespace ProjectStatusReporting.MVCModel.MappingToBL { 
		        public static partial class MapSupport
        {
            public static BL.ProjectReportDemoCtx ActiveCtx
            {
                get
                {
                    HttpContext httpContext = HttpContext.Current;
                    const string ctxName = "ProjectReportDemoCtx";
                    var currCtx = (BL.ProjectReportDemoCtx)httpContext.Items[ctxName];
                    if (currCtx == null)
                    {
                        currCtx = new BL.ProjectReportDemoCtx();
                        httpContext.Items.Add(ctxName, currCtx);
                    }
                    return currCtx;
                }
            }
        }
				public static partial class MapProject
		{
			public static List<BL.Project> MapMvcToBusinessList(IEnumerable<MVC.Project>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.Project> MapBusinessToMvcList(IEnumerable<BL.Project> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.Project MapMvcToBusiness(MVC.Project source)
			{
				if(source == null)
					return null;
				BL.Project target = BL.Project.GetOrCreate(MapSupport.ActiveCtx, source.ProjectId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectVersion;
				target.Name = source.ProjectName;
				target.StatusTrackingUrl = source.ProjectStatusTrackingUrl;
				target.TrackingUpdateTime = source.ProjectTrackingUpdateTime;
				target.StatusData = source.ProjectStatusData;
							target.ProjectInfo = MapProjectInfo.MapMvcToBusiness(source.ProjectInfo);
					target.ProjectStatusInfo = MapProjectStatusInfo.MapMvcToBusiness(source.ProjectStatusInfo);
	
				}
				return target;
			}
			
			public static MVC.Project MapBusinessToMvc(BL.Project source)
			{
				if(source == null)
					return null;
				MVC.Project target = new MVC.Project();
					target.ProjectId = source.Id;
				target.ProjectVersion = source.Version;
				target.ProjectName = source.Name;
				target.ProjectStatusTrackingUrl = source.StatusTrackingUrl;
				target.ProjectTrackingUpdateTime = source.TrackingUpdateTime;
				target.ProjectStatusData = source.StatusData;
							target.ProjectInfo = MapProjectInfo.MapBusinessToMvc(source.ProjectInfo);
					target.ProjectStatusInfo = MapProjectStatusInfo.MapBusinessToMvc(source.ProjectStatusInfo);
	

				return target;
			}
				}
		
				public static partial class MapProjectInfo
		{
			public static List<BL.ProjectInfo> MapMvcToBusinessList(IEnumerable<MVC.ProjectInfo>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectInfo> MapBusinessToMvcList(IEnumerable<BL.ProjectInfo> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectInfo MapMvcToBusiness(MVC.ProjectInfo source)
			{
				if(source == null)
					return null;
				BL.ProjectInfo target = BL.ProjectInfo.GetOrCreate(MapSupport.ActiveCtx, source.ProjectInfoId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectInfoVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectInfoIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectInfoVersion;
				target.Name = source.ProjectInfoName;
				target.ResponsibleUnitName = source.ProjectInfoResponsibleUnitName;
				target.ResponsiblePersonName = source.ProjectInfoResponsiblePersonName;
				target.FocusedYear = source.ProjectInfoFocusedYear;
				target.FocusedYearBudgetEuros = source.ProjectInfoFocusedYearBudgetEuros;
				target.FocusedYearEstimateEuros = source.ProjectInfoFocusedYearEstimateEuros;
				target.CommittedBudgetForNextYear = source.ProjectInfoCommittedBudgetForNextYear;
				target.WholeDurationRange = source.ProjectInfoWholeDurationRange;
			
				}
				return target;
			}
			
			public static MVC.ProjectInfo MapBusinessToMvc(BL.ProjectInfo source)
			{
				if(source == null)
					return null;
				MVC.ProjectInfo target = new MVC.ProjectInfo();
					target.ProjectInfoId = source.Id;
				target.ProjectInfoVersion = source.Version;
				target.ProjectInfoName = source.Name;
				target.ProjectInfoResponsibleUnitName = source.ResponsibleUnitName;
				target.ProjectInfoResponsiblePersonName = source.ResponsiblePersonName;
				target.ProjectInfoFocusedYear = source.FocusedYear;
				target.ProjectInfoFocusedYearBudgetEuros = source.FocusedYearBudgetEuros;
				target.ProjectInfoFocusedYearEstimateEuros = source.FocusedYearEstimateEuros;
				target.ProjectInfoCommittedBudgetForNextYear = source.CommittedBudgetForNextYear;
				target.ProjectInfoWholeDurationRange = source.WholeDurationRange;
			

				return target;
			}
				}
		
				public static partial class MapProjectStatusInfo
		{
			public static List<BL.ProjectStatusInfo> MapMvcToBusinessList(IEnumerable<MVC.ProjectStatusInfo>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectStatusInfo> MapBusinessToMvcList(IEnumerable<BL.ProjectStatusInfo> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectStatusInfo MapMvcToBusiness(MVC.ProjectStatusInfo source)
			{
				if(source == null)
					return null;
				BL.ProjectStatusInfo target = BL.ProjectStatusInfo.GetOrCreate(MapSupport.ActiveCtx, source.ProjectStatusInfoId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectStatusInfoVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectStatusInfoIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectStatusInfoVersion;
				target.Owner = source.ProjectStatusInfoOwner;
				target.Updated = source.ProjectStatusInfoUpdated;
				target.Goal = source.ProjectStatusInfoGoal;
							target.Measurements = MapProjectMeasurement.MapMvcToBusinessList(source.Measurements);
					target.Actions = MapProjectAction.MapMvcToBusinessList(source.Actions);
					target.InterestGroups = MapInterestGroup.MapMvcToBusinessList(source.InterestGroups);
					target.Results = MapProjectResult.MapMvcToBusinessList(source.Results);
					target.ChallengesAndExceptions = MapProjectChallenge.MapMvcToBusinessList(source.ChallengesAndExceptions);
					target.CounterActions = MapCounterAction.MapMvcToBusinessList(source.CounterActions);
	
				}
				return target;
			}
			
			public static MVC.ProjectStatusInfo MapBusinessToMvc(BL.ProjectStatusInfo source)
			{
				if(source == null)
					return null;
				MVC.ProjectStatusInfo target = new MVC.ProjectStatusInfo();
					target.ProjectStatusInfoId = source.Id;
				target.ProjectStatusInfoVersion = source.Version;
				target.ProjectStatusInfoOwner = source.Owner;
				target.ProjectStatusInfoUpdated = source.Updated;
				target.ProjectStatusInfoGoal = source.Goal;
							target.Measurements = MapProjectMeasurement.MapBusinessToMvcList(source.Measurements);
					target.Actions = MapProjectAction.MapBusinessToMvcList(source.Actions);
					target.InterestGroups = MapInterestGroup.MapBusinessToMvcList(source.InterestGroups);
					target.Results = MapProjectResult.MapBusinessToMvcList(source.Results);
					target.ChallengesAndExceptions = MapProjectChallenge.MapBusinessToMvcList(source.ChallengesAndExceptions);
					target.CounterActions = MapCounterAction.MapBusinessToMvcList(source.CounterActions);
	

				return target;
			}
				}
		
				public static partial class MapProjectMeasurement
		{
			public static List<BL.ProjectMeasurement> MapMvcToBusinessList(IEnumerable<MVC.ProjectMeasurement>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectMeasurement> MapBusinessToMvcList(IEnumerable<BL.ProjectMeasurement> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectMeasurement MapMvcToBusiness(MVC.ProjectMeasurement source)
			{
				if(source == null)
					return null;
				BL.ProjectMeasurement target = BL.ProjectMeasurement.GetOrCreate(MapSupport.ActiveCtx, source.ProjectMeasurementId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectMeasurementVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectMeasurementIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectMeasurementVersion;
				target.Text = source.ProjectMeasurementText;
				target.Status = source.ProjectMeasurementStatus;
			
				}
				return target;
			}
			
			public static MVC.ProjectMeasurement MapBusinessToMvc(BL.ProjectMeasurement source)
			{
				if(source == null)
					return null;
				MVC.ProjectMeasurement target = new MVC.ProjectMeasurement();
					target.ProjectMeasurementId = source.Id;
				target.ProjectMeasurementVersion = source.Version;
				target.ProjectMeasurementText = source.Text;
				target.ProjectMeasurementStatus = source.Status;
			

				return target;
			}
				}
		
				public static partial class MapProjectAction
		{
			public static List<BL.ProjectAction> MapMvcToBusinessList(IEnumerable<MVC.ProjectAction>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectAction> MapBusinessToMvcList(IEnumerable<BL.ProjectAction> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectAction MapMvcToBusiness(MVC.ProjectAction source)
			{
				if(source == null)
					return null;
				BL.ProjectAction target = BL.ProjectAction.GetOrCreate(MapSupport.ActiveCtx, source.ProjectActionId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectActionVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectActionIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectActionVersion;
				target.Text = source.ProjectActionText;
				target.Status = source.ProjectActionStatus;
			
				}
				return target;
			}
			
			public static MVC.ProjectAction MapBusinessToMvc(BL.ProjectAction source)
			{
				if(source == null)
					return null;
				MVC.ProjectAction target = new MVC.ProjectAction();
					target.ProjectActionId = source.Id;
				target.ProjectActionVersion = source.Version;
				target.ProjectActionText = source.Text;
				target.ProjectActionStatus = source.Status;
			

				return target;
			}
				}
		
				public static partial class MapInterestGroup
		{
			public static List<BL.InterestGroup> MapMvcToBusinessList(IEnumerable<MVC.InterestGroup>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.InterestGroup> MapBusinessToMvcList(IEnumerable<BL.InterestGroup> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.InterestGroup MapMvcToBusiness(MVC.InterestGroup source)
			{
				if(source == null)
					return null;
				BL.InterestGroup target = BL.InterestGroup.GetOrCreate(MapSupport.ActiveCtx, source.InterestGroupId);
                if(target.Version != Guid.Empty && target.Version != source.InterestGroupVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.InterestGroupIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.InterestGroupVersion;
				target.Name = source.InterestGroupName;
			
				}
				return target;
			}
			
			public static MVC.InterestGroup MapBusinessToMvc(BL.InterestGroup source)
			{
				if(source == null)
					return null;
				MVC.InterestGroup target = new MVC.InterestGroup();
					target.InterestGroupId = source.Id;
				target.InterestGroupVersion = source.Version;
				target.InterestGroupName = source.Name;
			

				return target;
			}
				}
		
				public static partial class MapProjectResult
		{
			public static List<BL.ProjectResult> MapMvcToBusinessList(IEnumerable<MVC.ProjectResult>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectResult> MapBusinessToMvcList(IEnumerable<BL.ProjectResult> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectResult MapMvcToBusiness(MVC.ProjectResult source)
			{
				if(source == null)
					return null;
				BL.ProjectResult target = BL.ProjectResult.GetOrCreate(MapSupport.ActiveCtx, source.ProjectResultId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectResultVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectResultIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectResultVersion;
				target.Text = source.ProjectResultText;
			
				}
				return target;
			}
			
			public static MVC.ProjectResult MapBusinessToMvc(BL.ProjectResult source)
			{
				if(source == null)
					return null;
				MVC.ProjectResult target = new MVC.ProjectResult();
					target.ProjectResultId = source.Id;
				target.ProjectResultVersion = source.Version;
				target.ProjectResultText = source.Text;
			

				return target;
			}
				}
		
				public static partial class MapProjectChallenge
		{
			public static List<BL.ProjectChallenge> MapMvcToBusinessList(IEnumerable<MVC.ProjectChallenge>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.ProjectChallenge> MapBusinessToMvcList(IEnumerable<BL.ProjectChallenge> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.ProjectChallenge MapMvcToBusiness(MVC.ProjectChallenge source)
			{
				if(source == null)
					return null;
				BL.ProjectChallenge target = BL.ProjectChallenge.GetOrCreate(MapSupport.ActiveCtx, source.ProjectChallengeId);
                if(target.Version != Guid.Empty && target.Version != source.ProjectChallengeVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.ProjectChallengeIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.ProjectChallengeVersion;
				target.Text = source.ProjectChallengeText;
			
				}
				return target;
			}
			
			public static MVC.ProjectChallenge MapBusinessToMvc(BL.ProjectChallenge source)
			{
				if(source == null)
					return null;
				MVC.ProjectChallenge target = new MVC.ProjectChallenge();
					target.ProjectChallengeId = source.Id;
				target.ProjectChallengeVersion = source.Version;
				target.ProjectChallengeText = source.Text;
			

				return target;
			}
				}
		
				public static partial class MapCounterAction
		{
			public static List<BL.CounterAction> MapMvcToBusinessList(IEnumerable<MVC.CounterAction>  sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapMvcToBusiness(source)).Where(source => source != null).ToList();
			}

			public static List<MVC.CounterAction> MapBusinessToMvcList(IEnumerable<BL.CounterAction> sourceList)
			{
				if(sourceList == null)
					return null;
			    return sourceList.Select(source => MapBusinessToMvc(source)).ToList();
			}
		
			public static BL.CounterAction MapMvcToBusiness(MVC.CounterAction source)
			{
				if(source == null)
					return null;
				BL.CounterAction target = BL.CounterAction.GetOrCreate(MapSupport.ActiveCtx, source.CounterActionId);
                if(target.Version != Guid.Empty && target.Version != source.CounterActionVersion)
                {
                    throw new DataException("Concurrency check failed");
                }
                if(source.CounterActionIsDeleted)
                {
                    target.Delete(MapSupport.ActiveCtx);
					return null;
                } else {
					target.Version = source.CounterActionVersion;
				target.Text = source.CounterActionText;
			
				}
				return target;
			}
			
			public static MVC.CounterAction MapBusinessToMvc(BL.CounterAction source)
			{
				if(source == null)
					return null;
				MVC.CounterAction target = new MVC.CounterAction();
					target.CounterActionId = source.Id;
				target.CounterActionVersion = source.Version;
				target.CounterActionText = source.Text;
			

				return target;
			}
				}
		
		 } 