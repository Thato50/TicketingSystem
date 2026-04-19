using Abp.Domain.Entities.Auditing;
using Shesha.Domain;
using Shesha.Domain.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoxFusion.TicketingSystem.Domain.Tickets
{
    public class Ticket : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(200)]
        [EntityDisplayName]
        public virtual string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public virtual string Description { get; set; } = string.Empty;

        public virtual RefListTicketCategory Category { get; set; } = RefListTicketCategory.General;
        public virtual RefListTicketPriority Priority { get; set; } = RefListTicketPriority.Medium;
        public virtual RefListTicketStatus Status { get; set; } = RefListTicketStatus.Open;

        public virtual Person? Requester { get; set; }
        public virtual Person? AssignedTo { get; set; }

        public virtual IList<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}
