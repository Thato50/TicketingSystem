using FluentMigrator;

namespace BoxFusion.TicketingSystem.Domain.Migrations
{
    [Migration(20260419093000)]
    public class M20260419093000 : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
IF OBJECT_ID(N'dbo.Frwk_EntityConfigs', N'U') IS NOT NULL
AND OBJECT_ID(N'dbo.Frwk_EntityProperties', N'U') IS NOT NULL
BEGIN
    DECLARE @TicketEntityConfigId UNIQUEIDENTIFIER;

    SELECT TOP 1 @TicketEntityConfigId = ec.Id
    FROM Frwk_EntityConfigs ec
    WHERE ec.ClassName = N'Ticket'
      AND ec.Namespace = N'BoxFusion.TicketingSystem.Domain.Tickets';

    IF @TicketEntityConfigId IS NOT NULL
    BEGIN
        UPDATE Frwk_EntityProperties
        SET
            Name = N'Requester',
            Label = N'Requester',
            DataType = N'entity',
            EntityType = N'Shesha.Domain.Person',
            ReferenceListName = NULL
        WHERE EntityConfigId = @TicketEntityConfigId
          AND Name = N'RequesterId';

        UPDATE Frwk_EntityProperties
        SET
            Name = N'AssignedTo',
            Label = N'Assigned To',
            DataType = N'entity',
            EntityType = N'Shesha.Domain.Person',
            ReferenceListName = NULL
        WHERE EntityConfigId = @TicketEntityConfigId
          AND Name = N'AssignedToId';
    END
END
");
        }

        public override void Down()
        {
            Execute.Sql(@"
IF OBJECT_ID(N'dbo.Frwk_EntityConfigs', N'U') IS NOT NULL
AND OBJECT_ID(N'dbo.Frwk_EntityProperties', N'U') IS NOT NULL
BEGIN
    DECLARE @TicketEntityConfigId UNIQUEIDENTIFIER;

    SELECT TOP 1 @TicketEntityConfigId = ec.Id
    FROM Frwk_EntityConfigs ec
    WHERE ec.ClassName = N'Ticket'
      AND ec.Namespace = N'BoxFusion.TicketingSystem.Domain.Tickets';

    IF @TicketEntityConfigId IS NOT NULL
    BEGIN
        UPDATE Frwk_EntityProperties
        SET
            Name = N'RequesterId',
            Label = N'Requester Id',
            DataType = N'guid',
            EntityType = NULL
        WHERE EntityConfigId = @TicketEntityConfigId
          AND Name = N'Requester';

        UPDATE Frwk_EntityProperties
        SET
            Name = N'AssignedToId',
            Label = N'Assigned To Id',
            DataType = N'guid',
            EntityType = NULL
        WHERE EntityConfigId = @TicketEntityConfigId
          AND Name = N'AssignedTo';
    END
END
");
        }
    }
}
