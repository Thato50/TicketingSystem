using Abp.Application.Services.Dto;
using BoxFusion.TicketingSystem.Domain.Tickets;
using System;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketDto : EntityDto<Guid>
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public RefListTicketCategory Category { get; set; }

        public RefListTicketPriority Priority { get; set; }

        public RefListTicketStatus Status { get; set; }

        public Guid? RequesterId { get; set; }

        public Guid? AssignedToId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
