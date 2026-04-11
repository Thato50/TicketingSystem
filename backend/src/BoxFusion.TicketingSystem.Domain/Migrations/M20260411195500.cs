using System;
using FluentMigrator;
using Shesha.FluentMigrator;

namespace BoxFusion.TicketingSystem.Domain.Migrations
{
    [Migration(20260411195500)]
    public class M20260411195500 : Migration
    {
        public override void Up()
        {
            // Create Tickets table
            Create.Table("TicketingSystem_Tickets")
                .WithIdAsGuid()
                .WithFullAuditColumns()
                .WithColumn("Title").AsString(200).NotNullable()
                .WithColumn("Description").AsString(2000).NotNullable()
                .WithColumn("CategoryLkp").AsInt64().Nullable()
                .WithColumn("PriorityLkp").AsInt64().Nullable()
                .WithColumn("StatusLkp").AsInt64().Nullable();

            Alter.Table("TicketingSystem_Tickets")
                .AddForeignKeyColumn("RequesterId", "Core_Persons").Nullable();

            Alter.Table("TicketingSystem_Tickets")
                .AddForeignKeyColumn("AssignedToId", "Core_Persons").Nullable();


            // Create TicketComments table
            Create.Table("TicketingSystem_TicketComments")
                .WithIdAsGuid()
                .WithFullAuditColumns()
                .WithColumn("CommentText").AsString(2000).NotNullable();

            Alter.Table("TicketingSystem_TicketComments")
                .AddForeignKeyColumn("TicketId", "TicketingSystem_Tickets").NotNullable();

            Alter.Table("TicketingSystem_TicketComments")
                .AddForeignKeyColumn("AuthorId", "Core_Persons").Nullable();
        }

        public override void Down()
        {
            // Optional (cleanup if rollback)
            Delete.Table("TicketingSystem_TicketComments");
            Delete.Table("TicketingSystem_Tickets");
        }
    }
}