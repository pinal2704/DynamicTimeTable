using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DynamicTimeTable.Migrations
{
    /// <inheritdoc />
    public partial class Dbtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimetableConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    SubjectsPerDay = table.Column<int>(type: "int", nullable: false),
                    TotalSubjects = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hours = table.Column<int>(type: "int", nullable: false),
                    TimetableConfigurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_TimetableConfigurations_TimetableConfigurationId",
                        column: x => x.TimetableConfigurationId,
                        principalTable: "TimetableConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimetableDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimetableJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimetableConfigurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableDatas_TimetableConfigurations_TimetableConfigurationId",
                        column: x => x.TimetableConfigurationId,
                        principalTable: "TimetableConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_TimetableConfigurationId",
                table: "Subjects",
                column: "TimetableConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableDatas_TimetableConfigurationId",
                table: "TimetableDatas",
                column: "TimetableConfigurationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "TimetableDatas");

            migrationBuilder.DropTable(
                name: "TimetableConfigurations");
        }
    }
}
