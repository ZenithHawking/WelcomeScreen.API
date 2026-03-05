using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WelcomeScreen.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPolledId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPolledAt",
                table: "DataSources");

            migrationBuilder.AddColumn<long>(
                name: "LastPolledId",
                table: "DataSources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPolledId",
                table: "DataSources");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPolledAt",
                table: "DataSources",
                type: "datetime2",
                nullable: true);
        }
    }
}
