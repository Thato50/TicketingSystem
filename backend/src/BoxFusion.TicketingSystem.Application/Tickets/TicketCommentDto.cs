using Abp.Application.Services.Dto;
using System;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketCommentDto : EntityDto<Guid>
    {
        public Guid TicketId { get; set; }

        public Guid? AuthorId { get; set; }

        public string CommentText { get; set; } = string.Empty;

        public DateTime CreationTime { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
