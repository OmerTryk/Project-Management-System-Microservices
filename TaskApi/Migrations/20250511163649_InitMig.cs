using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskApi.Migrations
{
    /// <inheritdoc />
    public partial class InitMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: true),
                    ActualMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActivityTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskActivities_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskKeywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false, defaultValue: 1.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskKeywords_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskPerformanceRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceScore = table.Column<double>(type: "float", nullable: false),
                    SimilarityScore = table.Column<double>(type: "float", nullable: false),
                    TimeEfficiencyScore = table.Column<double>(type: "float", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualMinutes = table.Column<int>(type: "int", nullable: false),
                    AverageMinutesForSimilarTasks = table.Column<int>(type: "int", nullable: false),
                    SimilarTasksCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPerformanceRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskPerformanceRatings_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimilarTaskReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceRatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimilarTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimilarityPercentage = table.Column<double>(type: "float", nullable: false),
                    SimilarTaskDurationMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarTaskReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimilarTaskReferences_TaskPerformanceRatings_PerformanceRatingId",
                        column: x => x.PerformanceRatingId,
                        principalTable: "TaskPerformanceRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimilarTaskReferences_PerformanceRatingId",
                table: "SimilarTaskReferences",
                column: "PerformanceRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskActivities_TaskId",
                table: "TaskActivities",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskKeywords_Keyword",
                table: "TaskKeywords",
                column: "Keyword");

            migrationBuilder.CreateIndex(
                name: "IX_TaskKeywords_TaskId",
                table: "TaskKeywords",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskPerformanceRatings_TaskId",
                table: "TaskPerformanceRatings",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimilarTaskReferences");

            migrationBuilder.DropTable(
                name: "TaskActivities");

            migrationBuilder.DropTable(
                name: "TaskKeywords");

            migrationBuilder.DropTable(
                name: "TaskPerformanceRatings");

            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
