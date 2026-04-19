using FluentMigrator;

namespace BoxFusion.TicketingSystem.Domain.Migrations
{
    [Migration(20260418093000)]
    public class M20260418093000 : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
IF OBJECT_ID(N'dbo.Frwk_FormConfigurations', N'U') IS NOT NULL
AND OBJECT_ID(N'dbo.Frwk_ConfigurationItems', N'U') IS NOT NULL
BEGIN
    UPDATE fc
    SET
        fc.ModelType = N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketInput',
        fc.GenerationLogicExtensionJson = REPLACE(
            COALESCE(fc.GenerationLogicExtensionJson, N''),
            N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket',
            N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketInput'
        ),
        fc.Markup = REPLACE(
            COALESCE(fc.Markup, N''),
            N'BoxFusion.TicketingSystem.Domain.Tickets.Ticket',
            N'BoxFusion.TicketingSystem.Application.Tickets.CreateTicketInput'
        )
    FROM Frwk_FormConfigurations fc
    INNER JOIN Frwk_ConfigurationItems ci ON ci.Id = fc.Id
    WHERE ci.Name = N'New-Ticket';

    UPDATE Frwk_ConfigurationItems
    SET IsLast = CASE
        WHEN Name = N'Ticket-Details'
         AND OriginId = 'DDECD228-C947-47FE-86E0-4A0B7C63D382'
         AND VersionNo = 4 THEN 1
        WHEN Name = N'TicketList'
         AND OriginId = 'E53012B8-4E26-4C8A-9CFF-DFE5DDE7FABB'
         AND VersionNo = 7 THEN 1
        ELSE 0
    END
    WHERE Name IN (N'Ticket-Details', N'TicketList');
END
");
        }

        public override void Down()
        {
        }
    }
}
