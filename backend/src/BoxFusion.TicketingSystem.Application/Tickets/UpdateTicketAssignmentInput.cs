using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class UpdateTicketAssignmentInput
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? AssignedToId { get; set; }
    }
}
