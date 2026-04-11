using System.ComponentModel;
using Shesha.Domain.Attributes;

namespace BoxFusion.TicketSystem.Domain.Tickets
{
    [ReferenceList("TicketStatus")]
    public enum RefListTicketStatus : long
    {
        [Description("Open")]
        Open = 1,

        [Description("In Progress")]
        InProgress = 2,

        [Description("Resolved")]
        Resolved = 3,

        [Description("Closed")]
        Closed = 4
    }
}