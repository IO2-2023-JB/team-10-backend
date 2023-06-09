﻿using AutoMapper;
using Contracts;
using Entities.Data.Subscription;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using MojeWidelo_WebApi.Filters;
using Repository.Managers;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Controllers
{
	[Route("subscriptions")]
	[ApiController]
	public class SubscriptionsController : BaseController
	{
		private readonly SubscriptionsManager _subscriptionsManager;

		public SubscriptionsController(
			IRepositoryWrapper repository,
			IMapper mapper,
			SubscriptionsManager subscriptionsManager
		)
			: base(repository, mapper)
		{
			_subscriptionsManager = subscriptionsManager;
		}

		/// <summary>
		/// Subscribe to another user
		/// </summary>
		/// <param name="creatorId"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">NotFound</response>
		[HttpPost]
		[ServiceFilter(typeof(RenameParameterFilter))]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> Subscribe([Required] string creatorId)
		{
			var creator = await _repository.UsersRepository.GetById(creatorId);

			#region Validate operation
			if (creator == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Podany użytkownik nie istnieje.");
			}

			if (creator.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, $"Użytkownik {creator.Nickname} nie jest twórcą.");
			}

			var subscriberId = GetUserIdFromToken();

			if (subscriberId == creatorId)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Nie można zasubskrybować swojego konta.");
			}
			#endregion

			var sub = await _repository.SubscriptionsRepository.GetSubscription(creatorId, subscriberId);
			if (sub != null)
			{
				return StatusCode(
					StatusCodes.Status400BadRequest,
					$"Twórca {creator.Nickname} jest już zasubskrybowany."
				);
			}

			var subscription = new Subscription() { CreatorId = creatorId, SubscriberId = subscriberId };

			await _repository.SubscriptionsRepository.Create(subscription);
			await _repository.UsersRepository.UpdateSubscriptionCount(creatorId, 1);

			return StatusCode(StatusCodes.Status200OK, $"Pomyślnie zasubskrybowano użytkownika {creator.Nickname}.");
		}

		/// <summary>
		/// Get user's subscriptions
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">NotFound</response>
		[HttpGet]
		[Produces(MediaTypeNames.Application.Json, Type = typeof(SubscriptionListDto))]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> GetSubscriptions([Required] string id)
		{
			var user = await _repository.UsersRepository.GetById(id);

			#region Validate operation
			if (user == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Podany użytkownik nie istnieje.");
			}
			#endregion

			var subscriptions = await _repository.SubscriptionsRepository.GetUserSubscriptions(id);
			var subscribersIds = _subscriptionsManager.GetSubscribedUsersIds(subscriptions);
			var subscribers = await _repository.UsersRepository.GetUsersByIds(subscribersIds);

			var subscriptionsDto = _mapper.Map<IEnumerable<SubscriptionDto>>(subscribers);
			var result = new SubscriptionListDto(subscriptionsDto);
			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Unsubscribe from creator
		/// </summary>
		/// <param name="subId"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">NotFound</response>
		[HttpDelete]
		[ServiceFilter(typeof(RenameParameterFilter))]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> Unsubscribe([Required] string subId)
		{
			var creator = await _repository.UsersRepository.GetById(subId);

			#region Validate operation
			if (creator == null)
				return StatusCode(StatusCodes.Status404NotFound, "Nie znaleziono użytkownika o podanym id.");

			var sub = await _repository.SubscriptionsRepository.GetSubscription(subId, GetUserIdFromToken());
			if (sub == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Nie znaleziono subskrypcji.");
			}
			#endregion

			await _repository.SubscriptionsRepository.Delete(sub.Id);
			await _repository.UsersRepository.UpdateSubscriptionCount(subId, -1);

			return StatusCode(StatusCodes.Status200OK, $"Pomyślnie odsubskrybowano użytkownika {creator.Nickname}.");
		}
	}
}
