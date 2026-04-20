using Abp.Authorization;
using Abp.Localization;

namespace BoxFusion.TicketingSystem.Domain.Authorization
{
    public class TicketingSystemAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var tickets = context.CreatePermission(
                TicketingSystemPermissions.TicketsView,
                new FixedLocalizableString("View tickets"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsCreate,
                new FixedLocalizableString("Create tickets"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsEdit,
                new FixedLocalizableString("Edit tickets"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsDelete,
                new FixedLocalizableString("Delete tickets"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsUpdateStatus,
                new FixedLocalizableString("Update ticket status"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsAssign,
                new FixedLocalizableString("Assign tickets and manage requester"));

            tickets.CreateChildPermission(
                TicketingSystemPermissions.TicketsComment,
                new FixedLocalizableString("Manage ticket comments"));
        }
    }
}
