using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Ticket
{
	public class GetTicketStatusDto
	{
		[EnumDataType(typeof(TicketStatus))]
		public TicketStatus Status { get; set; }
	}
}
