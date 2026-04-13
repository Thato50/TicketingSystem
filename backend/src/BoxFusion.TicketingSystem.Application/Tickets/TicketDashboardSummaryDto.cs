namespace BoxFusion.TicketingSystem.Application.Tickets
{
    public class TicketDashboardSummaryDto
    {
        public int TotalTickets { get; set; }

        public int OpenTickets { get; set; }

        public int InProgressTickets { get; set; }

        // Treat resolved tickets as closed for the current dashboard card layout.
        public int ClosedTickets { get; set; }
    }
}
