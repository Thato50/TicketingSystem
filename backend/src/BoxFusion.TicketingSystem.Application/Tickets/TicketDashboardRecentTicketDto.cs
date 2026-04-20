using BoxFusion.TicketingSystem.Domain.Tickets;
using System;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketDashboardRecentTicketDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public RefListTicketPriority Priority { get; set; }

        public RefListTicketStatus Status { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
