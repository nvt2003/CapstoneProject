using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planify_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__CategoryE__Campu__3B75D760",
                table: "CategoryEvent");

            migrationBuilder.DropForeignKey(
                name: "FK__District__Provin__403A8C7D",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CampusId__59FA5E80",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CategoryE__5AEE82B9",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CreateBy__571DF1D5",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__ManagerId__59063A47",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__EventMedi__Event__619B8048",
                table: "EventMedia");

            migrationBuilder.DropForeignKey(
                name: "FK__EventMedi__Media__628FA481",
                table: "EventMedia");

            migrationBuilder.DropForeignKey(
                name: "FK__Group__CreateBy__6B24EA82",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK__Group__EventId__6C190EBB",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK__InvoiceIm__SubTa__778AC167",
                table: "InvoiceImagesSubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__InvoiceIm__TaskI__7A672E12",
                table: "InvoiceImagesTask");

            migrationBuilder.DropForeignKey(
                name: "FK__JoinProje__Event__656C112C",
                table: "JoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK__JoinProje__UserI__66603565",
                table: "JoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK__JoinProjec__Role__68487DD7",
                table: "JoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK__SendReque__Event__5DCAEF64",
                table: "SendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK__SendReque__Manag__5EBF139D",
                table: "SendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK__SubTask__CreateB__74AE54BC",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__SubTask__TaskId__73BA3083",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__Task__CreateBy__6EF57B66",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK__Task__GroupId__70DDC3D8",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__Avatar__4F7CD00D",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__CampusId__5441852A",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__CreateByA__52593CB8",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__CreateByM__534D60F1",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__DistrictI__4D94879B",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__ProvinceI__4E88ABD4",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__Role__5165187F",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__WardId__4CA06362",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Ward__DistrictID__4316F928",
                table: "Ward");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Ward__3214EC07CBA258CF",
                table: "Ward");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Users__3214EC079CA1F7F4",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreateByAdmin",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreateByManage",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Task__3214EC07522E353D",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK__SubTask__3214EC07F2A302FC",
                table: "SubTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__SendRequ__3214EC07A267419B",
                table: "SendRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__3214EC0791A7DB13",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Province__3214EC077E6D8019",
                table: "Province");

            migrationBuilder.DropPrimaryKey(
                name: "PK__MediaIte__3214EC07B5BC8118",
                table: "MediaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK__JoinProj__3214EC0730D721C5",
                table: "JoinProject");

            migrationBuilder.DropIndex(
                name: "IX_JoinProject_Role",
                table: "JoinProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK__InvoiceI__3214EC074D20B675",
                table: "InvoiceImagesTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__InvoiceI__3214EC0707FDF766",
                table: "InvoiceImagesSubTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Group__3214EC073340868D",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK__EventMed__3214EC07C361BBF6",
                table: "EventMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Event__3214EC07C3C53BBD",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK__District__3214EC07F051D900",
                table: "District");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Category__3214EC07B017394E",
                table: "CategoryEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Campus__3214EC07699F6314",
                table: "Campus");

            migrationBuilder.DropColumn(
                name: "CreateByAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateByManage",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DistrictID",
                table: "Ward",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Ward_DistrictID",
                table: "Ward",
                newName: "IX_Ward_DistrictId");

            migrationBuilder.RenameColumn(
                name: "CampusID",
                table: "CategoryEvent",
                newName: "CampusId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryEvent_CampusID",
                table: "CategoryEvent",
                newName: "IX_CategoryEvent_CampusId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Ward",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                table: "Users",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "Task",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Task",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<double>(
                name: "Progress",
                table: "Task",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Task",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Task",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "SubTaskName",
                table: "SubTask",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SubTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "SubTask",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "SubTask",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "SubTask",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SendRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Province",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MediaURL",
                table: "MediaItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeJoinProject",
                table: "JoinProject",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InvoiceImagesTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceImageURL",
                table: "InvoiceImagesTask",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualBudgetAmount",
                table: "InvoiceImagesTask",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InvoiceImagesSubTask",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceImageURL",
                table: "InvoiceImagesSubTask",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualBudgetAmount",
                table: "InvoiceImagesSubTask",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Group",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Group",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "EventMedia",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimePublic",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeOfEvent",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Event",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "Placed",
                table: "Event",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "IsPublic",
                table: "Event",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndOfEvent",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                table: "Event",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Event",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "District",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CategoryEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Campus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Ward__3214EC07B5ACCBA1",
                table: "Ward",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Users__3214EC07FEA36B21",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Task__3214EC0750EFAB84",
                table: "Task",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__SubTask__3214EC0768B35C66",
                table: "SubTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__SendRequ__3214EC075EEE23D7",
                table: "SendRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__3214EC07BAF9B1A7",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Province__3214EC0742D4AAED",
                table: "Province",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__MediaIte__3214EC074378424F",
                table: "MediaItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__JoinProj__3214EC07659A18CC",
                table: "JoinProject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__InvoiceI__3214EC07B5054C75",
                table: "InvoiceImagesTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__InvoiceI__3214EC079F7723FF",
                table: "InvoiceImagesSubTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Group__3214EC079D0EF3F3",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__EventMed__3214EC078B36DE97",
                table: "EventMedia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Event__3214EC07879E40D1",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__District__3214EC074E4FB66C",
                table: "District",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Category__3214EC07313FA59A",
                table: "CategoryEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Campus__3214EC07A0547913",
                table: "Campus",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AssignTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubTaskId = table.Column<int>(type: "int", nullable: true),
                    TimeJoin = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeOut = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AssignTa__3214EC07D4FEF93A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__AssignTas__Assig__7C4F7684",
                        column: x => x.AssignId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__AssignTas__SubTa__7D439ABD",
                        column: x => x.SubTaskId,
                        principalTable: "SubTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JoinGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImplementerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    TimeJoin = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeOut = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JoinGrou__3214EC07C7D52D4A", x => x.Id);
                    table.ForeignKey(
                        name: "FK__JoinGroup__Group__75A278F5",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__JoinGroup__Imple__74AE54BC",
                        column: x => x.ImplementerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JoinTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    ImplementerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeJoin = table.Column<DateTime>(type: "datetime", nullable: true),
                    TimeOut = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JoinTask__3214EC07C883397F", x => x.Id);
                    table.ForeignKey(
                        name: "FK__JoinTask__Implem__797309D9",
                        column: x => x.ImplementerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__JoinTask__TaskId__787EE5A0",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendFrom = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReportUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SendTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Report__3214EC07305FD60E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Report__ReportUs__6EF57B66",
                        column: x => x.ReportUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Report__SendFrom__6D0D32F4",
                        column: x => x.SendFrom,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Report__TaskId__6E01572D",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReportMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    MediaURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ReportMe__3214EC07636296D0", x => x.Id);
                    table.ForeignKey(
                        name: "FK__ReportMed__Repor__71D1E811",
                        column: x => x.ReportId,
                        principalTable: "Report",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignTask_AssignId",
                table: "AssignTask",
                column: "AssignId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignTask_SubTaskId",
                table: "AssignTask",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroup_GroupId",
                table: "JoinGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroup_ImplementerId",
                table: "JoinGroup",
                column: "ImplementerId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinTask_ImplementerId",
                table: "JoinTask",
                column: "ImplementerId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinTask_TaskId",
                table: "JoinTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportUserId",
                table: "Report",
                column: "ReportUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_SendFrom",
                table: "Report",
                column: "SendFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Report_TaskId",
                table: "Report",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportMedia_ReportId",
                table: "ReportMedia",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK__CategoryE__Campu__44FF419A",
                table: "CategoryEvent",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__District__Provin__3F466844",
                table: "District",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CampusId__5165187F",
                table: "Event",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CategoryE__52593CB8",
                table: "Event",
                column: "CategoryEventId",
                principalTable: "CategoryEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CreateBy__4F7CD00D",
                table: "Event",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__ManagerId__5070F446",
                table: "Event",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__EventMedi__Event__5535A963",
                table: "EventMedia",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__EventMedi__Media__5629CD9C",
                table: "EventMedia",
                column: "MediaId",
                principalTable: "MediaItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Group__CreateBy__619B8048",
                table: "Group",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Group__EventId__628FA481",
                table: "Group",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__InvoiceIm__SubTa__00200768",
                table: "InvoiceImagesSubTask",
                column: "SubTaskId",
                principalTable: "SubTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__InvoiceIm__TaskI__02FC7413",
                table: "InvoiceImagesTask",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__JoinProje__Event__5DCAEF64",
                table: "JoinProject",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__JoinProje__UserI__5EBF139D",
                table: "JoinProject",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SendReque__Event__59063A47",
                table: "SendRequest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SendReque__Manag__59FA5E80",
                table: "SendRequest",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SubTask__CreateB__6A30C649",
                table: "SubTask",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SubTask__TaskId__693CA210",
                table: "SubTask",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Task__CreateBy__656C112C",
                table: "Task",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Task__GroupId__66603565",
                table: "Task",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__Avatar__4AB81AF0",
                table: "Users",
                column: "Avatar",
                principalTable: "MediaItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__CampusId__4CA06362",
                table: "Users",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__DistrictI__48CFD27E",
                table: "Users",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__ProvinceI__49C3F6B7",
                table: "Users",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__Role__4BAC3F29",
                table: "Users",
                column: "Role",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__WardId__47DBAE45",
                table: "Users",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Ward__DistrictId__4222D4EF",
                table: "Ward",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__CategoryE__Campu__44FF419A",
                table: "CategoryEvent");

            migrationBuilder.DropForeignKey(
                name: "FK__District__Provin__3F466844",
                table: "District");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CampusId__5165187F",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CategoryE__52593CB8",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__CreateBy__4F7CD00D",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__Event__ManagerId__5070F446",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK__EventMedi__Event__5535A963",
                table: "EventMedia");

            migrationBuilder.DropForeignKey(
                name: "FK__EventMedi__Media__5629CD9C",
                table: "EventMedia");

            migrationBuilder.DropForeignKey(
                name: "FK__Group__CreateBy__619B8048",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK__Group__EventId__628FA481",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK__InvoiceIm__SubTa__00200768",
                table: "InvoiceImagesSubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__InvoiceIm__TaskI__02FC7413",
                table: "InvoiceImagesTask");

            migrationBuilder.DropForeignKey(
                name: "FK__JoinProje__Event__5DCAEF64",
                table: "JoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK__JoinProje__UserI__5EBF139D",
                table: "JoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK__SendReque__Event__59063A47",
                table: "SendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK__SendReque__Manag__59FA5E80",
                table: "SendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK__SubTask__CreateB__6A30C649",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__SubTask__TaskId__693CA210",
                table: "SubTask");

            migrationBuilder.DropForeignKey(
                name: "FK__Task__CreateBy__656C112C",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK__Task__GroupId__66603565",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__Avatar__4AB81AF0",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__CampusId__4CA06362",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__DistrictI__48CFD27E",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__ProvinceI__49C3F6B7",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__Role__4BAC3F29",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Users__WardId__47DBAE45",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK__Ward__DistrictId__4222D4EF",
                table: "Ward");

            migrationBuilder.DropTable(
                name: "AssignTask");

            migrationBuilder.DropTable(
                name: "JoinGroup");

            migrationBuilder.DropTable(
                name: "JoinTask");

            migrationBuilder.DropTable(
                name: "ReportMedia");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Ward__3214EC07B5ACCBA1",
                table: "Ward");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Users__3214EC07FEA36B21",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Task__3214EC0750EFAB84",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK__SubTask__3214EC0768B35C66",
                table: "SubTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__SendRequ__3214EC075EEE23D7",
                table: "SendRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Role__3214EC07BAF9B1A7",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Province__3214EC0742D4AAED",
                table: "Province");

            migrationBuilder.DropPrimaryKey(
                name: "PK__MediaIte__3214EC074378424F",
                table: "MediaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK__JoinProj__3214EC07659A18CC",
                table: "JoinProject");

            migrationBuilder.DropPrimaryKey(
                name: "PK__InvoiceI__3214EC07B5054C75",
                table: "InvoiceImagesTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__InvoiceI__3214EC079F7723FF",
                table: "InvoiceImagesSubTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Group__3214EC079D0EF3F3",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK__EventMed__3214EC078B36DE97",
                table: "EventMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Event__3214EC07879E40D1",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK__District__3214EC074E4FB66C",
                table: "District");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Category__3214EC07313FA59A",
                table: "CategoryEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Campus__3214EC07A0547913",
                table: "Campus");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Ward",
                newName: "DistrictID");

            migrationBuilder.RenameIndex(
                name: "IX_Ward_DistrictId",
                table: "Ward",
                newName: "IX_Ward_DistrictID");

            migrationBuilder.RenameColumn(
                name: "CampusId",
                table: "CategoryEvent",
                newName: "CampusID");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryEvent_CampusId",
                table: "CategoryEvent",
                newName: "IX_CategoryEvent_CampusID");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Ward",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateByAdmin",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreateByManage",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "Task",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Task",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Progress",
                table: "Task",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Task",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "Task",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Task",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubTaskName",
                table: "SubTask",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SubTask",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "SubTask",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "SubTask",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "SubTask",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SendRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Province",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MediaURL",
                table: "MediaItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeJoinProject",
                table: "JoinProject",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InvoiceImagesTask",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceImageURL",
                table: "InvoiceImagesTask",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualBudgetAmount",
                table: "InvoiceImagesTask",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InvoiceImagesSubTask",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceImageURL",
                table: "InvoiceImagesSubTask",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ActualBudgetAmount",
                table: "InvoiceImagesSubTask",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "Group",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Group",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "EventMedia",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimePublic",
                table: "Event",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeOfEvent",
                table: "Event",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Event",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Placed",
                table: "Event",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsPublic",
                table: "Event",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Event",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndOfEvent",
                table: "Event",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                table: "Event",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountBudget",
                table: "Event",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "District",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CategoryEvent",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Campus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Ward__3214EC07CBA258CF",
                table: "Ward",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Users__3214EC079CA1F7F4",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Task__3214EC07522E353D",
                table: "Task",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__SubTask__3214EC07F2A302FC",
                table: "SubTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__SendRequ__3214EC07A267419B",
                table: "SendRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Role__3214EC0791A7DB13",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Province__3214EC077E6D8019",
                table: "Province",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__MediaIte__3214EC07B5BC8118",
                table: "MediaItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__JoinProj__3214EC0730D721C5",
                table: "JoinProject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__InvoiceI__3214EC074D20B675",
                table: "InvoiceImagesTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__InvoiceI__3214EC0707FDF766",
                table: "InvoiceImagesSubTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Group__3214EC073340868D",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__EventMed__3214EC07C361BBF6",
                table: "EventMedia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Event__3214EC07C3C53BBD",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__District__3214EC07F051D900",
                table: "District",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Category__3214EC07B017394E",
                table: "CategoryEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Campus__3214EC07699F6314",
                table: "Campus",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Avatar = table.Column<int>(type: "int", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__3214EC078C6BBC5B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Admin__Avatar__48CFD27E",
                        column: x => x.Avatar,
                        principalTable: "MediaItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreateByAdmin",
                table: "Users",
                column: "CreateByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreateByManage",
                table: "Users",
                column: "CreateByManage");

            migrationBuilder.CreateIndex(
                name: "IX_JoinProject_Role",
                table: "JoinProject",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_Avatar",
                table: "Admin",
                column: "Avatar");

            migrationBuilder.AddForeignKey(
                name: "FK__CategoryE__Campu__3B75D760",
                table: "CategoryEvent",
                column: "CampusID",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__District__Provin__403A8C7D",
                table: "District",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CampusId__59FA5E80",
                table: "Event",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CategoryE__5AEE82B9",
                table: "Event",
                column: "CategoryEventId",
                principalTable: "CategoryEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__CreateBy__571DF1D5",
                table: "Event",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Event__ManagerId__59063A47",
                table: "Event",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__EventMedi__Event__619B8048",
                table: "EventMedia",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__EventMedi__Media__628FA481",
                table: "EventMedia",
                column: "MediaId",
                principalTable: "MediaItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Group__CreateBy__6B24EA82",
                table: "Group",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Group__EventId__6C190EBB",
                table: "Group",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__InvoiceIm__SubTa__778AC167",
                table: "InvoiceImagesSubTask",
                column: "SubTaskId",
                principalTable: "SubTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__InvoiceIm__TaskI__7A672E12",
                table: "InvoiceImagesTask",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__JoinProje__Event__656C112C",
                table: "JoinProject",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__JoinProje__UserI__66603565",
                table: "JoinProject",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__JoinProjec__Role__68487DD7",
                table: "JoinProject",
                column: "Role",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SendReque__Event__5DCAEF64",
                table: "SendRequest",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SendReque__Manag__5EBF139D",
                table: "SendRequest",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SubTask__CreateB__74AE54BC",
                table: "SubTask",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__SubTask__TaskId__73BA3083",
                table: "SubTask",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Task__CreateBy__6EF57B66",
                table: "Task",
                column: "CreateBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Task__GroupId__70DDC3D8",
                table: "Task",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__Avatar__4F7CD00D",
                table: "Users",
                column: "Avatar",
                principalTable: "MediaItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__CampusId__5441852A",
                table: "Users",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__CreateByA__52593CB8",
                table: "Users",
                column: "CreateByAdmin",
                principalTable: "Admin",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__CreateByM__534D60F1",
                table: "Users",
                column: "CreateByManage",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__DistrictI__4D94879B",
                table: "Users",
                column: "DistrictId",
                principalTable: "District",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__ProvinceI__4E88ABD4",
                table: "Users",
                column: "ProvinceId",
                principalTable: "Province",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__Role__5165187F",
                table: "Users",
                column: "Role",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Users__WardId__4CA06362",
                table: "Users",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Ward__DistrictID__4316F928",
                table: "Ward",
                column: "DistrictID",
                principalTable: "District",
                principalColumn: "Id");
        }
    }
}
