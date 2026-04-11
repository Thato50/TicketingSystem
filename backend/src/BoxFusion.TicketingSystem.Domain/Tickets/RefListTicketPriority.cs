using System.ComponentModel;
using Shesha.Domain.Attributes;

namespace BoxFusion.TicketSystem.Domain.Tickets
{
    [ReferenceList("TicketPriority")]
    public enum RefListTicketPriority : long
    {
        [Description("Low")]
        Low = 1,

        [Description("Medium")]
        Medium = 2,

        [Description("High")]
        High = 3
    }
}
