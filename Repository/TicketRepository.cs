using Contracts;
using Entities.Models;
using Entities.Utils;
using MongoDB.Driver;

namespace Repository
{
	public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
	{
		public TicketRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.TicketCollectionName) { }

		public async Task<IEnumerable<Ticket>> GetTicketsByUserId(string userID)
		{
			return await Collection.Find(x => x.SubmitterId == userID).ToListAsync();
		}
	}
}
