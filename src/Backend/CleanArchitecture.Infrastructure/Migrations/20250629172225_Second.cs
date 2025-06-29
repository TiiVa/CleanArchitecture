using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanArchitecture.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_ApplicationUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Resources_ResourceId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_EventResources_Events_EventId",
                table: "EventResources");

            migrationBuilder.DropForeignKey(
                name: "FK_EventResources_Resources_ResourceId",
                table: "EventResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_ApplicationUserId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Invitations_InvitationId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcelFiles_AspNetUsers_ApplicationUserId",
                table: "ExcelFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationSections_Invitations_InvitationId",
                table: "InvitationSections");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_AspNetUsers_ApplicationUserId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_ApplicationUserId",
                table: "Stories");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Events_EventId",
                table: "Stories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMeetingRegistrationForms_Events_EventId",
                table: "UserMeetingRegistrationForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMeetingRegistrationForms",
                table: "UserMeetingRegistrationForms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stories",
                table: "Stories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resources",
                table: "Resources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvitationSections",
                table: "InvitationSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelFiles",
                table: "ExcelFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventResources",
                table: "EventResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "UserMeetingRegistrationForms",
                newName: "EventRegistrationForm");

            migrationBuilder.RenameTable(
                name: "Stories",
                newName: "Story");

            migrationBuilder.RenameTable(
                name: "Resources",
                newName: "Resource");

            migrationBuilder.RenameTable(
                name: "InvitationSections",
                newName: "InvitationSection");

            migrationBuilder.RenameTable(
                name: "Invitations",
                newName: "Invitation");

            migrationBuilder.RenameTable(
                name: "ExcelFiles",
                newName: "ExcelFile");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "EventResources",
                newName: "EventResource");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "Document");

            migrationBuilder.RenameIndex(
                name: "IX_UserMeetingRegistrationForms_EventId",
                table: "EventRegistrationForm",
                newName: "IX_EventRegistrationForm_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Stories_EventId",
                table: "Story",
                newName: "IX_Story_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Stories_ApplicationUserId",
                table: "Story",
                newName: "IX_Story_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Resources_ApplicationUserId",
                table: "Resource",
                newName: "IX_Resource_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_InvitationSections_InvitationId",
                table: "InvitationSection",
                newName: "IX_InvitationSection_InvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelFiles_ApplicationUserId",
                table: "ExcelFile",
                newName: "IX_ExcelFile_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_InvitationId",
                table: "Event",
                newName: "IX_Event_InvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_ApplicationUserId",
                table: "Event",
                newName: "IX_Event_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventResources_ResourceId",
                table: "EventResource",
                newName: "IX_EventResource_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ResourceId",
                table: "Document",
                newName: "IX_Document_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ApplicationUserId",
                table: "Document",
                newName: "IX_Document_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventRegistrationForm",
                table: "EventRegistrationForm",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Story",
                table: "Story",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resource",
                table: "Resource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvitationSection",
                table: "InvitationSection",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelFile",
                table: "ExcelFile",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventResource",
                table: "EventResource",
                columns: new[] { "EventId", "ResourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Document",
                table: "Document",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Document_AspNetUsers_ApplicationUserId",
                table: "Document",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Document_Resource_ResourceId",
                table: "Document",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_AspNetUsers_ApplicationUserId",
                table: "Event",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Invitation_InvitationId",
                table: "Event",
                column: "InvitationId",
                principalTable: "Invitation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventRegistrationForm_Event_EventId",
                table: "EventRegistrationForm",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventResource_Event_EventId",
                table: "EventResource",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventResource_Resource_ResourceId",
                table: "EventResource",
                column: "ResourceId",
                principalTable: "Resource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelFile_AspNetUsers_ApplicationUserId",
                table: "ExcelFile",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationSection_Invitation_InvitationId",
                table: "InvitationSection",
                column: "InvitationId",
                principalTable: "Invitation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_AspNetUsers_ApplicationUserId",
                table: "Resource",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Story_AspNetUsers_ApplicationUserId",
                table: "Story",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Story_Event_EventId",
                table: "Story",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_AspNetUsers_ApplicationUserId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Document_Resource_ResourceId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_AspNetUsers_ApplicationUserId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Invitation_InvitationId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventRegistrationForm_Event_EventId",
                table: "EventRegistrationForm");

            migrationBuilder.DropForeignKey(
                name: "FK_EventResource_Event_EventId",
                table: "EventResource");

            migrationBuilder.DropForeignKey(
                name: "FK_EventResource_Resource_ResourceId",
                table: "EventResource");

            migrationBuilder.DropForeignKey(
                name: "FK_ExcelFile_AspNetUsers_ApplicationUserId",
                table: "ExcelFile");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationSection_Invitation_InvitationId",
                table: "InvitationSection");

            migrationBuilder.DropForeignKey(
                name: "FK_Resource_AspNetUsers_ApplicationUserId",
                table: "Resource");

            migrationBuilder.DropForeignKey(
                name: "FK_Story_AspNetUsers_ApplicationUserId",
                table: "Story");

            migrationBuilder.DropForeignKey(
                name: "FK_Story_Event_EventId",
                table: "Story");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Story",
                table: "Story");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resource",
                table: "Resource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvitationSection",
                table: "InvitationSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExcelFile",
                table: "ExcelFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventResource",
                table: "EventResource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventRegistrationForm",
                table: "EventRegistrationForm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Document",
                table: "Document");

            migrationBuilder.RenameTable(
                name: "Story",
                newName: "Stories");

            migrationBuilder.RenameTable(
                name: "Resource",
                newName: "Resources");

            migrationBuilder.RenameTable(
                name: "InvitationSection",
                newName: "InvitationSections");

            migrationBuilder.RenameTable(
                name: "Invitation",
                newName: "Invitations");

            migrationBuilder.RenameTable(
                name: "ExcelFile",
                newName: "ExcelFiles");

            migrationBuilder.RenameTable(
                name: "EventResource",
                newName: "EventResources");

            migrationBuilder.RenameTable(
                name: "EventRegistrationForm",
                newName: "UserMeetingRegistrationForms");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "Document",
                newName: "Documents");

            migrationBuilder.RenameIndex(
                name: "IX_Story_EventId",
                table: "Stories",
                newName: "IX_Stories_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Story_ApplicationUserId",
                table: "Stories",
                newName: "IX_Stories_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Resource_ApplicationUserId",
                table: "Resources",
                newName: "IX_Resources_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_InvitationSection_InvitationId",
                table: "InvitationSections",
                newName: "IX_InvitationSections_InvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_ExcelFile_ApplicationUserId",
                table: "ExcelFiles",
                newName: "IX_ExcelFiles_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventResource_ResourceId",
                table: "EventResources",
                newName: "IX_EventResources_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_EventRegistrationForm_EventId",
                table: "UserMeetingRegistrationForms",
                newName: "IX_UserMeetingRegistrationForms_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_InvitationId",
                table: "Events",
                newName: "IX_Events_InvitationId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_ApplicationUserId",
                table: "Events",
                newName: "IX_Events_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ResourceId",
                table: "Documents",
                newName: "IX_Documents_ResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_Document_ApplicationUserId",
                table: "Documents",
                newName: "IX_Documents_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stories",
                table: "Stories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resources",
                table: "Resources",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvitationSections",
                table: "InvitationSections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExcelFiles",
                table: "ExcelFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventResources",
                table: "EventResources",
                columns: new[] { "EventId", "ResourceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMeetingRegistrationForms",
                table: "UserMeetingRegistrationForms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_ApplicationUserId",
                table: "Documents",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Resources_ResourceId",
                table: "Documents",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventResources_Events_EventId",
                table: "EventResources",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventResources_Resources_ResourceId",
                table: "EventResources",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_ApplicationUserId",
                table: "Events",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Invitations_InvitationId",
                table: "Events",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExcelFiles_AspNetUsers_ApplicationUserId",
                table: "ExcelFiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationSections_Invitations_InvitationId",
                table: "InvitationSections",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_AspNetUsers_ApplicationUserId",
                table: "Resources",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_ApplicationUserId",
                table: "Stories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Events_EventId",
                table: "Stories",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMeetingRegistrationForms_Events_EventId",
                table: "UserMeetingRegistrationForms",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
