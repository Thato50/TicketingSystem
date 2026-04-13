using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.UI;
using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketAppService : ApplicationService
    {
        private readonly IRepository<Ticket, Guid> _ticketRepository;

        public TicketAppService(IRepository<Ticket, Guid> ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<TicketDto>> GetAll()
        {
            var tickets = await _ticketRepository.GetAllListAsync();

            return tickets
                .OrderByDescending(t => t.CreationTime)
                .Select(MapToDto)
                .ToList();
        }

        public Task<List<TicketDto>> GetList(GetTicketsInput input)
        {
            var query = _ticketRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(input.SearchText))
            {
                var searchText = input.SearchText.Trim().ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchText) ||
                    t.Description.ToLower().Contains(searchText));
            }

            if (input.Category.HasValue)
                query = query.Where(t => t.Category == input.Category.Value);

            if (input.Priority.HasValue)
                query = query.Where(t => t.Priority == input.Priority.Value);

            if (input.Status.HasValue)
                query = query.Where(t => t.Status == input.Status.Value);

            if (input.RequesterId.HasValue)
                query = query.Where(t => t.RequesterId == input.RequesterId.Value);

            if (input.AssignedToId.HasValue)
                query = query.Where(t => t.AssignedToId == input.AssignedToId.Value);

            var tickets = query
                .OrderByDescending(t => t.CreationTime)
                .ToList()
                .Select(MapToDto)
                .ToList();

            return Task.FromResult(tickets);
        }

        public async Task<TicketDto> GetById(EntityDto<Guid> input)
        {
            return await GetDetails(input);
        }

        public async Task<TicketDto> GetDetails(EntityDto<Guid> input)
        {
            var ticket = await GetTicketOrThrow(input.Id);
            return MapToDto(ticket);
        }

        public async Task<UpdateTicketInput> GetForEdit(EntityDto<Guid> input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            return new UpdateTicketInput
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                RequesterId = ticket.RequesterId,
                AssignedToId = ticket.AssignedToId
            };
        }

        public async Task<TicketDto> Create(CreateTicketInput input)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid()
            };

            ApplyTicketValues(ticket, input.Title, input.Description, input.Category, input.Priority, input.Status, input.RequesterId, input.AssignedToId);

            await _ticketRepository.InsertAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        public async Task<TicketDto> Update(UpdateTicketInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            ValidateStatusTransition(ticket.Status, input.Status);
            ApplyTicketValues(ticket, input.Title, input.Description, input.Category, input.Priority, input.Status, input.RequesterId, input.AssignedToId);

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        public async Task<TicketDto> UpdateStatus(UpdateTicketStatusInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            ValidateStatusTransition(ticket.Status, input.Status);
            ticket.Status = input.Status;

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        public async Task Delete(EntityDto<Guid> input)
        {
            await GetTicketOrThrow(input.Id);
            await _ticketRepository.DeleteAsync(input.Id);
        }

        private async Task<Ticket> GetTicketOrThrow(Guid id)
        {
            var ticket = await _ticketRepository.FirstOrDefaultAsync(id);

            if (ticket == null)
                throw new UserFriendlyException("Ticket not found.");

            return ticket;
        }

        private static void ApplyTicketValues(
            Ticket ticket,
            string title,
            string description,
            RefListTicketCategory category,
            RefListTicketPriority priority,
            RefListTicketStatus status,
            Guid? requesterId,
            Guid? assignedToId)
        {
            var cleanTitle = title?.Trim() ?? string.Empty;
            var cleanDescription = description?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanTitle))
                throw new UserFriendlyException("Title is required.");

            if (string.IsNullOrWhiteSpace(cleanDescription))
                throw new UserFriendlyException("Description is required.");

            ticket.Title = cleanTitle;
            ticket.Description = cleanDescription;
            ticket.Category = category;
            ticket.Priority = priority;
            ticket.Status = status;
            ticket.RequesterId = requesterId;
            ticket.AssignedToId = assignedToId;
        }

        private static void ValidateStatusTransition(RefListTicketStatus currentStatus, RefListTicketStatus newStatus)
        {
            if (currentStatus == newStatus)
                return;

            var isAllowed = currentStatus switch
            {
                RefListTicketStatus.Open => newStatus == RefListTicketStatus.InProgress ||
                                            newStatus == RefListTicketStatus.Resolved ||
                                            newStatus == RefListTicketStatus.Closed,
                RefListTicketStatus.InProgress => newStatus == RefListTicketStatus.Open ||
                                                  newStatus == RefListTicketStatus.Resolved ||
                                                  newStatus == RefListTicketStatus.Closed,
                RefListTicketStatus.Resolved => newStatus == RefListTicketStatus.InProgress ||
                                                newStatus == RefListTicketStatus.Open ||
                                                newStatus == RefListTicketStatus.Closed,
                RefListTicketStatus.Closed => newStatus == RefListTicketStatus.Open,
                _ => false
            };

            if (!isAllowed)
                throw new UserFriendlyException($"Cannot change ticket status from {currentStatus} to {newStatus}.");
        }

        private static TicketDto MapToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                RequesterId = ticket.RequesterId,
                AssignedToId = ticket.AssignedToId,
                CreationTime = ticket.CreationTime,
                LastModificationTime = ticket.LastModificationTime
            };
        }
    }
}
