using Abp.Application.Services.Dto;
using Abp.UI;
using BoxFusion.TicketingSystem.Application.Tickets;
using BoxFusion.TicketingSystem.Domain.Tickets;
using Moq;
using Shouldly;
using Shesha.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BoxFusion.TicketingSystem.Common.Tests.Tickets
{
    public class TicketAppServiceTests
    {
        [Fact]
        public async Task Create_Should_Add_Ticket_And_Return_Dto()
        {
            var requesterId = Guid.NewGuid();
            var assignedToId = Guid.NewGuid();
            var tickets = new List<Ticket>();
            var people = new List<Person>
            {
                new Person { Id = requesterId },
                new Person { Id = assignedToId }
            };

            var service = CreateService(tickets, people);

            var result = await service.Create(new CreateTicketInput
            {
                Title = "Printer issue",
                Description = "Printer is not working",
                Category = RefListTicketCategory.IT,
                Priority = RefListTicketPriority.High,
                Status = RefListTicketStatus.Open,
                RequesterId = requesterId,
                AssignedToId = assignedToId
            });

            result.Title.ShouldBe("Printer issue");
            result.RequesterId.ShouldBe(requesterId);
            result.AssignedToId.ShouldBe(assignedToId);
            tickets.Count.ShouldBe(1);
            tickets.Single().Title.ShouldBe("Printer issue");
        }

        [Fact]
        public async Task GetDetails_And_GetForEdit_Should_Return_Existing_Ticket()
        {
            var ticketId = Guid.NewGuid();
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = ticketId,
                    Title = "Network issue",
                    Description = "Internet is down",
                    Category = RefListTicketCategory.IT,
                    Priority = RefListTicketPriority.Medium,
                    Status = RefListTicketStatus.Open
                }
            };

            var service = CreateService(tickets);

            var details = await service.GetDetails(new EntityDto<Guid>(ticketId));
            var forEdit = await service.GetForEdit(new EntityDto<Guid>(ticketId));

            details.Id.ShouldBe(ticketId);
            details.Title.ShouldBe("Network issue");
            forEdit.Id.ShouldBe(ticketId);
            forEdit.Description.ShouldBe("Internet is down");
        }

        [Fact]
        public async Task Update_Should_Modify_Ticket_Fields()
        {
            var ticketId = Guid.NewGuid();
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = ticketId,
                    Title = "Old title",
                    Description = "Old description",
                    Category = RefListTicketCategory.General,
                    Priority = RefListTicketPriority.Low,
                    Status = RefListTicketStatus.Open
                }
            };

            var service = CreateService(tickets);

            var result = await service.Update(new UpdateTicketInput
            {
                Id = ticketId,
                Title = "Updated title",
                Description = "Updated description",
                Category = RefListTicketCategory.Finance,
                Priority = RefListTicketPriority.High,
                Status = RefListTicketStatus.InProgress
            });

            result.Title.ShouldBe("Updated title");
            result.Priority.ShouldBe(RefListTicketPriority.High);
            tickets.Single().Status.ShouldBe(RefListTicketStatus.InProgress);
        }

        [Fact]
        public async Task GetList_Should_Filter_By_Search_And_Status()
        {
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Printer problem",
                    Description = "Office printer jam",
                    Status = RefListTicketStatus.Open
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Payroll query",
                    Description = "Finance needs help",
                    Status = RefListTicketStatus.Resolved
                }
            };

            var service = CreateService(tickets);

            var result = await service.GetList(new GetTicketsInput
            {
                SearchText = "printer",
                Status = RefListTicketStatus.Open
            });

            result.Count.ShouldBe(1);
            result.Single().Title.ShouldBe("Printer problem");
        }

        [Fact]
        public async Task UpdateStatus_Should_Allow_Valid_Transition()
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Laptop issue",
                Description = "Laptop not booting",
                Status = RefListTicketStatus.Open
            };

            var service = CreateService(new List<Ticket> { ticket });

            var result = await service.UpdateStatus(new UpdateTicketStatusInput
            {
                Id = ticket.Id,
                Status = RefListTicketStatus.InProgress
            });

            result.Status.ShouldBe(RefListTicketStatus.InProgress);
            ticket.Status.ShouldBe(RefListTicketStatus.InProgress);
        }

        [Fact]
        public async Task UpdateStatus_Should_Throw_On_Invalid_Transition()
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Closed issue",
                Description = "Already closed",
                Status = RefListTicketStatus.Closed
            };

            var service = CreateService(new List<Ticket> { ticket });

            var exception = await Should.ThrowAsync<UserFriendlyException>(() =>
                service.UpdateStatus(new UpdateTicketStatusInput
                {
                    Id = ticket.Id,
                    Status = RefListTicketStatus.Resolved
                }));

            exception.Message.ShouldContain("Cannot change ticket status");
        }

        [Fact]
        public async Task Assign_And_UpdateRequester_Should_Update_Person_Fields()
        {
            var requesterId = Guid.NewGuid();
            var assignedToId = Guid.NewGuid();
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Access issue",
                Description = "User cannot access portal",
                Status = RefListTicketStatus.Open
            };

            var people = new List<Person>
            {
                new Person { Id = requesterId },
                new Person { Id = assignedToId }
            };

            var service = CreateService(new List<Ticket> { ticket }, people);

            var assigned = await service.Assign(new UpdateTicketAssignmentInput
            {
                Id = ticket.Id,
                AssignedToId = assignedToId
            });

            var requester = await service.UpdateRequester(new UpdateTicketRequesterInput
            {
                Id = ticket.Id,
                RequesterId = requesterId
            });

            assigned.AssignedToId.ShouldBe(assignedToId);
            requester.RequesterId.ShouldBe(requesterId);
            ticket.AssignedToId.ShouldBe(assignedToId);
            ticket.RequesterId.ShouldBe(requesterId);
        }

        private static TicketAppService CreateService(List<Ticket> tickets, List<Person> people = null)
        {
            var ticketRepository = ApplicationServiceTestHelpers.CreateRepository(tickets);
            var personRepository = ApplicationServiceTestHelpers.CreateRepository(people ?? new List<Person>());

            var service = new TicketAppService(ticketRepository.Object, personRepository.Object)
            {
                UnitOfWorkManager = ApplicationServiceTestHelpers.CreateUnitOfWorkManager()
            };

            return service;
        }
    }
}
