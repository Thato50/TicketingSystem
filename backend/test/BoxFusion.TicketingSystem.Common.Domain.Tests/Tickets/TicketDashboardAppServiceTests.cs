using BoxFusion.TicketingSystem.Application.Tickets;
using BoxFusion.TicketingSystem.Domain.Tickets;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BoxFusion.TicketingSystem.Common.Tests.Tickets
{
    public class TicketDashboardAppServiceTests
    {
        [Fact]
        public async Task GetSummary_Should_Return_Counts_RecentTickets_And_Groups()
        {
            var now = DateTime.UtcNow;
            var tickets = new List<Ticket>
            {
                CreateTicket("Newest", RefListTicketStatus.Open, RefListTicketPriority.High, now),
                CreateTicket("In progress", RefListTicketStatus.InProgress, RefListTicketPriority.Medium, now.AddMinutes(-5)),
                CreateTicket("Resolved", RefListTicketStatus.Resolved, RefListTicketPriority.Low, now.AddMinutes(-10)),
                CreateTicket("Closed", RefListTicketStatus.Closed, RefListTicketPriority.High, now.AddMinutes(-15)),
                CreateTicket("Oldest", RefListTicketStatus.Open, RefListTicketPriority.Low, now.AddMinutes(-20)),
                CreateTicket("Outside top five", RefListTicketStatus.Open, RefListTicketPriority.Low, now.AddMinutes(-25))
            };

            var repository = ApplicationServiceTestHelpers.CreateRepository(tickets);
            var service = new TicketDashboardAppService(repository.Object);

            var summary = await service.GetSummary();

            summary.TotalTickets.ShouldBe(6);
            summary.OpenTickets.ShouldBe(3);
            summary.InProgressTickets.ShouldBe(1);
            summary.ClosedTickets.ShouldBe(2);

            summary.RecentTickets.Count.ShouldBe(5);
            summary.RecentTickets.First().Title.ShouldBe("Newest");
            summary.RecentTickets.Last().Title.ShouldBe("Oldest");

            summary.TicketsByStatus.Single(x => x.Value == (long)RefListTicketStatus.Open).Count.ShouldBe(3);
            summary.TicketsByStatus.Single(x => x.Value == (long)RefListTicketStatus.InProgress).Count.ShouldBe(1);
            summary.TicketsByStatus.Single(x => x.Value == (long)RefListTicketStatus.Resolved).Count.ShouldBe(1);
            summary.TicketsByStatus.Single(x => x.Value == (long)RefListTicketStatus.Closed).Count.ShouldBe(1);

            summary.TicketsByPriority.Single(x => x.Value == (long)RefListTicketPriority.Low).Count.ShouldBe(3);
            summary.TicketsByPriority.Single(x => x.Value == (long)RefListTicketPriority.Medium).Count.ShouldBe(1);
            summary.TicketsByPriority.Single(x => x.Value == (long)RefListTicketPriority.High).Count.ShouldBe(2);
        }

        private static Ticket CreateTicket(
            string title,
            RefListTicketStatus status,
            RefListTicketPriority priority,
            DateTime creationTime)
        {
            return new Ticket
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = title,
                Status = status,
                Priority = priority,
                CreationTime = creationTime
            };
        }
    }
}
