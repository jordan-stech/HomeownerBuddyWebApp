using Microsoft.EntityFrameworkCore.Migrations;

namespace HOB_WebApp.Migrations
{
    public partial class m1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "date",
                table: "MobileUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MaintenanceReminders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReminderItem = table.Column<string>(nullable: true),
                    Reminder = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    NotificationInterval = table.Column<string>(nullable: true),
                    ActionPlanId = table.Column<int>(nullable: false),
                    ActionPlanTitle = table.Column<string>(nullable: true),
                    ActionPlanCategory = table.Column<string>(nullable: true),
                    ActionPlanLink = table.Column<string>(nullable: true),
                    ActionPlanSteps = table.Column<string>(nullable: true),
                    SeasonSpring = table.Column<string>(nullable: true),
                    SeasonSummer = table.Column<string>(nullable: true),
                    SeasonFall = table.Column<string>(nullable: true),
                    SeasonWinter = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceReminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserReminders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReminderId = table.Column<int>(nullable: false),
                    ReminderDescription = table.Column<string>(nullable: true),
                    ReminderItem = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    FName = table.Column<string>(nullable: true),
                    LName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    NotificationInterval = table.Column<string>(nullable: true),
                    SeasonSpring = table.Column<string>(nullable: true),
                    SeasonSummer = table.Column<string>(nullable: true),
                    SeasonFall = table.Column<string>(nullable: true),
                    SeasonWinter = table.Column<string>(nullable: true),
                    ActionPlanId = table.Column<int>(nullable: false),
                    ActionPlanTitle = table.Column<string>(nullable: true),
                    ActionPlanCategory = table.Column<string>(nullable: true),
                    ActionPlanLink = table.Column<string>(nullable: true),
                    ActionPlanSteps = table.Column<string>(nullable: true),
                    Completed = table.Column<string>(nullable: true),
                    Reminder = table.Column<string>(nullable: true),
                    DueDate = table.Column<string>(nullable: true),
                    LastCompleted = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReminders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceReminders");

            migrationBuilder.DropTable(
                name: "UserReminders");

            migrationBuilder.DropColumn(
                name: "date",
                table: "MobileUsers");
        }
    }
}
