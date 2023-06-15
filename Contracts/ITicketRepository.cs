using Entities.Models;

namespace Contracts
{
	public interface ITicketRepository : IRepositoryBase<Ticket>
	{
		Task<IEnumerable<Ticket>> GetAllActive();
		Task<IEnumerable<Ticket>> GetTicketsByUserId(string userID);
	}
}
