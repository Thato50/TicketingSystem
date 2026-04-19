using FluentMigrator;

namespace BoxFusion.TicketingSystem.Domain.Migrations
{
    [Migration(20260418193000)]
    public class M20260418193000 : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
IF OBJECT_ID(N'dbo.Frwk_FormConfigurations', N'U') IS NOT NULL
AND OBJECT_ID(N'dbo.Frwk_ConfigurationItems', N'U') IS NOT NULL
BEGIN
    -- Normalize the ticket module to pure entity-based Shesha forms.
    UPDATE fc
    SET
        fc.ModelType = N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket',
        fc.GenerationLogicExtensionJson = REPLACE(
            REPLACE(
                COALESCE(fc.GenerationLogicExtensionJson, N''),
                N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketInput',
                N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket'
            ),
            N'BoxFusion.TicketingSystem.Application.Tickets.TicketDto',
            N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket'
        ),
        fc.Markup = REPLACE(
            REPLACE(
                COALESCE(fc.Markup, N''),
                N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketInput',
                N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket'
            ),
            N'BoxFusion.TicketingSystem.Application.Tickets.TicketDto',
            N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket'
        )
    FROM Frwk_FormConfigurations fc
    INNER JOIN Frwk_ConfigurationItems ci ON ci.Id = fc.Id
    WHERE ci.Name IN (N'New-Ticket', N'My-Tickets', N'Ticket-Details', N'Ticket_details', N'TicketList');

    UPDATE fc
    SET
        fc.ModelType = N'BoxFusion.TicketingSystem.Domain.Tickets.TicketComment',
        fc.GenerationLogicExtensionJson = REPLACE(
            COALESCE(fc.GenerationLogicExtensionJson, N''),
            N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketCommentInput',
            N'BoxFusion.TicketingSystem.Domain.Tickets.TicketComment'
        ),
        fc.Markup = REPLACE(
            REPLACE(
                COALESCE(fc.Markup, N''),
                N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketCommentInput',
                N'BoxFusion.TicketingSystem.Domain.Tickets.TicketComment'
            ),
            N'BoxFusion.TicketingSystem.Application.Tickets.UpdateTicketCommentInput',
            N'BoxFusion.TicketingSystem.Domain.Tickets.TicketComment'
        )
    FROM Frwk_FormConfigurations fc
    INNER JOIN Frwk_ConfigurationItems ci ON ci.Id = fc.Id
    WHERE ci.Name = N'Ticket-Comment';
END
");
        }

        public override void Down()
        {
        }
    }
}
