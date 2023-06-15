using Contracts;
using Entities.Models;
using Entities.Utils;

namespace Repository
{
	public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
	{
		public TicketRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.TicketCollectionName) { }
	}
}
