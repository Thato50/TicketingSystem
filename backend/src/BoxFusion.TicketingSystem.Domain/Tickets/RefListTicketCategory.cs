using System.ComponentModel;
using Shesha.Domain.Attributes;

namespace BoxFusion.TicketSystem.Domain.Tickets
{
    [ReferenceList("TicketCategory")]
    public enum RefListTicketCategory : long
    {
        [Description("General")]
        General = 1,

        [Description("IT")]
        IT = 2,

        [Description("HR")]
        HR = 3,

        [Description("Finance")]
        Finance = 4
    }
}