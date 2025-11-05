    using MediatR;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Users.Domain.Entities;
    using Users.Application.Commands;
    using Users.Infrastructure.Database;
    using Users.Core.Repositories;
    using Users.Infrastructure.Exceptions;
    using Users.Application.UserValidations;
    using Users.Application.DTO.Request; 
    using System.Text.Json;
    using Microsoft.Extensions.Logging;

namespace Users.Application.Handlers.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _dbContext;
        private readonly IKeycloakRepository _keycloakRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository userRepository, UserDbContext dbContext, IKeycloakRepository keycloakRepository, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _keycloakRepository = keycloakRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = UserId.Create(request.Users.UserId);
            var updatedUser = await _userRepository.GetByIdAsync(userId);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (request.Users.UserName != null) updatedUser.UpdateUserName(UserName.Create(request.Users.UserName));
                if (request.Users.UserLastName != null) updatedUser.UpdateUserLastName(UserLastName.Create(request.Users.UserLastName));
                if (request.Users.UserPhoneNumber != null) updatedUser.UpdateUserPhoneNumber(UserPhoneNumber.Create(request.Users.UserPhoneNumber));
                if (request.Users.UserDirection != null) updatedUser.UpdateUserDirection(UserDirection.Create(request.Users.UserDirection));
                if (request.Users.UserType != null) updatedUser.UpdateUserType(Enum.Parse<UserType>(request.Users.UserType!));

                await _userRepository.UpdateAsync(updatedUser);

                var token = await _keycloakRepository.GetTokenAsync();
                var keycloakUserId = await _keycloakRepository.GetUserIdAsync(updatedUser.UserEmail.Value, token);

                if (string.IsNullOrEmpty(keycloakUserId))
                {
                    throw new KeycloakException($"No se encontró el usuario en Keycloak con el email: {updatedUser.UserEmail.Value}");
                }
                
                object kcUser;
                if (request.Users.UserPassword != null)
                {
                    kcUser = new
                    {
                        username = updatedUser.UserEmail.Value,
                        email = updatedUser.UserEmail.Value,
                        enabled = true,
                        firstName = updatedUser.UserName.Value,
                        lastName = updatedUser.UserLastName.Value,
                        credentials = new[] { new { type = "password", value = request.Users.UserPassword, temporary = false } }
                    };
                }
                else
                {
                    kcUser = new
                    {
                        username = updatedUser.UserEmail.Value,
                        email = updatedUser.UserEmail.Value,
                        enabled = true,
                        firstName = updatedUser.UserName.Value,
                        lastName = updatedUser.UserLastName.Value,
                    };
                }

                try 
                {
                    await _keycloakRepository.UpdateUserAsync(kcUser, keycloakUserId, token);
                }
                catch (KeycloakException kex)
                {
                    _logger.LogError(kex, "Error al actualizar usuario en Keycloak. Usuario: {@User}", new { 
                        Email = updatedUser.UserEmail.Value, 
                        KeycloakId = keycloakUserId 
                    });
                    throw;
                }

                await transaction.CommitAsync(cancellationToken);
                
                return "Usuario Actualizado Correctamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la actualización del usuario");
                await transaction.RollbackAsync(cancellationToken);

                if (ex is KeycloakException || 
                    ex is UserNotFoundException)
                {
                    throw;
                }

                throw new UserException("Error al actualizar usuario", ex);
            }
        }
    }
}

