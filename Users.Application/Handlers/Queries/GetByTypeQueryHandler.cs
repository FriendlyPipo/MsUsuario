using Users.Application.DTO.Respond;
using Users.Application.Queries;
using Users.Core.Repositories;
using Users.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Users.Application.Handlers.Queries
{
    public class GetByTypeQueryHandler : IRequestHandler<GetByTypeQuery, List<GetUserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetByTypeQueryHandler> _logger;

        public GetByTypeQueryHandler(IUserRepository userRepository, ILogger<GetByTypeQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<GetUserDTO>> Handle(GetByTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetByTypeAsync(request.UserType);
                return users.Select(user => new GetUserDTO
                {
                    UserId = user.UserId.Value,
                    UserName = user.UserName.Value,
                    UserLastName = user.UserLastName.Value,
                    UserEmail = user.UserEmail.Value,
                    UserPhoneNumber = user.UserPhoneNumber.Value,
                    UserDirection = user.UserDirection.Value,
                    UserType = user.UserTypeToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios por tipo");
                throw new UserException("Error al obtener usuarios por tipo", ex);  
            }
        }
    }
}