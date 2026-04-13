using Abp.Application.Services.Dto;
using BoxFusion.TicketingSystem.Application.Tickets;
using BoxFusion.TicketingSystem.Domain.Tickets;
using Shouldly;
using Shesha.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BoxFusion.TicketingSystem.Common.Tests.Tickets
{
    public class TicketCommentAppServiceTests
    {
        [Fact]
        public async Task Create_Should_Add_Comment_For_Ticket()
        {
            var ticketId = Guid.NewGuid();
            var authorId = Guid.NewGuid();
            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = ticketId,
                    Title = "Printer issue",
                    Description = "Printer won't print"
                }
            };
            var comments = new List<TicketComment>();
            var people = new List<Person> { new Person { Id = authorId } };

            var service = CreateService(comments, tickets, people);

            var result = await service.Create(new CreateTicketCommentInput
            {
                TicketId = ticketId,
                AuthorId = authorId,
                CommentText = "Investigating the issue"
            });

            result.TicketId.ShouldBe(ticketId);
            result.AuthorId.ShouldBe(authorId);
            comments.Count.ShouldBe(1);
            comments[0].CommentText.ShouldBe("Investigating the issue");
        }

        [Fact]
        public async Task GetByTicket_Should_Return_Only_Comments_For_Requested_Ticket()
        {
            var firstTicketId = Guid.NewGuid();
            var secondTicketId = Guid.NewGuid();

            var firstTicket = new Ticket { Id = firstTicketId, Title = "A", Description = "A" };
            var secondTicket = new Ticket { Id = secondTicketId, Title = "B", Description = "B" };

            var comments = new List<TicketComment>
            {
                new TicketComment
                {
                    Id = Guid.NewGuid(),
                    Ticket = firstTicket,
                    CommentText = "Comment 1"
                },
                new TicketComment
                {
                    Id = Guid.NewGuid(),
                    Ticket = secondTicket,
                    CommentText = "Comment 2"
                }
            };

            var service = CreateService(comments, new List<Ticket> { firstTicket, secondTicket });

            var result = await service.GetByTicket(new GetTicketCommentsInput
            {
                TicketId = firstTicketId
            });

            result.Count.ShouldBe(1);
            result[0].CommentText.ShouldBe("Comment 1");
        }

        [Fact]
        public async Task Update_And_Delete_Should_Modify_And_Remove_Comment()
        {
            var ticket = new Ticket { Id = Guid.NewGuid(), Title = "A", Description = "A" };
            var comment = new TicketComment
            {
                Id = Guid.NewGuid(),
                Ticket = ticket,
                CommentText = "Original"
            };

            var comments = new List<TicketComment> { comment };
            var service = CreateService(comments, new List<Ticket> { ticket });

            var updated = await service.Update(new UpdateTicketCommentInput
            {
                Id = comment.Id,
                CommentText = "Updated"
            });

            updated.CommentText.ShouldBe("Updated");
            comments[0].CommentText.ShouldBe("Updated");

            await service.Delete(new EntityDto<Guid>(comment.Id));

            comments.Count.ShouldBe(0);
        }

        private static TicketCommentAppService CreateService(
            List<TicketComment> comments,
            List<Ticket> tickets,
            List<Person> people = null)
        {
            var commentRepository = ApplicationServiceTestHelpers.CreateRepository(comments);
            var ticketRepository = ApplicationServiceTestHelpers.CreateRepository(tickets);
            var personRepository = ApplicationServiceTestHelpers.CreateRepository(people ?? new List<Person>());

            var service = new TicketCommentAppService(
                commentRepository.Object,
                ticketRepository.Object,
                personRepository.Object)
            {
                UnitOfWorkManager = ApplicationServiceTestHelpers.CreateUnitOfWorkManager()
            };

            return service;
        }
    }
}
