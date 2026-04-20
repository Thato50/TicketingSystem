using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class UpdateTicketStatusInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public RefListTicketStatus Status { get; set; }
    }
}
