using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class UpdateTicketRequesterInput
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? RequesterId { get; set; }
    }
}
