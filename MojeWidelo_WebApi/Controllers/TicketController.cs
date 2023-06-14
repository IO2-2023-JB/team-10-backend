using AutoMapper;
using Contracts;
using Entities.Data.Ticket;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text.Json;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class TicketController : BaseController
	{
		public TicketController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		/// <summary>
		/// Ticket creatrion
		/// </summary>
		/// <param name="submitTicketDto"></param>
		/// <returns></returns>
		/// <response code="200">Created</response>
		/// <response code="400">Bad request</response>
		[HttpPost("ticket")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(SubmitTicketResponseDto))]
		public async Task<IActionResult> SubmitTicket([FromBody] SubmitTicketDto submitTicketDto)
		{
			var userID = GetUserIdFromToken();
			var ticket = _mapper.Map<Ticket>(submitTicketDto);
			ticket.AuthorId = userID;
			ticket.Status = TicketStatus.Submitted;

			ticket = await _repository.TicketRepository.Create(ticket);
			var result = _mapper.Map<SubmitTicketResponseDto>(ticket);

			return StatusCode(StatusCodes.Status200OK, result);
		}
	}
}
