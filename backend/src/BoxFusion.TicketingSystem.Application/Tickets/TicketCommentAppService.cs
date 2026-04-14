using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using BoxFusion.TicketingSystem.Domain.Authorization;
using BoxFusion.TicketingSystem.Domain.Tickets;
using Shesha.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketCommentAppService : ApplicationService
    {
        private readonly IRepository<TicketComment, Guid> _ticketCommentRepository;
        private readonly IRepository<Ticket, Guid> _ticketRepository;
        private readonly IRepository<Person, Guid> _personRepository;

        public TicketCommentAppService(
            IRepository<TicketComment, Guid> ticketCommentRepository,
            IRepository<Ticket, Guid> ticketRepository,
            IRepository<Person, Guid> personRepository)
        {
            _ticketCommentRepository = ticketCommentRepository;
            _ticketRepository = ticketRepository;
            _personRepository = personRepository;
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsComment)]
        public async Task<List<TicketCommentDto>> GetByTicket(GetTicketCommentsInput input)
        {
            await GetTicketOrThrow(input.TicketId);

            var comments = _ticketCommentRepository.GetAll()
                .Where(c => c.Ticket.Id == input.TicketId)
                .OrderByDescending(c => c.CreationTime)
                .ToList()
                .Select(MapToDto)
                .ToList();

            return await Task.FromResult(comments);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsComment)]
        public async Task<TicketCommentDto> GetById(EntityDto<Guid> input)
        {
            var comment = await GetCommentOrThrow(input.Id);
            return MapToDto(comment);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsComment)]
        public async Task<TicketCommentDto> Create(CreateTicketCommentInput input)
        {
            var ticket = await GetTicketOrThrow(input.TicketId);
            var author = await GetAuthorIfProvided(input.AuthorId);

            var cleanCommentText = input.CommentText?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cleanCommentText))
                throw new UserFriendlyException("Comment text is required.");

            var comment = new TicketComment
            {
                Id = Guid.NewGuid(),
                Ticket = ticket,
                Author = author,
                CommentText = cleanCommentText
            };

            await _ticketCommentRepository.InsertAsync(comment);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(comment);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsComment)]
        public async Task<TicketCommentDto> Update(UpdateTicketCommentInput input)
        {
            var comment = await GetCommentOrThrow(input.Id);
            var author = await GetAuthorIfProvided(input.AuthorId);

            var cleanCommentText = input.CommentText?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cleanCommentText))
                throw new UserFriendlyException("Comment text is required.");

            comment.Author = author;
            comment.CommentText = cleanCommentText;

            await _ticketCommentRepository.UpdateAsync(comment);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(comment);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsComment)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await GetCommentOrThrow(input.Id);
            await _ticketCommentRepository.DeleteAsync(input.Id);
        }

        private async Task<Ticket> GetTicketOrThrow(Guid id)
        {
            var ticket = await _ticketRepository.FirstOrDefaultAsync(id);

            if (ticket == null)
                throw new UserFriendlyException("Ticket not found.");

            return ticket;
        }

        private async Task<TicketComment> GetCommentOrThrow(Guid id)
        {
            var comment = await _ticketCommentRepository.FirstOrDefaultAsync(id);

            if (comment == null)
                throw new UserFriendlyException("Ticket comment not found.");

            return comment;
        }

        private async Task<Person?> GetAuthorIfProvided(Guid? authorId)
        {
            if (!authorId.HasValue)
                return null;

            var author = await _personRepository.FirstOrDefaultAsync(authorId.Value);

            if (author == null)
                throw new UserFriendlyException("Comment author not found.");

            return author;
        }

        private static TicketCommentDto MapToDto(TicketComment comment)
        {
            return new TicketCommentDto
            {
                Id = comment.Id,
                TicketId = comment.Ticket.Id,
                AuthorId = comment.Author?.Id,
                CommentText = comment.CommentText,
                CreationTime = comment.CreationTime,
                LastModificationTime = comment.LastModificationTime
            };
        }
    }
}
