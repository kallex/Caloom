 
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Xml.Serialization;


	namespace ProjectStatusReporting.BusinessLogic { 
			    public class ProjectReportDemoDBInitializer : DropCreateDatabaseIfModelChanges< ProjectReportDemoCtx >
	    {
	        protected override void Seed(ProjectReportDemoCtx context)
	        {
	            base.Seed(context);
	        }
	    }

	    public class ProjectReportDemoCtx : DbContext
	    {
		
			public ProjectReportDemoCtx() : base() 
		    { 
			    this.Configuration.LazyLoadingEnabled = true;
		    } 
		
	        public static void InitializeDB()
	        {
	            Database.SetInitializer(new ProjectReportDemoDBInitializer());
	            var ctx = new ProjectReportDemoCtx();
	            ctx.Database.Initialize(false);
	        }
			
			            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
				            modelBuilder.Entity< ProjectInfo >().HasRequired(child => child.ParentProject);
					modelBuilder.Entity< Project >().HasRequired(parent => parent.ProjectInfo)
				.WithRequiredPrincipal(child => child.ParentProject);
		            modelBuilder.Entity< ProjectStatusInfo >().HasRequired(child => child.ParentProject);
					modelBuilder.Entity< Project >().HasRequired(parent => parent.ProjectStatusInfo)
				.WithRequiredPrincipal(child => child.ParentProject);
		            modelBuilder.Entity< ProjectMeasurement >().HasRequired(child => child.ParentProjectStatusInfo);
		            modelBuilder.Entity< ProjectAction >().HasRequired(child => child.ParentProjectStatusInfo);
		            modelBuilder.Entity< InterestGroup >().HasRequired(child => child.ParentProjectStatusInfo);
		            modelBuilder.Entity< ProjectResult >().HasRequired(child => child.ParentProjectStatusInfo);
		            modelBuilder.Entity< ProjectChallenge >().HasRequired(child => child.ParentProjectStatusInfo);
		            modelBuilder.Entity< CounterAction >().HasRequired(child => child.ParentProjectStatusInfo);
		            }
		
					public DbSet< Project > Project { get; set; }
							public DbSet< ProjectInfo > ProjectInfo { get; set; }
							public DbSet< ProjectStatusInfo > ProjectStatusInfo { get; set; }
							public DbSet< ProjectMeasurement > ProjectMeasurement { get; set; }
							public DbSet< ProjectAction > ProjectAction { get; set; }
							public DbSet< InterestGroup > InterestGroup { get; set; }
							public DbSet< ProjectResult > ProjectResult { get; set; }
							public DbSet< ProjectChallenge > ProjectChallenge { get; set; }
							public DbSet< CounterAction > CounterAction { get; set; }
					    }
		
				public partial class Project
		{
		        public static Project GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            Project result = ctx.Project.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new Project();
                result.Id = id;
                ctx.Project.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.Project.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Name { get; set; }
				public string StatusTrackingUrl { get; set; }
				public DateTime? TrackingUpdateTime { get; set; }
		                [Column("StatusData")]
                public string StatusDataStorage { get; internal set; }
                private XmlSerializer _StatusDataSerializer = new XmlSerializer(typeof(StatusTracking_v1_0.StatusTrackingAbstractionType));
                private StatusTracking_v1_0.StatusTrackingAbstractionType _StatusData;
				[NotMapped]
                public StatusTracking_v1_0.StatusTrackingAbstractionType StatusData
                {
                    get
                    {
                        if(_StatusData == null && StatusDataStorage != null)
                        {
                            StringReader stringReader = new StringReader(StatusDataStorage);
                            _StatusData = (StatusTracking_v1_0.StatusTrackingAbstractionType) _StatusDataSerializer.Deserialize(stringReader);
                        }
                        return _StatusData;
                    }
				    set
                    {
                        if(_StatusData != value)
                        {
                            _StatusData = value;
                            if (_StatusData == null)
                                StatusDataStorage = null;
                            else
                            {
                                StringWriter stringWriter = new StringWriter();
                                _StatusDataSerializer.Serialize(stringWriter, _StatusData);
                                StatusDataStorage = stringWriter.GetStringBuilder().ToString();
                            }
                        }

                    }
                }
				}
		
				public partial class ProjectInfo
		{
		        public static ProjectInfo GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectInfo result = ctx.ProjectInfo.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectInfo();
                result.Id = id;
                ctx.ProjectInfo.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectInfo.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Name { get; set; }
				public string ResponsibleUnitName { get; set; }
				public string ResponsiblePersonName { get; set; }
				public int FocusedYear { get; set; }
				public decimal FocusedYearBudgetEuros { get; set; }
				public decimal FocusedYearEstimateEuros { get; set; }
				public decimal CommittedBudgetForNextYear { get; set; }
				public string WholeDurationRange { get; set; }
				}
		
				partial class Project
		{
			//[Association("Project_ProjectInfo", "ProjectInfo", "ParentProject")]
			//[RelatedTo(RelatedProperty="ParentProject")]
			public virtual ProjectInfo ProjectInfo { get; set; }
		}
				partial class ProjectInfo
		{
			//[Association("Project_ProjectInfo", "ParentProject", "ProjectInfo", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="ProjectInfo")]
			public virtual Project ParentProject { get; set; }
		}
				public partial class ProjectStatusInfo
		{
		        public static ProjectStatusInfo GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectStatusInfo result = ctx.ProjectStatusInfo.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectStatusInfo();
                result.Id = id;
                ctx.ProjectStatusInfo.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectStatusInfo.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Owner { get; set; }
				public DateTime Updated { get; set; }
				public string Goal { get; set; }
				}
		
				public partial class ProjectMeasurement
		{
		        public static ProjectMeasurement GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectMeasurement result = ctx.ProjectMeasurement.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectMeasurement();
                result.Id = id;
                ctx.ProjectMeasurement.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectMeasurement.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Text { get; set; }
				public int Status { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_ProjectMeasurement", "Measurements", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<ProjectMeasurement> Measurements { get; set; }
		}
				partial class ProjectMeasurement
		{
			//[Association("ProjectStatusInfo_ProjectMeasurement", "ParentProjectStatusInfo", "Measurements", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="Measurements")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				public partial class ProjectAction
		{
		        public static ProjectAction GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectAction result = ctx.ProjectAction.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectAction();
                result.Id = id;
                ctx.ProjectAction.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectAction.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Text { get; set; }
				public int Status { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_ProjectAction", "Actions", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<ProjectAction> Actions { get; set; }
		}
				partial class ProjectAction
		{
			//[Association("ProjectStatusInfo_ProjectAction", "ParentProjectStatusInfo", "Actions", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="Actions")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				public partial class InterestGroup
		{
		        public static InterestGroup GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            InterestGroup result = ctx.InterestGroup.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new InterestGroup();
                result.Id = id;
                ctx.InterestGroup.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.InterestGroup.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Name { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_InterestGroup", "InterestGroups", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<InterestGroup> InterestGroups { get; set; }
		}
				partial class InterestGroup
		{
			//[Association("ProjectStatusInfo_InterestGroup", "ParentProjectStatusInfo", "InterestGroups", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="InterestGroups")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				public partial class ProjectResult
		{
		        public static ProjectResult GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectResult result = ctx.ProjectResult.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectResult();
                result.Id = id;
                ctx.ProjectResult.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectResult.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Text { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_ProjectResult", "Results", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<ProjectResult> Results { get; set; }
		}
				partial class ProjectResult
		{
			//[Association("ProjectStatusInfo_ProjectResult", "ParentProjectStatusInfo", "Results", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="Results")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				public partial class ProjectChallenge
		{
		        public static ProjectChallenge GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            ProjectChallenge result = ctx.ProjectChallenge.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new ProjectChallenge();
                result.Id = id;
                ctx.ProjectChallenge.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.ProjectChallenge.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Text { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_ProjectChallenge", "ChallengesAndExceptions", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<ProjectChallenge> ChallengesAndExceptions { get; set; }
		}
				partial class ProjectChallenge
		{
			//[Association("ProjectStatusInfo_ProjectChallenge", "ParentProjectStatusInfo", "ChallengesAndExceptions", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="ChallengesAndExceptions")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				public partial class CounterAction
		{
		        public static CounterAction GetOrCreate(ProjectReportDemoCtx ctx, Guid id)
        {
            CounterAction result = ctx.CounterAction.SingleOrDefault(item => item.Id == id);
            if(result == null)
            {
                result = new CounterAction();
                result.Id = id;
                ctx.CounterAction.Add(result);
            }
            return result;
        }
		
        public void Delete(ProjectReportDemoCtx ctx)
        {
            ctx.CounterAction.Remove(this);
        }
			        [Key]
	        public Guid Id { get; set; }
			
			[ConcurrencyCheckAttribute]
 			public Guid Version { get; set; }
				public string Text { get; set; }
				}
		
				partial class ProjectStatusInfo
		{
			//[Association("ProjectStatusInfo_CounterAction", "CounterActions", "ParentProjectStatusInfo")]
			//[RelatedTo(RelatedProperty="ParentProjectStatusInfo")]
			public virtual ICollection<CounterAction> CounterActions { get; set; }
		}
				partial class CounterAction
		{
			//[Association("ProjectStatusInfo_CounterAction", "ParentProjectStatusInfo", "CounterActions", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="CounterActions")]
			public virtual ProjectStatusInfo ParentProjectStatusInfo { get; set; }
		}
				partial class Project
		{
			//[Association("Project_ProjectStatusInfo", "ProjectStatusInfo", "ParentProject")]
			//[RelatedTo(RelatedProperty="ParentProject")]
			public virtual ProjectStatusInfo ProjectStatusInfo { get; set; }
		}
				partial class ProjectStatusInfo
		{
			//[Association("Project_ProjectStatusInfo", "ParentProject", "ProjectStatusInfo", IsForeignKey = true)]
			//[RelatedTo(RelatedProperty="ProjectStatusInfo")]
			public virtual Project ParentProject { get; set; }
		}
		 } 