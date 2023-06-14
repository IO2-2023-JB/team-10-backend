using Entities.Data.Ticket;
using Entities.Models;

namespace MojeWidelo_WebApi.Mapper
{
	public class TicketProfile : AutoMapper.Profile
	{
		public TicketProfile()
		{
			CreateMap<SubmitTicketDto, Ticket>();
			CreateMap<Ticket, SubmitTicketResponseDto>();
			CreateMap<RespondToTicketDto, Ticket>();
			CreateMap<Ticket, GetTicketDto>();
		}
	}
}
