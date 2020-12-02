using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SeekQ.UserAssets.Api.Data;

namespace SeekQ.UserAssets.Api.Application.Commands
{
    public class DeleteUserAssetCommandHandler
    {
        public class Command : IRequest<bool>
        {
            public Command(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {

            }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private UserAssetsDbContext _userAssetsDbContext;

            public Handler(UserAssetsDbContext userAssetsDbContext)
            {
                _userAssetsDbContext = userAssetsDbContext;
            }

            public async Task<bool> Handle(
                Command request,
                CancellationToken cancellationToken
            )
            {
                try
                {
                    Guid Id = request.Id;

                    var existingUserAsset = await _userAssetsDbContext.UserAssets.FindAsync(Id);

                    if (existingUserAsset == null)
                    {
                        return false;
                    }

                    _userAssetsDbContext.UserAssets.Remove(existingUserAsset);
                    await _userAssetsDbContext.SaveChangesAsync();

                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
