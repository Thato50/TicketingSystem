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
    public class TicketAppService : ApplicationService
    {
        private readonly IRepository<Ticket, Guid> _ticketRepository;
        private readonly IRepository<Person, Guid> _personRepository;

        public TicketAppService(
            IRepository<Ticket, Guid> ticketRepository,
            IRepository<Person, Guid> personRepository)
        {
            _ticketRepository = ticketRepository;
            _personRepository = personRepository;
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsView)]
        public async Task<List<TicketDto>> GetAll()
        {
            var tickets = await _ticketRepository.GetAllListAsync();

            return tickets
                .OrderByDescending(t => t.CreationTime)
                .Select(MapToDto)
                .ToList();
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsView)]
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
                query = query.Where(t => t.Requester != null && t.Requester.Id == input.RequesterId.Value);

            if (input.AssignedToId.HasValue)
                query = query.Where(t => t.AssignedTo != null && t.AssignedTo.Id == input.AssignedToId.Value);

            var tickets = query
                .OrderByDescending(t => t.CreationTime)
                .ToList()
                .Select(MapToDto)
                .ToList();

            return Task.FromResult(tickets);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsView)]
        public async Task<TicketDto> GetById(EntityDto<Guid> input)
        {
            return await GetDetails(input);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsView)]
        public async Task<TicketDto> GetDetails(EntityDto<Guid> input)
        {
            var ticket = await GetTicketOrThrow(input.Id);
            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsEdit)]
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
                RequesterId = ticket.Requester?.Id,
                AssignedToId = ticket.AssignedTo?.Id
            };
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsCreate)]
        public async Task<TicketDto> Create(CreateTicketInput input)
        {
            var requester = await GetPersonIfProvided(input.RequesterId, "Requester not found.");
            var assignedTo = await GetPersonIfProvided(input.AssignedToId, "Assigned user not found.");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid()
            };

            ApplyTicketValues(ticket, input.Title, input.Description, input.Category, input.Priority, input.Status, requester, assignedTo);

            await _ticketRepository.InsertAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsEdit)]
        public async Task<TicketDto> Update(UpdateTicketInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);
            var requester = await GetPersonIfProvided(input.RequesterId, "Requester not found.");
            var assignedTo = await GetPersonIfProvided(input.AssignedToId, "Assigned user not found.");

            ValidateStatusTransition(ticket.Status, input.Status);
            ApplyTicketValues(ticket, input.Title, input.Description, input.Category, input.Priority, input.Status, requester, assignedTo);

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsAssign)]
        public async Task<TicketDto> Assign(UpdateTicketAssignmentInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            ticket.AssignedTo = input.AssignedToId.HasValue
                ? await GetPersonOrThrow(input.AssignedToId.Value, "Assigned user not found.")
                : null;

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsAssign)]
        public async Task<TicketDto> UpdateRequester(UpdateTicketRequesterInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            ticket.Requester = input.RequesterId.HasValue
                ? await GetPersonOrThrow(input.RequesterId.Value, "Requester not found.")
                : null;

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsUpdateStatus)]
        public async Task<TicketDto> UpdateStatus(UpdateTicketStatusInput input)
        {
            var ticket = await GetTicketOrThrow(input.Id);

            ValidateStatusTransition(ticket.Status, input.Status);
            ticket.Status = input.Status;

            await _ticketRepository.UpdateAsync(ticket);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToDto(ticket);
        }

        [AbpAuthorize(TicketingSystemPermissions.TicketsDelete)]
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

        private async Task<Person?> GetPersonIfProvided(Guid? id, string errorMessage)
        {
            return id.HasValue
                ? await GetPersonOrThrow(id.Value, errorMessage)
                : null;
        }

        private async Task<Person> GetPersonOrThrow(Guid id, string errorMessage)
        {
            var person = await _personRepository.FirstOrDefaultAsync(id);

            if (person == null)
                throw new UserFriendlyException(errorMessage);

            return person;
        }

        private static void ApplyTicketValues(
            Ticket ticket,
            string title,
            string description,
            RefListTicketCategory category,
            RefListTicketPriority priority,
            RefListTicketStatus status,
            Person? requester,
            Person? assignedTo)
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
            ticket.Requester = requester;
            ticket.AssignedTo = assignedTo;
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
                RequesterId = ticket.Requester?.Id,
                AssignedToId = ticket.AssignedTo?.Id,
                CreationTime = ticket.CreationTime,
                LastModificationTime = ticket.LastModificationTime
            };
        }
    }
}
