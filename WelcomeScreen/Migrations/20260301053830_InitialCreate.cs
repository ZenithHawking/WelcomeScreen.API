using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WelcomeScreen.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WelcomeScreens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomeScreens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WelcomeScreens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WelcomeScreenId = table.Column<int>(type: "int", nullable: false),
                    DbType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionStringEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TriggerColumn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastPolledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSources_WelcomeScreens_WelcomeScreenId",
                        column: x => x.WelcomeScreenId,
                        principalTable: "WelcomeScreens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WelcomeScreenId = table.Column<int>(type: "int", nullable: false),
                    SourceColumn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    FontFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontSize = table.Column<int>(type: "int", nullable: false),
                    FontColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontWeight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextAlign = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionX = table.Column<double>(type: "float", nullable: false),
                    PositionY = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    AnimationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnimationDuration = table.Column<int>(type: "int", nullable: false),
                    AnimationDelay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventFields_WelcomeScreens_WelcomeScreenId",
                        column: x => x.WelcomeScreenId,
                        principalTable: "WelcomeScreens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WelcomeScreenId = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFiles_WelcomeScreens_WelcomeScreenId",
                        column: x => x.WelcomeScreenId,
                        principalTable: "WelcomeScreens",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScreenConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WelcomeScreenId = table.Column<int>(type: "int", nullable: false),
                    BackgroundMediaId = table.Column<int>(type: "int", nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanvasWidth = table.Column<int>(type: "int", nullable: false),
                    CanvasHeight = table.Column<int>(type: "int", nullable: false),
                    LayoutJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenConfigs_MediaFiles_BackgroundMediaId",
                        column: x => x.BackgroundMediaId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScreenConfigs_WelcomeScreens_WelcomeScreenId",
                        column: x => x.WelcomeScreenId,
                        principalTable: "WelcomeScreens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_WelcomeScreenId",
                table: "DataSources",
                column: "WelcomeScreenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventFields_WelcomeScreenId",
                table: "EventFields",
                column: "WelcomeScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_WelcomeScreenId",
                table: "MediaFiles",
                column: "WelcomeScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenConfigs_BackgroundMediaId",
                table: "ScreenConfigs",
                column: "BackgroundMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenConfigs_WelcomeScreenId",
                table: "ScreenConfigs",
                column: "WelcomeScreenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WelcomeScreens_UserId",
                table: "WelcomeScreens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSources");

            migrationBuilder.DropTable(
                name: "EventFields");

            migrationBuilder.DropTable(
                name: "ScreenConfigs");

            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.DropTable(
                name: "WelcomeScreens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
