using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace intaps.camisPortal.Entities
{
    public partial class CamisPortalContext : DbContext
    {
        public CamisPortalContext()
        {
        }

        public CamisPortalContext(DbContextOptions<CamisPortalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AddressScheme> AddressScheme { get; set; }
        public virtual DbSet<AddressSchemeUnit> AddressSchemeUnit { get; set; }
        public virtual DbSet<AddressUnit> AddressUnit { get; set; }
        public virtual DbSet<ApplicationEvaluation> ApplicationEvaluation { get; set; }
        public virtual DbSet<ApplicationEvaluationTeam> ApplicationEvaluationTeam { get; set; }
        public virtual DbSet<CamisList> CamisList { get; set; }
        public virtual DbSet<CamisTransferRequest> CamisTransferRequest { get; set; }
        public virtual DbSet<EvaluationTeamMember> EvaluationTeamMember { get; set; }
        public virtual DbSet<Investor> Investor { get; set; }
        public virtual DbSet<InvestorApplication> InvestorApplication { get; set; }
        public virtual DbSet<InvestorApplicationDocument> InvestorApplicationDocument { get; set; }
        public virtual DbSet<InvestorApplicationResubmissionRequest> InvestorApplicationResubmissionRequest { get; set; }
        public virtual DbSet<InvestorProject> InvestorProject { get; set; }
        public virtual DbSet<InvestorRegistrationNumber> InvestorRegistrationNumber { get; set; }
        public virtual DbSet<PortalUser> PortalUser { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<PromotionDoc> PromotionDoc { get; set; }
        public virtual DbSet<PromotionPicture> PromotionPicture { get; set; }
        public virtual DbSet<PromotionStatusChange> PromotionStatusChange { get; set; }
        public virtual DbSet<PromotionUnit> PromotionUnit { get; set; }
        public virtual DbSet<Regions> Regions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Database=camis_portal;Username=postgres;Password=admin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

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
                entity.ToTable("address_scheme");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AddressSchemeUnit>(entity =>
            {
                entity.ToTable("address_scheme_unit");

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
                entity.ToTable("address_unit");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Custom).HasColumnName("custom");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ApplicationEvaluation>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.InvestorId, e.EvaluatorUserName, e.TeamId });

                entity.ToTable("application_evaluation");

                entity.HasIndex(e => new { e.PromotionUnitId, e.EvaluatorUserName })
                    .HasName("application_evaluation_pk")
                    .IsUnique();

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.EvaluatorUserName)
                    .HasColumnName("evaluator_user_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.EvaluationDetail)
                    .HasColumnName("evaluation_detail")
                    .HasColumnType("json");

                entity.Property(e => e.EvaluationPoint).HasColumnName("evaluation_point");

                entity.Property(e => e.SubmitDate).HasColumnName("submit_date");

                entity.HasOne(d => d.EvaluatorUserNameNavigation)
                    .WithMany(p => p.ApplicationEvaluation)
                    .HasForeignKey(d => d.EvaluatorUserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("application_evaluation_portal_user_user_name_fk");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.ApplicationEvaluation)
                    .HasForeignKey(d => d.InvestorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("application_evaluation_investor_id_fk");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.ApplicationEvaluation)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("application_evaluation_promotion_unit_id_fk");
            });

            modelBuilder.Entity<ApplicationEvaluationTeam>(entity =>
            {
                entity.ToTable("application_evaluation_team");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.EvaluationCriterion)
                    .HasColumnName("evaluation_criterion")
                    .HasColumnType("json");

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.TeamName)
                    .HasColumnName("team_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.TeamWeight).HasColumnName("team_weight");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.ApplicationEvaluationTeam)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .HasConstraintName("application_evaluator_team_promotion_unit_id_fk");
            });

            modelBuilder.Entity<CamisList>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.ToTable("camis_list");

                entity.Property(e => e.Key)
                    .HasColumnName("key")
                    .HasColumnType("character varying")
                    .ValueGeneratedNever();

                entity.Property(e => e.ListData)
                    .HasColumnName("list_data")
                    .HasColumnType("json");
            });

            modelBuilder.Entity<CamisTransferRequest>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.InvestorId });

                entity.ToTable("camis_transfer_request");

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.FarmId).HasColumnName("farm_id");

                entity.Property(e => e.RequestWfid).HasColumnName("request_wfid");
            });

            modelBuilder.Entity<EvaluationTeamMember>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.UserName });

                entity.ToTable("evaluation_team_member");

                entity.HasIndex(e => new { e.TeamId, e.UserName })
                    .HasName("evaluation_team_member_pk")
                    .IsUnique();

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.EvaluationTeamMember)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("evaluation_team_member_application_evaluation_team_id_fk");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.EvaluationTeamMember)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("evaluation_team_member_portal_user_user_name_fk");
            });

            modelBuilder.Entity<Investor>(entity =>
            {
                entity.ToTable("investor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DefaultProfile)
                    .HasColumnName("default_profile")
                    .HasColumnType("json");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Investor)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_profile_portal_user_user_name_fk");
            });

            modelBuilder.Entity<InvestorApplication>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.InvestorId });

                entity.ToTable("investor_application");

                entity.HasIndex(e => new { e.PromotionUnitId, e.InvestorId })
                    .HasName("investor_application_pk")
                    .IsUnique();

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.ApplyTime).HasColumnName("apply_time");

                entity.Property(e => e.Investment)
                    .HasColumnName("investment")
                    .HasColumnType("json");
                
                entity.Property(e => e.ActivityPlan)
                    .HasColumnName("activity_plan")
                    .HasColumnType("json");

                entity.Property(e => e.IsApproved).HasColumnName("is_approved");
                
                entity.Property(e => e.ContactAddress)
                    .HasColumnName("contactAddress")
                    .HasColumnType("json");

                entity.Property(e => e.InvestmentCapital).HasColumnName("investment_capital");

                entity.Property(e => e.InvestmentType)
                    .HasColumnName("investment_type")
                    .HasColumnType("json");

                entity.Property(e => e.ProposalAbstract)
                    .HasColumnName("proposal_abstract")
                    .HasColumnType("character varying");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestorApplication)
                    .HasForeignKey(d => d.InvestorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_application_investor_id_fk");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.InvestorApplication)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_application_promotion_unit_id_fk");
            });

            modelBuilder.Entity<InvestorApplicationDocument>(entity =>
            {
                entity.ToTable("investor_application_document");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .HasColumnType("json");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestorApplicationDocument)
                    .HasForeignKey(d => d.InvestorId)
                    .HasConstraintName("investor_application_document_investor_id_fk");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.InvestorApplicationDocument)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_application_document_promotion_unit_id_fk");
            });

            modelBuilder.Entity<InvestorApplicationResubmissionRequest>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.InvestorId });

                entity.ToTable("investor_application_resubmission_request");

                entity.HasIndex(e => new { e.PromotionUnitId, e.InvestorId })
                    .HasName("investor_application_resubmission_request_pk")
                    .IsUnique();

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.RequestTime).HasColumnName("request_time");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.WaitUntil).HasColumnName("wait_until");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestorApplicationResubmissionRequest)
                    .HasForeignKey(d => d.InvestorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_application_resubmission_request_investor_id_fk");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.InvestorApplicationResubmissionRequest)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investor_application_resubmission_request_promotion_unit_id_fk");
            });

            modelBuilder.Entity<InvestorProject>(entity =>
            {
                entity.HasKey(e => e.InvestorId);

                entity.ToTable("investor_project");

                entity.Property(e => e.InvestorId)
                    .HasColumnName("investor_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.ProjectProfile)
                    .HasColumnName("project_profile")
                    .HasColumnType("json");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SyncNumber).HasColumnName("sync_number");
            });

            modelBuilder.Entity<InvestorRegistrationNumber>(entity =>
            {
                entity.HasKey(e => new { e.InvestorId, e.RegisrationNumber, e.RegistrationTypeId });

                entity.ToTable("investor_registration_number");

                entity.Property(e => e.InvestorId).HasColumnName("investor_id");

                entity.Property(e => e.RegisrationNumber)
                    .HasColumnName("regisration_number")
                    .HasColumnType("character varying");

                entity.Property(e => e.RegistrationTypeId).HasColumnName("registration_type_id");
            });

            modelBuilder.Entity<PortalUser>(entity =>
            {
                entity.HasKey(e => e.UserName);

                entity.ToTable("portal_user");

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasColumnType("character varying")
                    .ValueGeneratedNever();

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CamisPassword)
                    .HasColumnName("camis_password")
                    .HasColumnType("character varying");

                entity.Property(e => e.CamisUserName)
                    .HasColumnName("camis_user_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.EMail)
                    .HasColumnName("e_mail")
                    .HasColumnType("character varying");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("character varying");

                entity.Property(e => e.PhoneNo)
                    .HasColumnName("phone_no")
                    .HasColumnType("character varying");

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasColumnName("region")
                    .HasColumnType("character varying");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.PortalUser)
                    .HasForeignKey(d => d.Region)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("portal_user_regions_code_fk");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ApplyDateFrom).HasColumnName("apply_date_from");

                entity.Property(e => e.ApplyDateTo).HasColumnName("apply_date_to");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("character varying");

                entity.Property(e => e.EvaluationCriterion)
                    .HasColumnName("evaluation_criterion")
                    .HasColumnType("json");

                entity.Property(e => e.PhysicalAddress)
                    .HasColumnName("physicalAddress")
                    .HasColumnType("character varying");

                entity.Property(e => e.PostedOn).HasColumnName("posted_on");

                entity.Property(e => e.PromotionRef)
                    .IsRequired()
                    .HasColumnName("promotion_ref")
                    .HasColumnType("character varying");

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasColumnType("character varying");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Summary)
                    .HasColumnName("summary")
                    .HasColumnType("character varying");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("character varying");

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Promotion)
                    .HasForeignKey(d => d.Region)
                    .HasConstraintName("promotion_regions_code_fk");
            });

            modelBuilder.Entity<PromotionDoc>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.Order });

                entity.ToTable("promotion_doc");

                entity.HasIndex(e => new { e.PromotionUnitId, e.Order })
                    .HasName("promotion_doc_pk")
                    .IsUnique();

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.DocData)
                    .HasColumnName("doc_data")
                    .HasColumnType("json");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.PromotionDoc)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promotion_doc_promotion_unit_id_fk");
            });

            modelBuilder.Entity<PromotionPicture>(entity =>
            {
                entity.HasKey(e => new { e.PromotionUnitId, e.Order });

                entity.ToTable("promotion_picture");

                entity.HasIndex(e => new { e.PromotionUnitId, e.Order })
                    .HasName("promotion_picture_pk")
                    .IsUnique();

                entity.Property(e => e.PromotionUnitId).HasColumnName("promotion_unit_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasColumnType("json");

                entity.HasOne(d => d.PromotionUnit)
                    .WithMany(p => p.PromotionPicture)
                    .HasForeignKey(d => d.PromotionUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promotion_picture_promotion_unit_id_fk");
            });

            modelBuilder.Entity<PromotionStatusChange>(entity =>
            {
                entity.ToTable("promotion_status_change");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ChangeTime).HasColumnName("change_time");

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .HasColumnType("json");

                entity.Property(e => e.NewStatus).HasColumnName("new_status");

                entity.Property(e => e.OldStatus).HasColumnName("old_status");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionStatusChange)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_promtion_status_change_promtion");
            });

            modelBuilder.Entity<PromotionUnit>(entity =>
            {
                entity.ToTable("promotion_unit");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("character varying");

                entity.Property(e => e.InvestmentType)
                    .HasColumnName("investment_type")
                    .HasColumnType("json");

                entity.Property(e => e.LandProfile)
                    .HasColumnName("land_profile")
                    .HasColumnType("json");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("character varying");
                
                entity.Property(e => e.WinnerInvestor).HasColumnName("winner_investor");
                
                entity.Property(e => e.Status).HasColumnName("status");
                    
                entity.HasOne(d => d.Promotion)
                    .WithMany(p => p.PromotionUnit)
                    .HasForeignKey(d => d.PromotionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promotion_unit_promotion_id_fk");
            });

            modelBuilder.Entity<Regions>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("regions");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasColumnType("character varying")
                    .ValueGeneratedNever();

                entity.Property(e => e.CamisUrl)
                    .HasColumnName("camis_url")
                    .HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("character varying");
            });
        }
    }
}
