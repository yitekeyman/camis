using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace intapscamis.camis.data.Entities
{
    public partial class CamisContext : DbContext
    {
        public CamisContext()
        {
        }

        public CamisContext(DbContextOptions<CamisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccessibiltyType> AccessibiltyType { get; set; }
        public virtual DbSet<ActionType> ActionType { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityPlan> ActivityPlan { get; set; }
        public virtual DbSet<ActivityPlanDetail> ActivityPlanDetail { get; set; }
        public virtual DbSet<ActivityPlanDocument> ActivityPlanDocument { get; set; }
        public virtual DbSet<ActivityPlanTemplate> ActivityPlanTemplate { get; set; }
        public virtual DbSet<ActivityProgress> ActivityProgress { get; set; }
        public virtual DbSet<ActivityProgressMeasuringUnit> ActivityProgressMeasuringUnit { get; set; }
        public virtual DbSet<ActivityProgressReport> ActivityProgressReport { get; set; }
        public virtual DbSet<ActivityProgressReportDocument> ActivityProgressReportDocument { get; set; }
        public virtual DbSet<ActivityProgressStatus> ActivityProgressStatus { get; set; }
        public virtual DbSet<ActivityProgressVariable> ActivityProgressVariable { get; set; }
        public virtual DbSet<ActivityProgressVariableType> ActivityProgressVariableType { get; set; }
        public virtual DbSet<ActivitySchedule> ActivitySchedule { get; set; }
        public virtual DbSet<ActivityStatusType> ActivityStatusType { get; set; }
        public virtual DbSet<ActivityVariableValueList> ActivityVariableValueList { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AddressScheme> AddressScheme { get; set; }
        public virtual DbSet<AddressSchemeUnit> AddressSchemeUnit { get; set; }
        public virtual DbSet<AddressUnit> AddressUnit { get; set; }
        public virtual DbSet<AgriculturalZone> AgriculturalZone { get; set; }
        public virtual DbSet<AgroEchology> AgroEchology { get; set; }
        public virtual DbSet<AgroType> AgroType { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Certificate> Certificate { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<Ethiopia> Ethiopia { get; set; }
        public virtual DbSet<Ethiopiaboundary> Ethiopiaboundary { get; set; }
        public virtual DbSet<Ethiopiaregion> Ethiopiaregion { get; set; }
        public virtual DbSet<Ethiopiaworeda> Ethiopiaworeda { get; set; }
        public virtual DbSet<Ethiopiazone> Ethiopiazone { get; set; }
        public virtual DbSet<Ethrivers> Ethrivers { get; set; }
        public virtual DbSet<Ethroad> Ethroad { get; set; }
        public virtual DbSet<Farm> Farm { get; set; }
        public virtual DbSet<FarmLand> FarmLand { get; set; }
        public virtual DbSet<FarmOperator> FarmOperator { get; set; }
        public virtual DbSet<FarmOperatorOrigin> FarmOperatorOrigin { get; set; }
        public virtual DbSet<FarmOperatorRegistration> FarmOperatorRegistration { get; set; }
        public virtual DbSet<FarmOperatorType> FarmOperatorType { get; set; }
        public virtual DbSet<FarmRegistration> FarmRegistration { get; set; }
        public virtual DbSet<FarmType> FarmType { get; set; }
        public virtual DbSet<GroundData> GroundData { get; set; }
        public virtual DbSet<GroundWater> GroundWater { get; set; }
        public virtual DbSet<InverstmentType> InverstmentType { get; set; }
        public virtual DbSet<Irrigation> Irrigation { get; set; }
        public virtual DbSet<Lake> Lake { get; set; }
        public virtual DbSet<Land> Land { get; set; }
        public virtual DbSet<LandAccessibility> LandAccessibility { get; set; }
        public virtual DbSet<LandClimate> LandClimate { get; set; }
        public virtual DbSet<LandDoc> LandDoc { get; set; }
        public virtual DbSet<LandInvestment> LandInvestment { get; set; }
        public virtual DbSet<LandMoisture> LandMoisture { get; set; }
        public virtual DbSet<LandRight> LandRight { get; set; }
        public virtual DbSet<LandSplit> LandSplit { get; set; }
        public virtual DbSet<LandType> LandType { get; set; }
        public virtual DbSet<LandUpin> LandUpin { get; set; }
        public virtual DbSet<LandUsage> LandUsage { get; set; }
        public virtual DbSet<MoistureSource> MoistureSource { get; set; }
        public virtual DbSet<Months> Months { get; set; }
        public virtual DbSet<RegistrationAuthority> RegistrationAuthority { get; set; }
        public virtual DbSet<RegistrationType> RegistrationType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SoilTest> SoilTest { get; set; }
        public virtual DbSet<SoilTestTypes> SoilTestTypes { get; set; }
        public virtual DbSet<SpatialRefSys> SpatialRefSys { get; set; }
        public virtual DbSet<Stream> Stream { get; set; }
        public virtual DbSet<SurfaceWater> SurfaceWater { get; set; }
        public virtual DbSet<SurfaceWaterType> SurfaceWaterType { get; set; }
        public virtual DbSet<Topography> Topography { get; set; }
        public virtual DbSet<TopographyType> TopographyType { get; set; }
        public virtual DbSet<UsageType> UsageType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAction> UserAction { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<WaterSourceType> WaterSourceType { get; set; }
        public virtual DbSet<WaterSrcParam> WaterSrcParam { get; set; }
        public virtual DbSet<WoretaWatershed> WoretaWatershed { get; set; }
        public virtual DbSet<Workflow> Workflow { get; set; }
        public virtual DbSet<WorkflowType> WorkflowType { get; set; }
        public virtual DbSet<WorkItem> WorkItem { get; set; }


        public virtual DbSet<TRegions> TRegions { get; set; }
        public virtual DbSet<TWoredas> TWoredas { get; set; }
        public virtual DbSet<TZones> TZones { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Host=localhost;Database=camis;Username=postgres;Password=admin",o => o.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack")
                .HasPostgresExtension("postgis")
                .HasPostgresExtension("postgis_topology");

            modelBuilder.Entity<AccessibiltyType>(entity =>
            {
                entity.ToTable("accessibilty_type", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.ToTable("action_type", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activity", "pm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.ParentActivityId).HasColumnName("parent_activity_id");

                entity.Property(e => e.Tag).HasColumnName("tag");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.ParentActivity)
                    .WithMany(p => p.InverseParentActivity)
                    .HasForeignKey(d => d.ParentActivityId)
                    .HasConstraintName("activity_activity_id_fk");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("activity_activity_plan_template_id_fk");
            });

            modelBuilder.Entity<ActivityPlan>(entity =>
            {
                entity.ToTable("activity_plan", "pm");

                entity.HasIndex(e => e.RootActivityId)
                    .HasName("ixfk_activity_plan_activity");

                entity.HasIndex(e => e.StatusId)
                    .HasName("activity_plan_status_id_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.RootActivityId).HasColumnName("root_activity_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.HasOne(d => d.RootActivity)
                    .WithMany(p => p.ActivityPlan)
                    .HasForeignKey(d => d.RootActivityId)
                    .HasConstraintName("fk_activity_plan_activity");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ActivityPlan)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("activity_plan_activity_status_type_id_fk");
            });

            modelBuilder.Entity<ActivityPlanDetail>(entity =>
            {
                entity.ToTable("activity_plan_detail", "pm");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("ixfk_activity_plan_detail_activity");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_plan_detail_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.PlanId)
                    .HasName("ixfk_activity_plan_detail_activity_plan");

                entity.HasIndex(e => e.VariableId)
                    .HasName("ixfk_activity_plan_detail_activity_progress_variable");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.CustomVariableName).HasColumnName("custom_variable_name");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.Tag).HasColumnName("tag");

                entity.Property(e => e.Target).HasColumnName("target");

                entity.Property(e => e.VariableId).HasColumnName("variable_id");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityPlanDetail)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_activity_plan_detail_activity");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.ActivityPlanDetail)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("fk_activity_plan_detail_activity_plan");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.ActivityPlanDetail)
                    .HasForeignKey(d => d.VariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_activity_plan_detail_activity_progress_variable");
            });

            modelBuilder.Entity<ActivityPlanDocument>(entity =>
            {
                entity.ToTable("activity_plan_document", "pm");

                entity.HasIndex(e => e.Aid)
                    .HasName("ixfk_activity_plan_document_user_action");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("ixfk_activity_plan_document_document");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_plan_document_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.PlanId)
                    .HasName("ixfk_activity_plan_document_activity_plan");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.ActivityPlanDocument)
                    .HasForeignKey(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_activity_plan_document_document");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.ActivityPlanDocument)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("fk_activity_plan_document_activity_plan");
            });

            modelBuilder.Entity<ActivityPlanTemplate>(entity =>
            {
                entity.ToTable("activity_plan_template", "pm");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_plan_template_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("activity_plan_template_name_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnName("data")
                    .HasColumnType("json");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ActivityProgress>(entity =>
            {
                entity.ToTable("activity_progress", "pm");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("ixfk_activity_progress_activity");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_progress_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ReportId)
                    .HasName("ixfk_activity_progress_activity_progress_report");

                entity.HasIndex(e => e.VariableId)
                    .HasName("ixfk_activity_progress_activity_progress_variable");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.Progress).HasColumnName("progress");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.VariableId).HasColumnName("variable_id");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityProgress)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("fk_activity_progress_activity");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ActivityProgress)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_activity_progress_activity_progress_report");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.ActivityProgress)
                    .HasForeignKey(d => d.VariableId)
                    .HasConstraintName("fk_activity_progress_activity_progress_variable");
            });

            modelBuilder.Entity<ActivityProgressMeasuringUnit>(entity =>
            {
                entity.ToTable("activity_progress_measuring_unit", "pm");

                entity.HasIndex(e => e.ConvertFrom)
                    .HasName("ixfk_activity_progress_measuring_unit_activity_progress_measuri");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ConversionFactor).HasColumnName("conversion_factor");

                entity.Property(e => e.ConversionOffset)
                    .HasColumnName("conversion_offset")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ConvertFrom).HasColumnName("convert_from");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.HasOne(d => d.ConvertFromNavigation)
                    .WithMany(p => p.InverseConvertFromNavigation)
                    .HasForeignKey(d => d.ConvertFrom)
                    .HasConstraintName("fk_activity_progress_measuring_unit_activity_progress_measuring");
            });

            modelBuilder.Entity<ActivityProgressReport>(entity =>
            {
                entity.ToTable("activity_progress_report", "pm");

                entity.HasIndex(e => e.RootActivityId)
                    .HasName("activity_progress_report_root_activity_id_index");

                entity.HasIndex(e => e.StatusId)
                    .HasName("activity_progress_report_status_id_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.ReportTime).HasColumnName("report_time");

                entity.Property(e => e.RootActivityId).HasColumnName("root_activity_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.HasOne(d => d.RootActivity)
                    .WithMany(p => p.ActivityProgressReport)
                    .HasForeignKey(d => d.RootActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activity_progress_report_activity_id_fk");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ActivityProgressReport)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activity_progress_report_activity_status_type_id_fk");
            });

            modelBuilder.Entity<ActivityProgressReportDocument>(entity =>
            {
                entity.ToTable("activity_progress_report_document", "pm");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("ixfk_activity_progress_report_document_document");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_progress_report_document_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ReportId)
                    .HasName("ixfk_activity_progress_report_document_activity_progress_report");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.ActivityProgressReportDocument)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("fk_activity_progress_report_document_document");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ActivityProgressReportDocument)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_activity_progress_report_document_activity_progress_report");
            });

            modelBuilder.Entity<ActivityProgressStatus>(entity =>
            {
                entity.ToTable("activity_progress_status", "pm");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("ixfk_activity_progress_status_activity");

                entity.HasIndex(e => e.ReportId)
                    .HasName("ixfk_activity_progress_status_activity_progress_report");

                entity.HasIndex(e => e.StatusId)
                    .HasName("activity_progress_status_status_id_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.ReportId).HasColumnName("report_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityProgressStatus)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("fk_activity_progress_status_activity");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ActivityProgressStatus)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("fk_activity_progress_status_activity_progress_report");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ActivityProgressStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activity_progress_status_activity_status_type_id_fk");
            });

            modelBuilder.Entity<ActivityProgressVariable>(entity =>
            {
                entity.ToTable("activity_progress_variable", "pm");

                entity.HasIndex(e => e.DefaultUnitId)
                    .HasName("ixfk_activity_progress_variable_activity_progress_measuring_uni");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DefaultUnitId).HasColumnName("default_unit_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.DefaultUnit)
                    .WithMany(p => p.ActivityProgressVariable)
                    .HasForeignKey(d => d.DefaultUnitId)
                    .HasConstraintName("fk_activity_progress_variable_activity_progress_measuring_unit");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.ActivityProgressVariable)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activity_progress_variable_activity_progress_variable_type_id_f");
            });

            modelBuilder.Entity<ActivityProgressVariableType>(entity =>
            {
                entity.ToTable("activity_progress_variable_type", "pm");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_progress_variable_type_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ActivitySchedule>(entity =>
            {
                entity.ToTable("activity_schedule", "pm");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("ixfk_activity_schedule_activity");

                entity.HasIndex(e => e.Id)
                    .HasName("activity_schedule_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.PlanId)
                    .HasName("ixfk_activity_schedule_activity_plan");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.From).HasColumnName("from");

                entity.Property(e => e.PlanId).HasColumnName("plan_id");

                entity.Property(e => e.To).HasColumnName("to");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivitySchedule)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_activity_schedule_activity");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.ActivitySchedule)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("fk_activity_schedule_activity_plan");
            });

            modelBuilder.Entity<ActivityStatusType>(entity =>
            {
                entity.ToTable("activity_status_type", "pm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<ActivityVariableValueList>(entity =>
            {
                entity.HasKey(e => new { e.Order, e.VariableId });

                entity.ToTable("activity_variable_value_list", "pm");

                entity.HasIndex(e => e.VariableId)
                    .HasName("ixfk_activity_variable_value_list_activity_progress_variable");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.VariableId).HasColumnName("variable_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Variable)
                    .WithMany(p => p.ActivityVariableValueList)
                    .HasForeignKey(d => d.VariableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_activity_variable_value_list_activity_progress_variable");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address", "sys");

                entity.HasIndex(e => e.UnitId)
                    .HasName("ixfk_address_address_unit");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_address_address_unit");
            });

            modelBuilder.Entity<AddressScheme>(entity =>
            {
                entity.ToTable("address_scheme", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<AddressSchemeUnit>(entity =>
            {
                entity.ToTable("address_scheme_unit", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.SchemeId).HasColumnName("scheme_id");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.HasOne(d => d.Scheme)
                    .WithMany(p => p.AddressSchemeUnit)
                    .HasForeignKey(d => d.SchemeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("address_scheme_unit_address_scheme_id_fk");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.AddressSchemeUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("address_scheme_unit_address_unit_id_fk");
            });

            modelBuilder.Entity<AddressUnit>(entity =>
            {
                entity.ToTable("address_unit", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Custom)
                    .IsRequired()
                    .HasColumnName("custom")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TRegions>(entity =>
            {
                entity.ToTable("t_regions", "sys");

                entity.HasComment("The table t_regions stores all regions to ensure an stable identification of object in the system");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Csaregionid).HasColumnName("csaregionid");

                entity.Property(e => e.Csaregionnameamharic).HasColumnName("csaregionnameamharic");

                entity.Property(e => e.Csaregionnameeng).HasColumnName("csaregionnameeng");

                entity.Property(e => e.Csaregionnameoromifya).HasColumnName("csaregionnameoromifya");

                entity.Property(e => e.Csaregionnametigrinya).HasColumnName("csaregionnametigrinya");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.Property(e => e.Regioncodeam).HasColumnName("regioncodeam");
            });

            modelBuilder.Entity<TWoredas>(entity =>
            {
                entity.ToTable("t_woredas", "sys");

                entity.HasComment("The table t_woredas stores all woredas to ensure an stable identification of object in the system");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.Property(e => e.NrlaisWoredaid).HasColumnName("nrlais_woredaid");

                entity.Property(e => e.NrlaisZoneid).HasColumnName("nrlais_zoneid");

                entity.Property(e => e.Woredaid).HasColumnName("woredaid");

                entity.Property(e => e.Woredanameamharic).HasColumnName("woredanameamharic");

                entity.Property(e => e.Woredanameeng).HasColumnName("woredanameeng");

                entity.Property(e => e.Woredanameoromifya).HasColumnName("woredanameoromifya");

                entity.Property(e => e.Woredanametigrinya).HasColumnName("woredanametigrinya");
            });

            modelBuilder.Entity<TZones>(entity =>
            {
                entity.ToTable("t_zones", "sys");

                entity.HasComment("The table t_zones stores all zones to ensure an stable identification of object in the system");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Csaregionid).HasColumnName("csaregionid");

                entity.Property(e => e.Csazoneid).HasColumnName("csazoneid");

                entity.Property(e => e.Csazonenameamharic).HasColumnName("csazonenameamharic");

                entity.Property(e => e.Csazonenameeng).HasColumnName("csazonenameeng");

                entity.Property(e => e.Csazonenameoromifya).HasColumnName("csazonenameoromifya");

                entity.Property(e => e.Csazonenametigrinya).HasColumnName("csazonenametigrinya");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.Property(e => e.NrlaisZoneid).HasColumnName("nrlais_zoneid");
            });

            modelBuilder.Entity<AgriculturalZone>(entity =>
            {
                entity.ToTable("agricultural_zone", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("agricultural_zone_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.LandId)
                    .HasName("agricultural_zone_land_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.agricultural_zone_id_seq'::regclass)");

                entity.Property(e => e.IsAgriZone)
                    .IsRequired()
                    .HasColumnName("is_agri_zone")
                    .HasDefaultValueSql("'No'::character varying");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.HasOne(d => d.Land)
                    .WithOne(p => p.AgriculturalZone)
                    .HasForeignKey<AgriculturalZone>(d => d.LandId)
                    .HasConstraintName("agricultural_zone_land_id_fk");
            });

            modelBuilder.Entity<AgroEchology>(entity =>
            {
                entity.ToTable("agro_echology", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("agro_echology_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.agro_echology_id_seq'::regclass)");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Result)
                    .IsRequired()
                    .HasColumnName("result");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.AgroEchology)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("agro_echology_land_id_fk");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.AgroEchology)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("agro_echology_agro_type_id_fk");
            });

            modelBuilder.Entity<AgroType>(entity =>
            {
                entity.ToTable("agro_type", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("audit_log", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('sys.audit_seq'::regclass)");

                entity.Property(e => e.KeyValues).HasColumnName("key_values");

                entity.Property(e => e.NewValues).HasColumnName("new_values");

                entity.Property(e => e.OldValues).HasColumnName("old_values");

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasColumnName("table_name");

                entity.Property(e => e.TimeStamp).HasColumnName("time_stamp");

                entity.Property(e => e.UserAction).HasColumnName("user_action");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasKey(e => e.LandId);

                entity.ToTable("certificate", "lb");

                entity.Property(e => e.LandId)
                    .HasColumnName("land_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Label).HasColumnName("label");

                entity.Property(e => e.WId).HasColumnName("w_id");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("document", "doc");

                entity.HasIndex(e => e.Aid)
                    .HasName("ixfk_document_user_action");

                entity.HasIndex(e => e.Type)
                    .HasName("ixfk_document_document_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.File)
                    .IsRequired()
                    .HasColumnName("file");

                entity.Property(e => e.Filename).HasColumnName("filename");

                entity.Property(e => e.Mimetype)
                    .IsRequired()
                    .HasColumnName("mimetype");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.OverrideFilePath).HasColumnName("override_file_path");

                entity.Property(e => e.Ref)
                    .IsRequired()
                    .HasColumnName("ref");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("fk_document_document_type");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("document_type", "doc");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Ethiopia>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethiopia", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethiopia_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Regionname).HasColumnName("regionname");

                entity.Property(e => e.Regionno).HasColumnName("regionno");
            });

            modelBuilder.Entity<Ethiopiaboundary>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethiopiaboundary", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethiopiaboundary_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.Etbound01).HasColumnName("etbound0_1");

                entity.Property(e => e.Etbound01c).HasColumnName("etbound01c");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Perimeter).HasColumnName("perimeter");
            });

            modelBuilder.Entity<Ethiopiaregion>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethiopiaregion", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethiopiaregion_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.Etregio01).HasColumnName("etregio0_1");

                entity.Property(e => e.Etregio01c).HasColumnName("etregio01c");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Perimeter).HasColumnName("perimeter");

                entity.Property(e => e.Region).HasColumnName("region");
            });

            modelBuilder.Entity<Ethiopiaworeda>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethiopiaworeda", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethiopiaworeda_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.AreaLak).HasColumnName("area_lak");

                entity.Property(e => e.AreaTxt).HasColumnName("area_txt");

                entity.Property(e => e.Areakm2).HasColumnName("areakm2");

                entity.Property(e => e.CorPdI).HasColumnName("cor_pd_i");

                entity.Property(e => e.Elevation).HasColumnName("elevation");

                entity.Property(e => e.Etwered01c).HasColumnName("etwered01c");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Perimeter).HasColumnName("perimeter");

                entity.Property(e => e.PopdeTxt).HasColumnName("popde_txt");

                entity.Property(e => e.Poprur).HasColumnName("poprur");

                entity.Property(e => e.PoptoTxt).HasColumnName("popto_txt");

                entity.Property(e => e.Poptot).HasColumnName("poptot");

                entity.Property(e => e.Popurb).HasColumnName("popurb");

                entity.Property(e => e.Region).HasColumnName("region");

                entity.Property(e => e.Rupopde).HasColumnName("rupopde");

                entity.Property(e => e.Wereda).HasColumnName("wereda");

                entity.Property(e => e.Zones).HasColumnName("zones");
            });

            modelBuilder.Entity<Ethiopiazone>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethiopiazone", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethiopiazone_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.Etzones01).HasColumnName("etzones0_1");

                entity.Property(e => e.Etzones01c).HasColumnName("etzones01c");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Perimeter).HasColumnName("perimeter");

                entity.Property(e => e.Zones).HasColumnName("zones");
            });

            modelBuilder.Entity<Ethrivers>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethrivers", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethrivers_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Etriver01).HasColumnName("etriver0_1");

                entity.Property(e => e.Etriver01c).HasColumnName("etriver01c");

                entity.Property(e => e.Fnode).HasColumnName("fnode_");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Length).HasColumnName("length");

                entity.Property(e => e.Lpoly).HasColumnName("lpoly_");

                entity.Property(e => e.RivName).HasColumnName("riv_name");

                entity.Property(e => e.Rpoly).HasColumnName("rpoly_");

                entity.Property(e => e.Tnode).HasColumnName("tnode_");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Ethroad>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("ethroad", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("ethroad_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Etroad011).HasColumnName("etroad01_1");

                entity.Property(e => e.Etroad01co).HasColumnName("etroad01co");

                entity.Property(e => e.Fnode).HasColumnName("fnode_");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Length).HasColumnName("length");

                entity.Property(e => e.Lpoly).HasColumnName("lpoly_");

                entity.Property(e => e.Rpoly).HasColumnName("rpoly_");

                entity.Property(e => e.Tnode).HasColumnName("tnode_");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("farm", "frm");

                entity.HasIndex(e => e.ActivityId)
                    .HasName("ixfk_farm_activity");

                entity.HasIndex(e => e.InvestedCapital)
                    .HasName("farm_investment_capital_index");

                entity.HasIndex(e => e.OperatorId)
                    .HasName("ixfk_farms_farm_operator");

                entity.HasIndex(e => e.TypeId)
                    .HasName("ixfk_farm_farm_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.InvestedCapital).HasColumnName("invested_capital");

                entity.Property(e => e.OperatorId).HasColumnName("operator_id");

                entity.Property(e => e.OtherTypeIds)
                    .HasColumnName("other_type_ids")
                    .HasDefaultValueSql("ARRAY[]::integer[]");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Farm)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_activity");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Farm)
                    .HasForeignKey(d => d.Aid)
                    .HasConstraintName("farm_user_action_id_fk");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.Farm)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farms_farm_operator");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Farm)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_farm_type");
            });

            modelBuilder.Entity<FarmLand>(entity =>
            {
                entity.HasKey(e => new { e.LandId, e.FarmId });

                entity.ToTable("farm_land", "frm");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.FarmId).HasColumnName("farm_id");

                entity.Property(e => e.CertificateDoc).HasColumnName("certificate_doc");

                entity.Property(e => e.LeaseContractDoc).HasColumnName("lease_contract_doc");

                entity.HasOne(d => d.CertificateDocNavigation)
                    .WithMany(p => p.FarmLandCertificateDocNavigation)
                    .HasForeignKey(d => d.CertificateDoc)
                    .HasConstraintName("farm_land_document_id_fk");

                entity.HasOne(d => d.Farm)
                    .WithMany(p => p.FarmLand)
                    .HasForeignKey(d => d.FarmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("farm_land_farm_id_fk");

                entity.HasOne(d => d.LeaseContractDocNavigation)
                    .WithMany(p => p.FarmLandLeaseContractDocNavigation)
                    .HasForeignKey(d => d.LeaseContractDoc)
                    .HasConstraintName("farm_land_document_id_fk_2");
            });

            modelBuilder.Entity<FarmOperator>(entity =>
            {
                entity.ToTable("farm_operator", "frm");

                entity.HasIndex(e => e.AddressId)
                    .HasName("ixfk_farm_operator_address");

                entity.HasIndex(e => e.Capital)
                    .HasName("farm_operator_capital_index");

                entity.HasIndex(e => e.OriginId)
                    .HasName("farm_operator_origin_index");

                entity.HasIndex(e => e.TypeId)
                    .HasName("ixfk_farm_operator_farm_operator_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.Birthdate).HasColumnName("birthdate");

                entity.Property(e => e.Capital).HasColumnName("capital");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("char(1)");

                entity.Property(e => e.MartialStatus).HasColumnName("martial_status");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasColumnName("nationality");

                entity.Property(e => e.OriginId)
                    .HasColumnName("origin_id")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.Ventures)
                    .HasColumnName("ventures")
                    .HasDefaultValueSql("ARRAY[]::uuid[]");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.FarmOperator)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_operator_address");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.FarmOperator)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("farm_operator_farm_operator_origin_id_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.FarmOperator)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_operator_farm_operator_type");
            });

            modelBuilder.Entity<FarmOperatorOrigin>(entity =>
            {
                entity.ToTable("farm_operator_origin", "frm");

                entity.HasIndex(e => e.Id)
                    .HasName("farm_operator_origin_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<FarmOperatorRegistration>(entity =>
            {
                entity.ToTable("farm_operator_registration", "frm");

                entity.HasIndex(e => e.AuthorityId)
                    .HasName("ixfk_farm_operator_registration_regsitration_authority");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("farm_operator_registration_document_id_index");

                entity.HasIndex(e => e.OperatorId)
                    .HasName("ixfk_farm_operator_registration_farm_operator");

                entity.HasIndex(e => e.TypeId)
                    .HasName("ixfk_farm_operator_registration_registration_type");

                entity.HasIndex(e => new { e.AuthorityId, e.TypeId })
                    .HasName("farm_operator_registration_authority_id_type_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('frm.farm_operator_registration_id_seq'::regclass)");

                entity.Property(e => e.AuthorityId).HasColumnName("authority_id");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.OperatorId).HasColumnName("operator_id");

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasColumnName("registration_number");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Authority)
                    .WithMany(p => p.FarmOperatorRegistration)
                    .HasForeignKey(d => d.AuthorityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_operator_registration_regsitration_authority");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.FarmOperatorRegistration)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("farm_operator_registration_document_id_fk");

                entity.HasOne(d => d.Operator)
                    .WithMany(p => p.FarmOperatorRegistration)
                    .HasForeignKey(d => d.OperatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_operator_registration_farm_operator");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.FarmOperatorRegistration)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_operator_registration_registration_type");
            });

            modelBuilder.Entity<FarmOperatorType>(entity =>
            {
                entity.ToTable("farm_operator_type", "frm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<FarmRegistration>(entity =>
            {
                entity.ToTable("farm_registration", "frm");

                entity.HasIndex(e => e.AuthorityId)
                    .HasName("ixfk_farm_registration_regsitration_authority");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("farm_registration_document_id_index");

                entity.HasIndex(e => e.FarmId)
                    .HasName("ixfk_farm_registration_farm");

                entity.HasIndex(e => e.TypeId)
                    .HasName("ixfk_farm_registration_registration_type");

                entity.HasIndex(e => new { e.AuthorityId, e.TypeId })
                    .HasName("farm_registration_authority_id_type_id_index");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('frm.farm_registration_id_seq'::regclass)");

                entity.Property(e => e.AuthorityId).HasColumnName("authority_id");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.FarmId).HasColumnName("farm_id");

                entity.Property(e => e.RegistrationNumber)
                    .IsRequired()
                    .HasColumnName("registration_number");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Authority)
                    .WithMany(p => p.FarmRegistration)
                    .HasForeignKey(d => d.AuthorityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_registration_regsitration_authority");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.FarmRegistration)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("farm_registration_document_id_fk");

                entity.HasOne(d => d.Farm)
                    .WithMany(p => p.FarmRegistration)
                    .HasForeignKey(d => d.FarmId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_registration_farm");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.FarmRegistration)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_farm_registration_registration_type");
            });

            modelBuilder.Entity<FarmType>(entity =>
            {
                entity.ToTable("farm_type", "frm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<GroundData>(entity =>
            {
                entity.ToTable("ground_data", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.ground_data_id_seq'::regclass)");

                entity.Property(e => e.GrndType).HasColumnName("grnd_type");

                entity.Property(e => e.Irrigation)
                    .HasColumnName("irrigation")
                    .HasDefaultValueSql("nextval('lb.ground_data_irrigation_seq'::regclass)");

                entity.HasOne(d => d.GrndTypeNavigation)
                    .WithMany(p => p.GroundData)
                    .HasForeignKey(d => d.GrndType)
                    .HasConstraintName("ground_data_ground_water_id_fk");

                entity.HasOne(d => d.IrrigationNavigation)
                    .WithMany(p => p.GroundData)
                    .HasForeignKey(d => d.Irrigation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ground_data_lb.irrigation_id_fk");
            });

            modelBuilder.Entity<GroundWater>(entity =>
            {
                entity.ToTable("ground_water", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("ground_water_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<InverstmentType>(entity =>
            {
                entity.ToTable("inverstment_type", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("inverstment_type_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Irrigation>(entity =>
            {
                entity.ToTable("irrigation", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.irrigation_id_seq'::regclass)");

                entity.Property(e => e.GrndWater).HasColumnName("grnd_water");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.HasOne(d => d.GrndWaterNavigation)
                    .WithMany(p => p.Irrigation)
                    .HasForeignKey(d => d.GrndWater)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("irrigation_ground_water_id_fk");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.Irrigation)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("irrigation_land_id_fk");
            });

            modelBuilder.Entity<Lake>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("lake", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("lake_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Region).HasColumnName("region");
            });

            modelBuilder.Entity<Land>(entity =>
            {
                entity.ToTable("land", "lb");

                entity.HasIndex(e => e.LandType)
                    .HasName("IXFK_land_land_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.LandType).HasColumnName("land_type");

                entity.Property(e => e.Wid).HasColumnName("wid");
            });

            modelBuilder.Entity<LandAccessibility>(entity =>
            {
                entity.ToTable("land_accessibility", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.lb_seq'::regclass)");

                entity.Property(e => e.Accessibility).HasColumnName("accessibility");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.HasOne(d => d.AccessibilityNavigation)
                    .WithMany(p => p.LandAccessibility)
                    .HasForeignKey(d => d.Accessibility)
                    .HasConstraintName("accessibility_accessibility_fkey");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandAccessibility)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("accessibility_land_id_fkey");
            });

            modelBuilder.Entity<LandClimate>(entity =>
            {
                entity.HasKey(e => new { e.LandId, e.Month });

                entity.ToTable("land_climate", "lb");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Month).HasColumnName("month");

                entity.Property(e => e.Precipitation).HasColumnName("precipitation");

                entity.Property(e => e.TempAvg).HasColumnName("temp_avg");

                entity.Property(e => e.TempHigh).HasColumnName("temp_high");

                entity.Property(e => e.TempLow).HasColumnName("temp_low");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandClimate)
                    .HasForeignKey(d => d.LandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("land_climate_land_id_fk");
            });

            modelBuilder.Entity<LandDoc>(entity =>
            {
                entity.ToTable("land_doc", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("land_doc_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DocId).HasColumnName("doc_id");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.HasOne(d => d.Doc)
                    .WithMany(p => p.LandDoc)
                    .HasForeignKey(d => d.DocId)
                    .HasConstraintName("land_doc_document_id_fk");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandDoc)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("land_doc_land_id_fk");
            });

            modelBuilder.Entity<LandInvestment>(entity =>
            {
                entity.ToTable("land_investment", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("land_investment_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.land_investment_id_seq'::regclass)");

                entity.Property(e => e.Investment).HasColumnName("investment");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandInvestment)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("land_investment_land_id_fk");
            });

            modelBuilder.Entity<LandMoisture>(entity =>
            {
                entity.ToTable("land_moisture", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("land_moisture_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.land_moisture_id_seq'::regclass)");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Moisture).HasColumnName("moisture");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandMoisture)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("land_moisture_land_id_fk");

                entity.HasOne(d => d.MoistureNavigation)
                    .WithMany(p => p.LandMoisture)
                    .HasForeignKey(d => d.Moisture)
                    .HasConstraintName("land_moisture_moisture_source_id_fk");
            });

            modelBuilder.Entity<LandRight>(entity =>
            {
                entity.HasKey(e => e.LandId);

                entity.ToTable("land_right", "lb");

                entity.Property(e => e.LandId)
                    .HasColumnName("land_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CertificateDocument).HasColumnName("certificate_document");

                entity.Property(e => e.ContractDocument).HasColumnName("contract_document");

                entity.Property(e => e.LandSectionArea).HasColumnName("land_section_area");

                entity.Property(e => e.RightFrom).HasColumnName("right_from");

                entity.Property(e => e.RightTo).HasColumnName("right_to");

                entity.Property(e => e.RightType).HasColumnName("right_type");

                entity.Property(e => e.YearlyRent).HasColumnName("yearly_rent");

                entity.HasOne(d => d.CertificateDocumentNavigation)
                    .WithMany(p => p.LandRightCertificateDocumentNavigation)
                    .HasForeignKey(d => d.CertificateDocument)
                    .HasConstraintName("land_right_document_id_fk");

                entity.HasOne(d => d.ContractDocumentNavigation)
                    .WithMany(p => p.LandRightContractDocumentNavigation)
                    .HasForeignKey(d => d.ContractDocument)
                    .HasConstraintName("land_right_document_id_fk_2");

                entity.HasOne(d => d.Land)
                    .WithOne(p => p.LandRight)
                    .HasForeignKey<LandRight>(d => d.LandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("land_right_land_id_fk");
            });

            modelBuilder.Entity<LandSplit>(entity =>
            {
                entity.ToTable("land_split", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.lb_seq'::regclass)");

                entity.Property(e => e.Geom).HasColumnName("geom");
            });

            modelBuilder.Entity<LandType>(entity =>
            {
                entity.ToTable("land_type", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<LandUpin>(entity =>
            {
                entity.HasKey(e => e.Upin);

                entity.ToTable("land_upin", "lb");

                entity.HasIndex(e => e.LandId)
                    .HasName("IXFK_land_upin_land");

                entity.Property(e => e.Upin)
                    .HasColumnName("upin")
                    .ValueGeneratedNever();

                entity.Property(e => e.Area).HasColumnName("area");

                entity.Property(e => e.CentroidX).HasColumnName("centroid_x");

                entity.Property(e => e.CentroidY).HasColumnName("centroid_y");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Profile)
                    .HasColumnName("profile")
                    .HasColumnType("json");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandUpin)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("land_upin_land_id_fkey");
            });

            modelBuilder.Entity<LandUsage>(entity =>
            {
                entity.ToTable("land_usage", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("land_usage_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.land_usage_id_seq'::regclass)");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Use)
                    .HasColumnName("use")
                    .HasDefaultValueSql("4");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.LandUsage)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("land_usage_land_id_fk");

                entity.HasOne(d => d.UseNavigation)
                    .WithMany(p => p.LandUsage)
                    .HasForeignKey(d => d.Use)
                    .HasConstraintName("land_usage_usage_type_name_fk");
            });

            modelBuilder.Entity<MoistureSource>(entity =>
            {
                entity.ToTable("moisture_source", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("moisture_source_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Months>(entity =>
            {
                entity.ToTable("months", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<RegistrationAuthority>(entity =>
            {
                entity.ToTable("registration_authority", "frm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<RegistrationType>(entity =>
            {
                entity.ToTable("registration_type", "frm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role", "sys");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval(('sys.\"role_id_seq\"'::text)::regclass)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<SoilTest>(entity =>
            {
                entity.ToTable("soil_test", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.lb_seq'::regclass)");

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Result).HasColumnName("result");

                entity.Property(e => e.TestType).HasColumnName("test_type");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.SoilTest)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("soil_test_land_id_fkey");

                entity.HasOne(d => d.TestTypeNavigation)
                    .WithMany(p => p.SoilTest)
                    .HasForeignKey(d => d.TestType)
                    .HasConstraintName("soil_test_test_type_fkey");
            });

            modelBuilder.Entity<SoilTestTypes>(entity =>
            {
                entity.ToTable("soil_test_types", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<SpatialRefSys>(entity =>
            {
                entity.HasKey(e => e.Srid);

                entity.ToTable("spatial_ref_sys");

                entity.Property(e => e.Srid)
                    .HasColumnName("srid")
                    .ValueGeneratedNever();

                entity.Property(e => e.AuthName).HasColumnName("auth_name");

                entity.Property(e => e.AuthSrid).HasColumnName("auth_srid");

                entity.Property(e => e.Proj4text).HasColumnName("proj4text");

                entity.Property(e => e.Srtext).HasColumnName("srtext");
            });

            modelBuilder.Entity<Stream>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("stream", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("stream_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<SurfaceWater>(entity =>
            {
                entity.ToTable("surface_water", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("surface_water_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.surface_water_id_seq'::regclass)");

                entity.Property(e => e.Irrigation).HasColumnName("irrigation");

                entity.Property(e => e.Result)
                    .IsRequired()
                    .HasColumnName("result");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.IrrigationNavigation)
                    .WithMany(p => p.SurfaceWater)
                    .HasForeignKey(d => d.Irrigation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("surface_water_irrigation_id_fk");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.SurfaceWater)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("surface_water_surface_water_type_id_fk");
            });

            modelBuilder.Entity<SurfaceWaterType>(entity =>
            {
                entity.ToTable("surface_water_type", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("surface_water_type_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Topography>(entity =>
            {
                entity.ToTable("topography", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("topography_id_uindex_2")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.LandId).HasColumnName("land_id");

                entity.Property(e => e.Result)
                    .IsRequired()
                    .HasColumnName("result");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Land)
                    .WithMany(p => p.Topography)
                    .HasForeignKey(d => d.LandId)
                    .HasConstraintName("topography_land_id_fk");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Topography)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("topography_topography_type_id_fk");
            });

            modelBuilder.Entity<TopographyType>(entity =>
            {
                entity.ToTable("topography_type", "lb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<UsageType>(entity =>
            {
                entity.ToTable("usage_type", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("usage_type_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "sys");

                entity.HasIndex(e => e.Username)
                    .HasName("user_username_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval(('sys.\"user_id_seq\"'::text)::regclass)");

                entity.Property(e => e.FullName).HasColumnName("full_name");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.PhoneNo).HasColumnName("phone_no");

                entity.Property(e => e.RegOn).HasColumnName("reg_on");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");
            });

            modelBuilder.Entity<UserAction>(entity =>
            {
                entity.ToTable("user_action", "sys");

                entity.HasIndex(e => e.ActionTypeId)
                    .HasName("ixfk_user_action_action_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('sys.user_action_id_seq'::regclass)");

                entity.Property(e => e.ActionTypeId).HasColumnName("action_type_id");

                entity.Property(e => e.Remark).HasColumnName("remark");

                entity.Property(e => e.Timestamp).HasColumnName("timestamp");

                entity.Property(e => e.Username).HasColumnName("username");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.UserAction)
                    .HasPrincipalKey(p => p.Username)
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("user_action_user_username_fk");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("user_role", "sys");

                entity.HasIndex(e => e.Aid)
                    .HasName("ixfk_user_role_user_action");

                entity.HasIndex(e => e.RoleId)
                    .HasName("ixfk_userrole_role");

                entity.HasIndex(e => e.UserId)
                    .HasName("ixfk_userrole_user");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.Aid)
                    .HasConstraintName("fk_user_role_user_action");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userrole_role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_userrole_user");
            });

            modelBuilder.Entity<WaterSourceType>(entity =>
            {
                entity.ToTable("water_source_type", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("water_source_type_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<WaterSrcParam>(entity =>
            {
                entity.ToTable("water_src_param", "lb");

                entity.HasIndex(e => e.Id)
                    .HasName("water_src_param_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('lb.water_src_param_id_seq'::regclass)");

                entity.Property(e => e.Irrigation).HasColumnName("irrigation");

                entity.Property(e => e.Result).HasColumnName("result");

                entity.Property(e => e.SrcType).HasColumnName("src_type");

                entity.HasOne(d => d.IrrigationNavigation)
                    .WithMany(p => p.WaterSrcParam)
                    .HasForeignKey(d => d.Irrigation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("water_src_param_irrigation_id_fk");

                entity.HasOne(d => d.SrcTypeNavigation)
                    .WithMany(p => p.WaterSrcParam)
                    .HasForeignKey(d => d.SrcType)
                    .HasConstraintName("water_src_param_water_source_type_id_fk");
            });

            modelBuilder.Entity<WoretaWatershed>(entity =>
            {
                entity.HasKey(e => e.Gid);

                entity.ToTable("woreta_watershed", "spatial");

                entity.HasIndex(e => e.Geom)
                    .HasName("woreta_watershed_geom_idx")
                    .HasMethod("gist");

                entity.Property(e => e.Gid)
                    .HasColumnName("gid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Altitude).HasColumnName("altitude");

                entity.Property(e => e.Color).HasColumnName("color");

                entity.Property(e => e.Depth).HasColumnName("depth");

                entity.Property(e => e.Display).HasColumnName("display");

                entity.Property(e => e.Filename).HasColumnName("filename");

                entity.Property(e => e.Geom).HasColumnName("geom");

                entity.Property(e => e.Ident).HasColumnName("ident");

                entity.Property(e => e.Lat).HasColumnName("lat");

                entity.Property(e => e.Long).HasColumnName("long");

                entity.Property(e => e.Ltime).HasColumnName("ltime");

                entity.Property(e => e.Model).HasColumnName("model");

                entity.Property(e => e.NewSeg).HasColumnName("new_seg");

                entity.Property(e => e.Temp).HasColumnName("temp");

                entity.Property(e => e.Time).HasColumnName("time");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.XProj).HasColumnName("x_proj");

                entity.Property(e => e.YProj).HasColumnName("y_proj");
            });

            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.ToTable("workflow", "wf");

                entity.HasComment("This table holds the instances of all state machines.");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.CurrentState).HasColumnName("current_state");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Workflow)
                    .HasForeignKey(d => d.Aid)
                    .HasConstraintName("workflow_user_action_id_fk");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Workflow)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("workflow_workflow_type_id_fk");
            });

            modelBuilder.Entity<WorkflowType>(entity =>
            {
                entity.ToTable("workflow_type", "wf");

                entity.HasComment("This table enumerates different types of workflow.");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<WorkItem>(entity =>
            {
                entity.ToTable("work_item", "wf");

                entity.HasIndex(e => e.Id)
                    .HasName("work_item_id_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.AssignedRole).HasColumnName("assigned_role");

                entity.Property(e => e.AssignedUser).HasColumnName("assigned_user");

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .HasColumnType("json");

                entity.Property(e => e.DataType).HasColumnName("data_type");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FromState).HasColumnName("from_state");

                entity.Property(e => e.SeqNo).HasColumnName("seq_no");

                entity.Property(e => e.ToState).HasColumnName("to_state");

                entity.Property(e => e.Trigger).HasColumnName("trigger");

                entity.Property(e => e.WorkflowId).HasColumnName("workflow_id");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.WorkItem)
                    .HasForeignKey(d => d.Aid)
                    .HasConstraintName("work_item_user_action_id_fk");

                entity.HasOne(d => d.AssignedRoleNavigation)
                    .WithMany(p => p.WorkItem)
                    .HasForeignKey(d => d.AssignedRole)
                    .HasConstraintName("work_item_role_id_fk");

                entity.HasOne(d => d.AssignedUserNavigation)
                    .WithMany(p => p.WorkItem)
                    .HasForeignKey(d => d.AssignedUser)
                    .HasConstraintName("work_item_user_id_fk");

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.WorkItem)
                    .HasForeignKey(d => d.WorkflowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("work_item_workflow_id_fk");
            });

            modelBuilder.HasSequence("farm_operator_registration_id_seq");

            modelBuilder.HasSequence("farm_registration_id_seq");

            modelBuilder.HasSequence("agricultural_zone_id_seq");

            modelBuilder.HasSequence("agro_echology_id_seq");

            modelBuilder.HasSequence("ground_data_id_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("ground_data_irrigation_seq")
                .HasMin(1)
                .HasMax(2147483647);

            modelBuilder.HasSequence("irrigation_id_seq");

            modelBuilder.HasSequence("land_investment_id_seq");

            modelBuilder.HasSequence("land_moisture_id_seq");

            modelBuilder.HasSequence("land_usage_id_seq");

            modelBuilder.HasSequence("lb_seq").IsCyclic();

            modelBuilder.HasSequence("surface_water_id_seq");

            modelBuilder.HasSequence("topography_id_seq");

            modelBuilder.HasSequence("water_src_param_id_seq");

            modelBuilder.HasSequence("audit_seq");

            modelBuilder.HasSequence("role_id_seq");

            modelBuilder.HasSequence("user_action_id_seq");

            modelBuilder.HasSequence("user_id_seq");
        }
    }
}
