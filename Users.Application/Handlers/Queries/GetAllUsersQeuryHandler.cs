using Users.Application.DTO.Respond;
using Users.Application.Queries;
using Users.Core.Repositories;
using Users.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Users.Application.Handlers.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<GetUserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(IUserRepository userRepository, ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<GetUserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    throw new UserNotFoundException("No se encontraron usuarios");
                }

                return users.Select(user => new GetUserDTO
                {
                    UserId = user.UserId.Value,
                    UserName = user.UserName.Value,
                    UserLastName = user.UserLastName.Value,
                    UserEmail = user.UserEmail.Value,
                    UserPhoneNumber = user.UserPhoneNumber.Value,
                    UserDirection = user.UserDirection.Value,
                    UserType = user.UserType.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new UserException("Error al obtener todos los usuarios", ex);
            }
        }
    }
}