using System;
using intapscamis.camis.data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace camis.aggregator.data.Entities
{
    public partial class aggregatorContext : DbContext
    {
        public aggregatorContext()
        {
        }

        public aggregatorContext(DbContextOptions<aggregatorContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccessibiltyType> AccessibiltyType { get; set; }
        public virtual DbSet<ActionType> ActionType { get; set; }
        public virtual DbSet<AgriculturalZone> AgriculturalZone { get; set; }
        public virtual DbSet<AgroEchology> AgroEchology { get; set; }
        public virtual DbSet<AgroType> AgroType { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Certificate> Certificate { get; set; }
        public virtual DbSet<ConnectionParams> ConnectionParams { get; set; }
        public virtual DbSet<GroundData> GroundData { get; set; }
        public virtual DbSet<GroundWater> GroundWater { get; set; }
        public virtual DbSet<InverstmentType> InverstmentType { get; set; }
        public virtual DbSet<Irrigation> Irrigation { get; set; }
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
        public virtual DbSet<ReportData> ReportData { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SoilTest> SoilTest { get; set; }
        public virtual DbSet<SoilTestTypes> SoilTestTypes { get; set; }
        public virtual DbSet<SpatialRefSys> SpatialRefSys { get; set; }
        public virtual DbSet<SurfaceWater> SurfaceWater { get; set; }
        public virtual DbSet<SurfaceWaterType> SurfaceWaterType { get; set; }
        public virtual DbSet<Topography> Topography { get; set; }
        public virtual DbSet<TopographyType> TopographyType { get; set; }
        public virtual DbSet<TRegions> TRegions { get; set; }
        public virtual DbSet<TWoredas> TWoredas { get; set; }
        public virtual DbSet<TZones> TZones { get; set; }
        public virtual DbSet<UsageType> UsageType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAction> UserAction { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<WaterSourceType> WaterSourceType { get; set; }
        public virtual DbSet<WaterSrcParam> WaterSrcParam { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Host=localhost;Database=aggregator;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

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
                entity.ToTable("action_type");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnName("name");
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
                entity.ToTable("audit_log");

                entity.Property(e => e.Id).HasColumnName("id");

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

            modelBuilder.Entity<ConnectionParams>(entity =>
            {
                entity.ToTable("connection_params");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.RegionCode)
                    .HasColumnName("region_code")
                    .HasColumnType("varchar");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("varchar");
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

                entity.Property(e => e.RightFrom).HasColumnName("right_from");

                entity.Property(e => e.RightTo).HasColumnName("right_to");

                entity.Property(e => e.RightType).HasColumnName("right_type");

                entity.Property(e => e.YearlyRent).HasColumnName("yearly_rent");

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

            modelBuilder.Entity<ReportData>(entity =>
            {
                entity.ToTable("report_data");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FilteredBy).HasColumnName("filtered_by");

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasColumnType("varchar");

                entity.Property(e => e.ReportRequest)
                    .IsRequired()
                    .HasColumnName("report_request");

                entity.Property(e => e.ReportResponse)
                    .HasColumnName("report_response")
                    .HasColumnType("varchar");

                entity.Property(e => e.ReportType).HasColumnName("report_type");

                entity.Property(e => e.SummerizedBy).HasColumnName("summerized_by");

                entity.Property(e => e.Timestamp).HasColumnName("timestamp");

                entity.Property(e => e.Woreda)
                    .HasColumnName("woreda")
                    .HasColumnType("varchar");

                entity.Property(e => e.Zone)
                    .HasColumnName("zone")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval(('\"role_id_seq\"'::text)::regclass)");

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

            modelBuilder.Entity<TRegions>(entity =>
            {
                entity.HasKey(e => new { e.Csaregionid, e.Id });

                entity.ToTable("t_regions");

                entity.HasComment("The table t_regions stores all regions to ensure an stable identification of object in the system");

                entity.Property(e => e.Csaregionid).HasColumnName("csaregionid");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Csaregionnameamharic).HasColumnName("csaregionnameamharic");

                entity.Property(e => e.Csaregionnameeng).HasColumnName("csaregionnameeng");

                entity.Property(e => e.Csaregionnameoromifya).HasColumnName("csaregionnameoromifya");

                entity.Property(e => e.Csaregionnametigrinya).HasColumnName("csaregionnametigrinya");

                entity.Property(e => e.Geometry).HasColumnName("geometry");

                entity.Property(e => e.Password).HasColumnName("password");

                entity.Property(e => e.Regioncodeam).HasColumnName("regioncodeam");

                entity.Property(e => e.Url).HasColumnName("url");

                entity.Property(e => e.Username).HasColumnName("username");
            });

            modelBuilder.Entity<TWoredas>(entity =>
            {
                entity.ToTable("t_woredas");

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
                entity.ToTable("t_zones");

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
                entity.ToTable("user");

                entity.HasIndex(e => e.Username)
                    .HasName("user_username_uindex")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval(('\"user_id_seq\"'::text)::regclass)");

                entity.Property(e => e.CamisPassword)
                    .HasColumnName("camis_password")
                    .HasColumnType("varchar");

                entity.Property(e => e.CamisUsername)
                    .HasColumnName("camis_username")
                    .HasColumnType("varchar");

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
                entity.ToTable("user_action");

                entity.HasIndex(e => e.ActionTypeId)
                    .HasName("ixfk_user_action_action_type");

                entity.Property(e => e.Id).HasColumnName("id");

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

                entity.ToTable("user_role");

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

            modelBuilder.HasSequence("user_id_seq");

            

            modelBuilder.Entity<FarmLand>().Ignore(m => m.CertificateDocNavigation);
            modelBuilder.Entity<FarmLand>().Ignore(m => m.CertificateDoc);
            modelBuilder.Entity<Document>().Ignore(m => m.FarmLandCertificateDocNavigation);
            modelBuilder.Entity<Document>().Ignore(m => m.LandRightCertificateDocumentNavigation);
            modelBuilder.Entity<Document>().Ignore(m => m.LandRightContractDocumentNavigation);
            modelBuilder.Entity<LandRight>().Ignore(m => m.CertificateDocumentNavigation);
            modelBuilder.Entity<LandRight>().Ignore(m => m.ContractDocumentNavigation);
            modelBuilder.Ignore<ActivityVariableValueList>();
            modelBuilder.Ignore<FarmLand>();

            modelBuilder.Entity<UserRole>().HasKey(vf => new { vf.UserId, vf.RoleId });
            modelBuilder.Entity<intapscamis.camis.data.Entities.UserRole>().HasKey(vf => new { vf.UserId, vf.RoleId });
        }
    }
}
