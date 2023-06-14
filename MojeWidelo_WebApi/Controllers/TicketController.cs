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
			ticket.CreationDate = DateTime.Now;

			ticket = await _repository.TicketRepository.Create(ticket);
			var result = _mapper.Map<SubmitTicketResponseDto>(ticket);

			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Ticket responding
		/// </summary>
		/// <param name="id"></param>
		/// <param name="respondToTicketDto"></param>
		/// <returns></returns>
		/// <response code="200">Created</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorized</response>
		[HttpPut("ticket")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(SubmitTicketResponseDto))]
		public async Task<IActionResult> RespondToTicket(
			[Required] string id,
			[FromBody] RespondToTicketDto respondToTicketDto
		)
		{
			var ticket = await _repository.TicketRepository.GetById(id);

			if (ticket == null)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Ticket o podanym ID nie istnieje.");
			}

			if (ticket.AdminId != null || ticket.Response != null)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Ticket o podanym ID został już zaadresowany.");
			}

			var user = await GetUserFromToken();
			if (user.UserType != UserType.Administrator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Użytkownik nie jest administratorem.");
			}

			ticket = _mapper.Map<RespondToTicketDto, Ticket>(respondToTicketDto, ticket);
			ticket.ResponseDate = DateTime.Now;
			ticket = await _repository.TicketRepository.Update(id, ticket);

			var result = _mapper.Map<SubmitTicketResponseDto>(ticket);
			return StatusCode(StatusCodes.Status200OK, result);
		}
	}
}
