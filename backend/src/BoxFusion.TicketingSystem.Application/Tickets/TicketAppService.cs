using Abp.Application.Services;
using Abp.Domain.Repositories;
using BoxFusion.TicketingSystem.Domain.Tickets;
using System;
using System.Collections.Generic;
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

        public async Task<List<Ticket>> GetAll()
        {
            return await _ticketRepository.GetAllListAsync();
        }
    }
}