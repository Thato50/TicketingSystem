using Abp.Application.Services;
using Abp.Domain.Repositories;
using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
                    t.Status == RefListTicketStatus.Resolved),
                RecentTickets = tickets
                    .OrderByDescending(t => t.CreationTime)
                    .Take(5)
                    .Select(t => new TicketDashboardRecentTicketDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Priority = t.Priority,
                        Status = t.Status,
                        CreationTime = t.CreationTime
                    })
                    .ToList(),
                TicketsByStatus = Enum.GetValues(typeof(RefListTicketStatus))
                    .Cast<RefListTicketStatus>()
                    .Select(status => new TicketDashboardGroupCountDto
                    {
                        Value = (long)status,
                        Label = GetEnumLabel(status),
                        Count = tickets.Count(t => t.Status == status)
                    })
                    .ToList(),
                TicketsByPriority = Enum.GetValues(typeof(RefListTicketPriority))
                    .Cast<RefListTicketPriority>()
                    .Select(priority => new TicketDashboardGroupCountDto
                    {
                        Value = (long)priority,
                        Label = GetEnumLabel(priority),
                        Count = tickets.Count(t => t.Priority == priority)
                    })
                    .ToList()
            };
        }

        private static string GetEnumLabel<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            var description = member?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return string.IsNullOrWhiteSpace(description) ? value.ToString() : description;
        }
    }
}
