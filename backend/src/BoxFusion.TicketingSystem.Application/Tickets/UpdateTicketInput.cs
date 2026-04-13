using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class UpdateTicketInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public RefListTicketCategory Category { get; set; } = RefListTicketCategory.General;

        public RefListTicketPriority Priority { get; set; } = RefListTicketPriority.Medium;

        public RefListTicketStatus Status { get; set; } = RefListTicketStatus.Open;

        public Guid? RequesterId { get; set; }

        public Guid? AssignedToId { get; set; }
    }
}
