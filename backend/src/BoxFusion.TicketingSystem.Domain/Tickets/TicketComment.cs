using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using Shesha.Domain;

namespace BoxFusion.TicketingSystem.Domain.Tickets
{
    public class TicketComment : FullAuditedEntity<Guid>
    {
        [Required]
        public virtual Ticket Ticket { get; set; } = null!;

        public virtual Person? Author { get; set; }

        [Required]
        [StringLength(2000)]
        public virtual string CommentText { get; set; } = string.Empty;
    }
}