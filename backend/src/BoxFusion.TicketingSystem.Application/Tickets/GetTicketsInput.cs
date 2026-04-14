using BoxFusion.TicketingSystem.Domain.Tickets;
using System;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class GetTicketsInput
    {
        public string? SearchText { get; set; }

        public RefListTicketCategory? Category { get; set; }

        public RefListTicketPriority? Priority { get; set; }

        public RefListTicketStatus? Status { get; set; }

        public Guid? RequesterId { get; set; }

        public Guid? AssignedToId { get; set; }
    }
}
