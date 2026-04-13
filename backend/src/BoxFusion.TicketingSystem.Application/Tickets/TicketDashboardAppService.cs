using Abp.Application.Services;
using Abp.Domain.Repositories;
using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketDashboardAppService : ApplicationService
    {
        private readonly IRepository<Ticket, Guid> _ticketRepository;

        public TicketDashboardAppService(IRepository<Ticket, Guid> ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketDashboardSummaryDto> GetSummary()
        {
            var tickets = await _ticketRepository.GetAllListAsync();

            return new TicketDashboardSummaryDto
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == RefListTicketStatus.Open),
                InProgressTickets = tickets.Count(t => t.Status == RefListTicketStatus.InProgress),
                ClosedTickets = tickets.Count(t =>
                    t.Status == RefListTicketStatus.Closed ||
                    t.Status == RefListTicketStatus.Resolved)
            };
        }
    }
}
