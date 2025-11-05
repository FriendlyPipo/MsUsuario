using Users.Application.Queries;
using Users.Core.Repositories;
using Users.Application.DTO.Respond;
using Users.Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Users.Application.Handlers.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(IUserRepository userRepository, ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }   

        public async Task<GetUserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new UserNotFoundException("Usuario no encontrado");
            }

            return new GetUserDTO
            {
                UserId = user.UserId.Value,
                UserName = user.UserName.Value,
                UserLastName = user.UserLastName.Value,
                UserEmail = user.UserEmail.Value,
                UserPhoneNumber = user.UserPhoneNumber.Value,
                UserDirection = user.UserDirection.Value,
                UserType = user.UserType.ToString()
            };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID");
                throw new UserException("Error al obtener usuario por ID", ex);
            }
        }
    }
}