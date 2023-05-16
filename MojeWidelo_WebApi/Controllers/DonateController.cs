using AutoMapper;
using Contracts;
using Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using System.ComponentModel.DataAnnotations;

namespace MojeWidelo_WebApi.Controllers
{
	[ApiController]
	public class DonateController : BaseController
	{
		public DonateController(IRepositoryWrapper repository, IMapper mapper)
			: base(repository, mapper) { }

		[HttpPost("/donate/send", Name = "sendDonation")]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> SendDonation([Required] string id, [Required] decimal amount)
		{
			if (id == GetUserIdFromToken())
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Nie można wysłać dotacji samemu sobie.");
			}

			var user = await _repository.UsersRepository.GetById(id);

			if (user == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Użytkownik o podanym ID nie istnieje.");
			}

			if (user.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Użytkownik o podanym ID nie jest twórcą.");
			}

			if (amount <= 0)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Podana wartość musi być większa od zera.");
			}

			await _repository.UsersRepository.UpdateAccountBalance(id, amount);

			return StatusCode(StatusCodes.Status200OK, "Dotacja została wysłana pomyślnie.");
		}

		[HttpPost("/donate/withdraw", Name = "withdrawFunds")]
		public async Task<IActionResult> WithdrawFunds([Required] decimal amount)
		{
			var id = GetUserIdFromToken();

			var user = await _repository.UsersRepository.GetById(id);

			if (user.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Nie można wypłacić środków, nie jesteś twórcą.");
			}

			if (amount <= 0)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Podana wartość musi być większa od zera.");
			}

			if (user.AccountBalance < amount)
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					"Nie można wypłacić środków, niewystarczająca ilość środków na koncie."
				);
			}

			await _repository.UsersRepository.UpdateAccountBalance(id, -amount);

			return StatusCode(StatusCodes.Status200OK, "Środki zostały wypłacone pomyślnie.");
		}
	}
}
