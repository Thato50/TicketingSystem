using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class GetTicketCommentsInput
    {
        [Required]
        public Guid TicketId { get; set; }
    }
}
