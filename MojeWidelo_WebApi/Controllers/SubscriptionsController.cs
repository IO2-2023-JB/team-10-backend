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
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">NotFound</response>
		[HttpPost]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> Subscribe([Required] string id)
		{
			#region Validate operation
			var creator = await _repository.UsersRepository.GetById(id);

			if (creator == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Podany użytkownik nie istnieje.");
			}

			if (creator.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, $"Użytkownik {creator.Nickname} nie jest twórcą.");
			}

			var subscriberId = GetUserIdFromToken();

			if (subscriberId == id)
			{
				return StatusCode(StatusCodes.Status400BadRequest, "Nie można zasubskrybować swojego konta.");
			}
			#endregion

			var sub = await _repository.SubscriptionsRepository.GetSubscription(id, subscriberId);
			if (sub != null)
			{
				return StatusCode(StatusCodes.Status200OK, $"Twórca {creator.Nickname} jest już zasubskrybowany");
			}

			var subscription = new Subscription() { CreatorId = id, SubscriberId = subscriberId };
			var result = await _repository.SubscriptionsRepository.Create(subscription);

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
			#region Validate operation
			var user = await _repository.UsersRepository.GetById(id);
			if (user == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Podany użytkownik nie istnieje.");
			}
			if (user.UserType != UserType.Creator)
			{
				return StatusCode(StatusCodes.Status400BadRequest, $"Użytkownik {user.Nickname} nie jest twórcą.");
			}
			#endregion

			var task = _repository.SubscriptionsRepository.GetUserSubscriptions(id);
			var subscribersIds = await _subscriptionsManager.GetSubscribersIds(task);
			var subscribers = await _repository.UsersRepository.GetUsersByIds(subscribersIds);

			var subscriptions = _mapper.Map<IEnumerable<SubscriptionDto>>(subscribers);
			var result = new SubscriptionListDto(subscriptions);
			return StatusCode(StatusCodes.Status200OK, result);
		}

		/// <summary>
		/// Unsubscribe from another user
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <response code="200">Ok</response>
		/// <response code="400">Bad request</response>
		/// <response code="401">Unauthorised</response>
		/// <response code="404">NotFound</response>
		[HttpDelete]
		[ServiceFilter(typeof(ObjectIdValidationFilter))]
		public async Task<IActionResult> Unsubscribe([Required] string id)
		{
			var creator = await _repository.UsersRepository.GetById(id);
			if (creator == null)
				return StatusCode(StatusCodes.Status404NotFound, "Nie znaleziono użytkownika o podanym id.");

			var sub = await _repository.SubscriptionsRepository.GetSubscription(id, GetUserIdFromToken());
			if (sub == null)
			{
				return StatusCode(StatusCodes.Status404NotFound, "Nie znaleziono subskrypcji.");
			}

			await _repository.SubscriptionsRepository.Delete(sub.Id);
			return StatusCode(StatusCodes.Status200OK, $"Pomyślnie odsubskrybowano użytkownika {creator.Nickname}.");
		}
	}
}
