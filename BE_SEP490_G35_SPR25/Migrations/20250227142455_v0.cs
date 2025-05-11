using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planify_BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class v0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Campus__3214EC07699F6314", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MediaIte__3214EC07B5BC8118", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Province__3214EC077E6D8019", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__3214EC0791A7DB13", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryEventName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CampusID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__3214EC07B017394E", x => x.Id);
                    table.ForeignKey(
                        name: "FK__CategoryE__Campu__3B75D760",
                        column: x => x.CampusID,
                        principalTable: "Campus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Avatar = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__District__3214EC07F051D900", x => x.Id);
                    table.ForeignKey(
                        name: "FK__District__Provin__403A8C7D",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WardName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DistrictID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ward__3214EC07CBA258CF", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Ward__DistrictID__4316F928",
                        column: x => x.DistrictID,
                        principalTable: "District",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    UserName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    WardId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    Avatar = table.Column<int>(type: "int", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Role = table.Column<int>(type: "int", nullable: true),
                    CreateByAdmin = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateByManage = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampusId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC079CA1F7F4", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Users__Avatar__4F7CD00D",
                        column: x => x.Avatar,
                        principalTable: "MediaItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__CampusId__5441852A",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__CreateByA__52593CB8",
                        column: x => x.CreateByAdmin,
                        principalTable: "Admin",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__CreateByM__534D60F1",
                        column: x => x.CreateByManage,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__DistrictI__4D94879B",
                        column: x => x.DistrictId,
                        principalTable: "District",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__ProvinceI__4E88ABD4",
                        column: x => x.ProvinceId,
                        principalTable: "Province",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__Role__5165187F",
                        column: x => x.Role,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Users__WardId__4CA06362",
                        column: x => x.WardId,
                        principalTable: "Ward",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    TimeOfEvent = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndOfEvent = table.Column<DateTime>(type: "datetime", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    AmountBudget = table.Column<decimal>(type: "money", nullable: false),
                    IsPublic = table.Column<int>(type: "int", nullable: false),
                    TimePublic = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CampusId = table.Column<int>(type: "int", nullable: true),
                    CategoryEventId = table.Column<int>(type: "int", nullable: true),
                    Placed = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event__3214EC07C3C53BBD", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Event__CampusId__59FA5E80",
                        column: x => x.CampusId,
                        principalTable: "Campus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Event__CategoryE__5AEE82B9",
                        column: x => x.CategoryEventId,
                        principalTable: "CategoryEvent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Event__CreateBy__571DF1D5",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Event__ManagerId__59063A47",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    MediaId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventMed__3214EC07C361BBF6", x => x.Id);
                    table.ForeignKey(
                        name: "FK__EventMedi__Event__619B8048",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__EventMedi__Media__628FA481",
                        column: x => x.MediaId,
                        principalTable: "MediaItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    AmountBudget = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Group__3214EC073340868D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Group__CreateBy__6B24EA82",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Group__EventId__6C190EBB",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JoinProject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TimeJoinProject = table.Column<DateTime>(type: "datetime", nullable: false),
                    TimeOutProject = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    Role = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JoinProj__3214EC0730D721C5", x => x.Id);
                    table.ForeignKey(
                        name: "FK__JoinProje__Event__656C112C",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__JoinProje__UserI__66603565",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__JoinProjec__Role__68487DD7",
                        column: x => x.Role,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SendRequ__3214EC07A267419B", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SendReque__Event__5DCAEF64",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__SendReque__Manag__5EBF139D",
                        column: x => x.ManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaskName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    AmountBudget = table.Column<decimal>(type: "money", nullable: false),
                    Progress = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Task__3214EC07522E353D", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Task__CreateBy__6EF57B66",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Task__GroupId__70DDC3D8",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceImagesTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    InvoiceImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActualBudgetAmount = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvoiceI__3214EC074D20B675", x => x.Id);
                    table.ForeignKey(
                        name: "FK__InvoiceIm__TaskI__7A672E12",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubTaskName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SubTaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    AmountBudget = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SubTask__3214EC07F2A302FC", x => x.Id);
                    table.ForeignKey(
                        name: "FK__SubTask__CreateB__74AE54BC",
                        column: x => x.CreateBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__SubTask__TaskId__73BA3083",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceImagesSubTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubTaskId = table.Column<int>(type: "int", nullable: true),
                    InvoiceImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActualBudgetAmount = table.Column<decimal>(type: "money", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__InvoiceI__3214EC0707FDF766", x => x.Id);
                    table.ForeignKey(
                        name: "FK__InvoiceIm__SubTa__778AC167",
                        column: x => x.SubTaskId,
                        principalTable: "SubTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_Avatar",
                table: "Admin",
                column: "Avatar");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEvent_CampusID",
                table: "CategoryEvent",
                column: "CampusID");

            migrationBuilder.CreateIndex(
                name: "IX_District_ProvinceId",
                table: "District",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CampusId",
                table: "Event",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CategoryEventId",
                table: "Event",
                column: "CategoryEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreateBy",
                table: "Event",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Event_ManagerId",
                table: "Event",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedia_EventId",
                table: "EventMedia",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventMedia_MediaId",
                table: "EventMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_CreateBy",
                table: "Group",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Group_EventId",
                table: "Group",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceImagesSubTask_SubTaskId",
                table: "InvoiceImagesSubTask",
                column: "SubTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceImagesTask_TaskId",
                table: "InvoiceImagesTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinProject_EventId",
                table: "JoinProject",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinProject_Role",
                table: "JoinProject",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_JoinProject_UserId",
                table: "JoinProject",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SendRequest_EventId",
                table: "SendRequest",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SendRequest_ManagerId",
                table: "SendRequest",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_CreateBy",
                table: "SubTask",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_TaskId",
                table: "SubTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreateBy",
                table: "Task",
                column: "CreateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Task_GroupId",
                table: "Task",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Avatar",
                table: "Users",
                column: "Avatar");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CampusId",
                table: "Users",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreateByAdmin",
                table: "Users",
                column: "CreateByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreateByManage",
                table: "Users",
                column: "CreateByManage");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DistrictId",
                table: "Users",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProvinceId",
                table: "Users",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Role",
                table: "Users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WardId",
                table: "Users",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Ward_DistrictID",
                table: "Ward",
                column: "DistrictID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventMedia");

            migrationBuilder.DropTable(
                name: "InvoiceImagesSubTask");

            migrationBuilder.DropTable(
                name: "InvoiceImagesTask");

            migrationBuilder.DropTable(
                name: "JoinProject");

            migrationBuilder.DropTable(
                name: "SendRequest");

            migrationBuilder.DropTable(
                name: "SubTask");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "CategoryEvent");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Campus");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Ward");

            migrationBuilder.DropTable(
                name: "MediaItems");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
