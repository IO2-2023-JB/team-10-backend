using AutoMapper;
using Contracts;
using Entities.Data.Ticket;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class TicketController : BaseController
	{
		public TicketController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		/// <summary>
		/// Ticket creation
		/// </summary>
		/// <param name="submitTicketDto"></param>
		/// <returns></returns>
		/// <response code="201">Created</response>
		/// <response code="400">Bad request</response>
		[HttpPost("ticket")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(SubmitTicketResponseDto))]
		public async Task<IActionResult> SubmitTicket([FromBody] SubmitTicketDto submitTicketDto)
		{
			var userID = GetUserIdFromToken();
			bool ownContent = false;
			TicketTargetTypeDto? type = null;

			var video = await _repository.VideoRepository.GetById(submitTicketDto.TargetId);
			if (video != null)
			{
				if (video.AuthorId == userID)
					ownContent = true;
				type = TicketTargetTypeDto.Video;
			}
			else
			{
				var user = await _repository.UsersRepository.GetById(submitTicketDto.TargetId);
				if (user != null)
				{
					if (user.Id == userID)
						ownContent = true;
					type = TicketTargetTypeDto.User;
				}
				else
				{
					var playlist = await _repository.PlaylistRepository.GetById(submitTicketDto.TargetId);
					if (playlist != null)
					{
						if (playlist.AuthorId == userID)
							ownContent = true;
						type = TicketTargetTypeDto.Playlist;
					}
					else
					{
						var comment = await _repository.CommentRepository.GetById(submitTicketDto.TargetId);
						if (comment != null)
						{
							if (comment.AuthorId == userID)
							{
								ownContent = true;
							}
							if (comment.OriginCommentId == null)
							{
								type = TicketTargetTypeDto.Comment;
							}
							else
							{
								type = TicketTargetTypeDto.CommentResponse;
							}
						}
					}
				}
			}

			if (ownContent)
				return StatusCode(StatusCodes.Status400BadRequest, "Nie można zgłaszać własnych treści!");
			if (type == null)
				return StatusCode(StatusCodes.Status400BadRequest, "Treść o podanym ID nie istnieje!");

			var ticket = _mapper.Map<Ticket>(submitTicketDto);
			ticket.SubmitterId = userID;
<<<<<<< HEAD
			ticket.TargetType = type.Value;
=======
			ticket.TargetType = (TicketTargetTypeDto)type;
>>>>>>> 0d3915f (Fix model so that it fits documentation)
			ticket.Status = TicketStatus.Submitted;
			ticket.CreationDate = DateTime.Now;

			ticket = await _repository.TicketRepository.Create(ticket);
			var result = _mapper.Map<SubmitTicketResponseDto>(ticket);

			return StatusCode(StatusCodes.Status201Created, result);
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
			ticket.AdminId = user.Id;
			ticket.Status = TicketStatus.Resolved;
			ticket = await _repository.TicketRepository.Update(id, ticket);

			var result = _mapper.Map<SubmitTicketResponseDto>(ticket);
			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Ticket retreival
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">Created</response>
		/// <response code="400">Bad request</response>
		[HttpGet("ticket")]
		[ServiceFilter(typeof(ModelValidationFilter))]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(GetTicketDto))]
		public async Task<IActionResult> GetTicket([Required] string id)
		{
			var ticket = await _repository.TicketRepository.GetById(id);

			if (ticket == null)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Ticket o podanym ID nie istnieje.");
			}

			var user = await GetUserFromToken();
			if (user.UserType != UserType.Administrator || ticket.SubmitterId != user.Id)
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Brak uprawnień do dostępu do ticketu o podanym ID!"
				);
			}

			var result = _mapper.Map<GetTicketDto>(ticket);
			result.TicketId = id;
			return StatusCode(StatusCodes.Status200OK, result);
		}
	}
}
