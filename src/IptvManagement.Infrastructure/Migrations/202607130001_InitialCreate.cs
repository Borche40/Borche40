using IptvManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IptvManagement.Infrastructure.Migrations;

/// <summary>
/// Erstellt alle Identity- und Fachdatentabellen für die lokale Entwicklungsdatenbank.
/// </summary>
[DbContext(typeof(AppDbContext))]
[Migration("202607130001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("AspNetRoles", table => new
        {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

        migrationBuilder.CreateTable("AspNetUsers", table => new
        {
            Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        migrationBuilder.CreateTable("AuditLogs", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Action = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
            EntityName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
            EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
            NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_AuditLogs", x => x.Id));

        migrationBuilder.CreateTable("Categories", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            SortOrder = table.Column<int>(type: "int", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_Categories", x => x.Id));

        migrationBuilder.CreateTable("Customers", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
            PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            City = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            IsBlocked = table.Column<bool>(type: "bit", nullable: false),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_Customers", x => x.Id));

        migrationBuilder.CreateTable("Packages", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
            DurationInMonths = table.Column<int>(type: "int", nullable: false),
            MaxDevices = table.Column<int>(type: "int", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_Packages", x => x.Id));

        migrationBuilder.CreateTable("PlaylistAccessLogs", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
            RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
            FailureReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
        }, constraints: table => table.PrimaryKey("PK_PlaylistAccessLogs", x => x.Id));

        migrationBuilder.CreateTable("SystemSettings", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Key = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_SystemSettings", x => x.Id));

        migrationBuilder.CreateTable("AspNetRoleClaims", table => new
        {
            Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
            table.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserClaims", table => new
        {
            Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
            table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserLogins", table => new
        {
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
            table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserRoles", table => new
        {
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
            table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("AspNetUserTokens", table => new
        {
            UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("Channels", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            StreamUrlEncrypted = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
            LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            SortOrder = table.Column<int>(type: "int", nullable: false),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            IsPubliclyAvailable = table.Column<bool>(type: "bit", nullable: false),
            LicenseReference = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
            LicenseValidUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_Channels", x => x.Id);
            table.ForeignKey("FK_Channels_Categories_CategoryId", x => x.CategoryId, "Categories", "Id", onDelete: ReferentialAction.Restrict);
        });

        migrationBuilder.CreateTable("Subscriptions", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubscriptionNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
            StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            ExpirationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            MaxDevices = table.Column<int>(type: "int", nullable: false),
            PlaylistTokenHash = table.Column<string>(type: "nvarchar(450)", nullable: true),
            TokenCreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            LastRenewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            AutomaticRenewal = table.Column<bool>(type: "bit", nullable: false),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_Subscriptions", x => x.Id);
            table.ForeignKey("FK_Subscriptions_Customers_CustomerId", x => x.CustomerId, "Customers", "Id", onDelete: ReferentialAction.Restrict);
            table.ForeignKey("FK_Subscriptions_Packages_PackageId", x => x.PackageId, "Packages", "Id", onDelete: ReferentialAction.Restrict);
        });

        migrationBuilder.CreateTable("Devices", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DeviceName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            DeviceIdentifierHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
            DeviceType = table.Column<int>(type: "int", nullable: false),
            LastIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            FirstSeenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            LastSeenAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
            IsActive = table.Column<bool>(type: "bit", nullable: false),
            IsBlocked = table.Column<bool>(type: "bit", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_Devices", x => x.Id);
            table.ForeignKey("FK_Devices_Customers_CustomerId", x => x.CustomerId, "Customers", "Id", onDelete: ReferentialAction.Restrict);
            table.ForeignKey("FK_Devices_Subscriptions_SubscriptionId", x => x.SubscriptionId, "Subscriptions", "Id", onDelete: ReferentialAction.Restrict);
        });

        migrationBuilder.CreateTable("DeviceActivationCodes", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CodeHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
            ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            UsedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
        }, constraints: table => table.PrimaryKey("PK_DeviceActivationCodes", x => x.Id));

        migrationBuilder.CreateTable("Invoices", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            InvoiceNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
            InvoiceDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            PdfFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_Invoices", x => x.Id));

        migrationBuilder.CreateTable("PackageChannels", table => new
        {
            PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_PackageChannels", x => new { x.PackageId, x.ChannelId });
            table.ForeignKey("FK_PackageChannels_Channels_ChannelId", x => x.ChannelId, "Channels", "Id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("FK_PackageChannels_Packages_PackageId", x => x.PackageId, "Packages", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("Payments", table => new
        {
            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PaymentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
            PaymentMethod = table.Column<int>(type: "int", nullable: false),
            PaymentDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
            CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_Payments", x => x.Id));

        CreateIndexes(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in new[] { "PackageChannels", "Payments", "Invoices", "DeviceActivationCodes", "Devices", "Subscriptions", "Channels", "AspNetUserTokens", "AspNetUserRoles", "AspNetUserLogins", "AspNetUserClaims", "AspNetRoleClaims", "SystemSettings", "PlaylistAccessLogs", "Packages", "Customers", "Categories", "AuditLogs", "AspNetUsers", "AspNetRoles" })
        {
            migrationBuilder.DropTable(table);
        }
    }

    private static void CreateIndexes(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", unique: true, filter: "[NormalizedName] IS NOT NULL");
        migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail");
        migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", unique: true, filter: "[NormalizedUserName] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId");
        migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
        migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
        migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
        migrationBuilder.CreateIndex("IX_Categories_Name", "Categories", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_Customers_CustomerNumber", "Customers", "CustomerNumber", unique: true);
        migrationBuilder.CreateIndex("IX_Customers_Email", "Customers", "Email");
        migrationBuilder.CreateIndex("IX_Packages_Name", "Packages", "Name", unique: true);
        migrationBuilder.CreateIndex("IX_SystemSettings_Key", "SystemSettings", "Key", unique: true);
        migrationBuilder.CreateIndex("IX_Channels_CategoryId_Name", "Channels", new[] { "CategoryId", "Name" }, unique: true);
        migrationBuilder.CreateIndex("IX_Subscriptions_CustomerId", "Subscriptions", "CustomerId");
        migrationBuilder.CreateIndex("IX_Subscriptions_PackageId", "Subscriptions", "PackageId");
        migrationBuilder.CreateIndex("IX_Subscriptions_PlaylistTokenHash", "Subscriptions", "PlaylistTokenHash", unique: true, filter: "[PlaylistTokenHash] IS NOT NULL");
        migrationBuilder.CreateIndex("IX_Subscriptions_SubscriptionNumber", "Subscriptions", "SubscriptionNumber", unique: true);
        migrationBuilder.CreateIndex("IX_Devices_CustomerId", "Devices", "CustomerId");
        migrationBuilder.CreateIndex("IX_Devices_SubscriptionId_DeviceIdentifierHash", "Devices", new[] { "SubscriptionId", "DeviceIdentifierHash" }, unique: true);
        migrationBuilder.CreateIndex("IX_DeviceActivationCodes_CodeHash", "DeviceActivationCodes", "CodeHash", unique: true);
        migrationBuilder.CreateIndex("IX_Invoices_InvoiceNumber", "Invoices", "InvoiceNumber", unique: true);
        migrationBuilder.CreateIndex("IX_PackageChannels_ChannelId", "PackageChannels", "ChannelId");
        migrationBuilder.CreateIndex("IX_Payments_PaymentNumber", "Payments", "PaymentNumber", unique: true);
    }
}
