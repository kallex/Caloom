 
using System;
using System.Collections.Generic;

	namespace ProjectStatusReporting.MVCModel { 
				public partial class Project
		{
			    private Guid _projectId = Guid.NewGuid();
	    public Guid ProjectId 
	    {
	        get { return _projectId; }
	        set { _projectId = value; }
	    }
			    private Guid _projectVersion = Guid.NewGuid();
	    public Guid ProjectVersion 
	    {
	        get { return _projectVersion; }
	        set { _projectVersion = value; }
	    }
			    private bool _projectIsDeleted;
	    public bool ProjectIsDeleted 
	    {
	        get { return _projectIsDeleted; }
	        set { _projectIsDeleted = value; }
	    }
			    private string _projectName;
	    public string ProjectName 
	    {
	        get { return _projectName; }
	        set { _projectName = value; }
	    }
			    private string _projectStatusTrackingUrl;
	    public string ProjectStatusTrackingUrl 
	    {
	        get { return _projectStatusTrackingUrl; }
	        set { _projectStatusTrackingUrl = value; }
	    }
			    private DateTime? _projectTrackingUpdateTime;
	    public DateTime? ProjectTrackingUpdateTime 
	    {
	        get { return _projectTrackingUpdateTime; }
	        set { _projectTrackingUpdateTime = value; }
	    }
			    private StatusTracking_v1_0.StatusTrackingAbstractionType _projectStatusData;
	    public StatusTracking_v1_0.StatusTrackingAbstractionType ProjectStatusData 
	    {
	        get { return _projectStatusData; }
	        set { _projectStatusData = value; }
	    }
				}
		
				public partial class ProjectInfo
		{
			    private Guid _projectInfoId = Guid.NewGuid();
	    public Guid ProjectInfoId 
	    {
	        get { return _projectInfoId; }
	        set { _projectInfoId = value; }
	    }
			    private Guid _projectInfoVersion = Guid.NewGuid();
	    public Guid ProjectInfoVersion 
	    {
	        get { return _projectInfoVersion; }
	        set { _projectInfoVersion = value; }
	    }
			    private bool _projectInfoIsDeleted;
	    public bool ProjectInfoIsDeleted 
	    {
	        get { return _projectInfoIsDeleted; }
	        set { _projectInfoIsDeleted = value; }
	    }
			    private string _projectInfoName;
	    public string ProjectInfoName 
	    {
	        get { return _projectInfoName; }
	        set { _projectInfoName = value; }
	    }
			    private string _projectInfoResponsibleUnitName;
	    public string ProjectInfoResponsibleUnitName 
	    {
	        get { return _projectInfoResponsibleUnitName; }
	        set { _projectInfoResponsibleUnitName = value; }
	    }
			    private string _projectInfoResponsiblePersonName;
	    public string ProjectInfoResponsiblePersonName 
	    {
	        get { return _projectInfoResponsiblePersonName; }
	        set { _projectInfoResponsiblePersonName = value; }
	    }
			    private int _projectInfoFocusedYear;
	    public int ProjectInfoFocusedYear 
	    {
	        get { return _projectInfoFocusedYear; }
	        set { _projectInfoFocusedYear = value; }
	    }
			    private decimal _projectInfoFocusedYearBudgetEuros;
	    public decimal ProjectInfoFocusedYearBudgetEuros 
	    {
	        get { return _projectInfoFocusedYearBudgetEuros; }
	        set { _projectInfoFocusedYearBudgetEuros = value; }
	    }
			    private decimal _projectInfoFocusedYearEstimateEuros;
	    public decimal ProjectInfoFocusedYearEstimateEuros 
	    {
	        get { return _projectInfoFocusedYearEstimateEuros; }
	        set { _projectInfoFocusedYearEstimateEuros = value; }
	    }
			    private decimal _projectInfoCommittedBudgetForNextYear;
	    public decimal ProjectInfoCommittedBudgetForNextYear 
	    {
	        get { return _projectInfoCommittedBudgetForNextYear; }
	        set { _projectInfoCommittedBudgetForNextYear = value; }
	    }
			    private string _projectInfoWholeDurationRange;
	    public string ProjectInfoWholeDurationRange 
	    {
	        get { return _projectInfoWholeDurationRange; }
	        set { _projectInfoWholeDurationRange = value; }
	    }
				}
		
				partial class Project
		{
			    private ProjectInfo _projectInfo;
	    public ProjectInfo ProjectInfo 
	    {
	        get { return _projectInfo; }
	        set { _projectInfo = value; }
	    }
				}
				partial class ProjectInfo
		{
			    private Project _parentProject;
	    public Project ParentProject 
	    {
	        get { return _parentProject; }
	        set { _parentProject = value; }
	    }
				}
				public partial class ProjectStatusInfo
		{
			    private Guid _projectStatusInfoId = Guid.NewGuid();
	    public Guid ProjectStatusInfoId 
	    {
	        get { return _projectStatusInfoId; }
	        set { _projectStatusInfoId = value; }
	    }
			    private Guid _projectStatusInfoVersion = Guid.NewGuid();
	    public Guid ProjectStatusInfoVersion 
	    {
	        get { return _projectStatusInfoVersion; }
	        set { _projectStatusInfoVersion = value; }
	    }
			    private bool _projectStatusInfoIsDeleted;
	    public bool ProjectStatusInfoIsDeleted 
	    {
	        get { return _projectStatusInfoIsDeleted; }
	        set { _projectStatusInfoIsDeleted = value; }
	    }
			    private string _projectStatusInfoOwner;
	    public string ProjectStatusInfoOwner 
	    {
	        get { return _projectStatusInfoOwner; }
	        set { _projectStatusInfoOwner = value; }
	    }
			    private DateTime _projectStatusInfoUpdated;
	    public DateTime ProjectStatusInfoUpdated 
	    {
	        get { return _projectStatusInfoUpdated; }
	        set { _projectStatusInfoUpdated = value; }
	    }
			    private string _projectStatusInfoGoal;
	    public string ProjectStatusInfoGoal 
	    {
	        get { return _projectStatusInfoGoal; }
	        set { _projectStatusInfoGoal = value; }
	    }
				}
		
				public partial class ProjectMeasurement
		{
			    private Guid _projectMeasurementId = Guid.NewGuid();
	    public Guid ProjectMeasurementId 
	    {
	        get { return _projectMeasurementId; }
	        set { _projectMeasurementId = value; }
	    }
			    private Guid _projectMeasurementVersion = Guid.NewGuid();
	    public Guid ProjectMeasurementVersion 
	    {
	        get { return _projectMeasurementVersion; }
	        set { _projectMeasurementVersion = value; }
	    }
			    private bool _projectMeasurementIsDeleted;
	    public bool ProjectMeasurementIsDeleted 
	    {
	        get { return _projectMeasurementIsDeleted; }
	        set { _projectMeasurementIsDeleted = value; }
	    }
			    private string _projectMeasurementText;
	    public string ProjectMeasurementText 
	    {
	        get { return _projectMeasurementText; }
	        set { _projectMeasurementText = value; }
	    }
			    private int _projectMeasurementStatus;
	    public int ProjectMeasurementStatus 
	    {
	        get { return _projectMeasurementStatus; }
	        set { _projectMeasurementStatus = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<ProjectMeasurement> _measurements = new List<ProjectMeasurement>();
	    public List<ProjectMeasurement> Measurements 
	    {
	        get { return _measurements; }
	        set { _measurements = value; }
	    }
				}
				partial class ProjectMeasurement
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				public partial class ProjectAction
		{
			    private Guid _projectActionId = Guid.NewGuid();
	    public Guid ProjectActionId 
	    {
	        get { return _projectActionId; }
	        set { _projectActionId = value; }
	    }
			    private Guid _projectActionVersion = Guid.NewGuid();
	    public Guid ProjectActionVersion 
	    {
	        get { return _projectActionVersion; }
	        set { _projectActionVersion = value; }
	    }
			    private bool _projectActionIsDeleted;
	    public bool ProjectActionIsDeleted 
	    {
	        get { return _projectActionIsDeleted; }
	        set { _projectActionIsDeleted = value; }
	    }
			    private string _projectActionText;
	    public string ProjectActionText 
	    {
	        get { return _projectActionText; }
	        set { _projectActionText = value; }
	    }
			    private int _projectActionStatus;
	    public int ProjectActionStatus 
	    {
	        get { return _projectActionStatus; }
	        set { _projectActionStatus = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<ProjectAction> _actions = new List<ProjectAction>();
	    public List<ProjectAction> Actions 
	    {
	        get { return _actions; }
	        set { _actions = value; }
	    }
				}
				partial class ProjectAction
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				public partial class InterestGroup
		{
			    private Guid _interestGroupId = Guid.NewGuid();
	    public Guid InterestGroupId 
	    {
	        get { return _interestGroupId; }
	        set { _interestGroupId = value; }
	    }
			    private Guid _interestGroupVersion = Guid.NewGuid();
	    public Guid InterestGroupVersion 
	    {
	        get { return _interestGroupVersion; }
	        set { _interestGroupVersion = value; }
	    }
			    private bool _interestGroupIsDeleted;
	    public bool InterestGroupIsDeleted 
	    {
	        get { return _interestGroupIsDeleted; }
	        set { _interestGroupIsDeleted = value; }
	    }
			    private string _interestGroupName;
	    public string InterestGroupName 
	    {
	        get { return _interestGroupName; }
	        set { _interestGroupName = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<InterestGroup> _interestGroups = new List<InterestGroup>();
	    public List<InterestGroup> InterestGroups 
	    {
	        get { return _interestGroups; }
	        set { _interestGroups = value; }
	    }
				}
				partial class InterestGroup
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				public partial class ProjectResult
		{
			    private Guid _projectResultId = Guid.NewGuid();
	    public Guid ProjectResultId 
	    {
	        get { return _projectResultId; }
	        set { _projectResultId = value; }
	    }
			    private Guid _projectResultVersion = Guid.NewGuid();
	    public Guid ProjectResultVersion 
	    {
	        get { return _projectResultVersion; }
	        set { _projectResultVersion = value; }
	    }
			    private bool _projectResultIsDeleted;
	    public bool ProjectResultIsDeleted 
	    {
	        get { return _projectResultIsDeleted; }
	        set { _projectResultIsDeleted = value; }
	    }
			    private string _projectResultText;
	    public string ProjectResultText 
	    {
	        get { return _projectResultText; }
	        set { _projectResultText = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<ProjectResult> _results = new List<ProjectResult>();
	    public List<ProjectResult> Results 
	    {
	        get { return _results; }
	        set { _results = value; }
	    }
				}
				partial class ProjectResult
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				public partial class ProjectChallenge
		{
			    private Guid _projectChallengeId = Guid.NewGuid();
	    public Guid ProjectChallengeId 
	    {
	        get { return _projectChallengeId; }
	        set { _projectChallengeId = value; }
	    }
			    private Guid _projectChallengeVersion = Guid.NewGuid();
	    public Guid ProjectChallengeVersion 
	    {
	        get { return _projectChallengeVersion; }
	        set { _projectChallengeVersion = value; }
	    }
			    private bool _projectChallengeIsDeleted;
	    public bool ProjectChallengeIsDeleted 
	    {
	        get { return _projectChallengeIsDeleted; }
	        set { _projectChallengeIsDeleted = value; }
	    }
			    private string _projectChallengeText;
	    public string ProjectChallengeText 
	    {
	        get { return _projectChallengeText; }
	        set { _projectChallengeText = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<ProjectChallenge> _challengesAndExceptions = new List<ProjectChallenge>();
	    public List<ProjectChallenge> ChallengesAndExceptions 
	    {
	        get { return _challengesAndExceptions; }
	        set { _challengesAndExceptions = value; }
	    }
				}
				partial class ProjectChallenge
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				public partial class CounterAction
		{
			    private Guid _counterActionId = Guid.NewGuid();
	    public Guid CounterActionId 
	    {
	        get { return _counterActionId; }
	        set { _counterActionId = value; }
	    }
			    private Guid _counterActionVersion = Guid.NewGuid();
	    public Guid CounterActionVersion 
	    {
	        get { return _counterActionVersion; }
	        set { _counterActionVersion = value; }
	    }
			    private bool _counterActionIsDeleted;
	    public bool CounterActionIsDeleted 
	    {
	        get { return _counterActionIsDeleted; }
	        set { _counterActionIsDeleted = value; }
	    }
			    private string _counterActionText;
	    public string CounterActionText 
	    {
	        get { return _counterActionText; }
	        set { _counterActionText = value; }
	    }
				}
		
				partial class ProjectStatusInfo
		{
			    private List<CounterAction> _counterActions = new List<CounterAction>();
	    public List<CounterAction> CounterActions 
	    {
	        get { return _counterActions; }
	        set { _counterActions = value; }
	    }
				}
				partial class CounterAction
		{
			    private ProjectStatusInfo _parentProjectStatusInfo;
	    public ProjectStatusInfo ParentProjectStatusInfo 
	    {
	        get { return _parentProjectStatusInfo; }
	        set { _parentProjectStatusInfo = value; }
	    }
				}
				partial class Project
		{
			    private ProjectStatusInfo _projectStatusInfo;
	    public ProjectStatusInfo ProjectStatusInfo 
	    {
	        get { return _projectStatusInfo; }
	        set { _projectStatusInfo = value; }
	    }
				}
				partial class ProjectStatusInfo
		{
			    private Project _parentProject;
	    public Project ParentProject 
	    {
	        get { return _parentProject; }
	        set { _parentProject = value; }
	    }
				}
		 } 