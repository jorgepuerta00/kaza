using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SeekQ.UserAssets.Api.Application.Commands;
using SeekQ.UserAssets.Api.Application.Queries;
using SeekQ.UserAssets.Api.Application.ViewModel;
using SeekQ.UserAssets.Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SeekQ.UserAssets.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly IMediator _mediator;
        public ProfileController(
            IMediator mediator
        )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // GET api/v1/profile
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "get all user assets")]
        public async Task<IEnumerable<UserAssetModel>> GetAllUserAssets(
            [SwaggerParameter(Description = "userId is a Guid Type")]
            [FromRoute] Guid userId
        )
        {
            return await _mediator.Send(new GetUserAssetQueryHandler.Query(userId));
        }

        // POST api/v1/profile
        [HttpPost]
        [SwaggerOperation(Summary = "create user asset")]
        [SwaggerResponse((int)HttpStatusCode.OK, "user assets created succesfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request")]
        public async Task<ActionResult<UserAssetModel>> CreateUserAsset(
            [FromForm] CreateUserAssetCommandHandler.Command command
        )
        {
            return await _mediator.Send(command);
        }

        // PUT api/v1/profile
        [HttpPut]
        [SwaggerOperation(Summary = "update user asset")]
        [SwaggerResponse((int)HttpStatusCode.OK, "user assets updated succesfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad Request")]
        public async Task<ActionResult<UserAssetModel>> UpdateUserAsset(
            [FromBody] UpdateUserAssetCommandHandler.Command command
        )
        {
            return await _mediator.Send(command);
        }

        // DELETE api/v1/profile
        [HttpDelete("{Id}")]
        [SwaggerOperation(Summary = "delete user asset")]
        public async Task<bool> DeleteUserAsset(
            [SwaggerParameter(Description = "userId is a Guid Type")]
            [FromRoute] Guid Id
        )
        {
            return await _mediator.Send(new DeleteUserAssetCommandHandler.Command(Id));
        }
    }
}