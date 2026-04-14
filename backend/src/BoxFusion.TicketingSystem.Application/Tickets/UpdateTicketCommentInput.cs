using System;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class UpdateTicketCommentInput
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? AuthorId { get; set; }

        [Required]
        [StringLength(2000)]
        public string CommentText { get; set; } = string.Empty;
    }
}
