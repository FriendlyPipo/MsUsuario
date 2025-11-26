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
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDbContext _dbContext;
        private readonly IKeycloakRepository _keycloakRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            UserDbContext dbContext,
            IKeycloakRepository keycloakRepository,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _keycloakRepository = keycloakRepository;
            _logger = logger;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {             
                // Validación general 
                var validacion = new UserValidation();
                await validacion.ValidateRequest(request.Users);

                 var token = await _keycloakRepository.GetTokenAsync();
                        var kcUser = new
                        {
                            username = request.Users.UserEmail,
                            email = request.Users.UserEmail,
                            enabled = true,
                            firstName = request.Users.UserName,
                            lastName = request.Users.UserLastName,
                            credentials = new[] { new { type = "password", value = request.Users.UserPassword, temporary = false } },
                            requiredActions = new[] { "VERIFY_EMAIL" },
                        };
                var createUserResponseJson = await _keycloakRepository.CreateUserAsync(kcUser, token);

                string keycloakUserId = await _keycloakRepository.GetUserIdAsync(kcUser.username, token);
                

                await _keycloakRepository.AssignTypeToUserAsync(keycloakUserId, request.Users.UserType, token);

                Guid.TryParse(keycloakUserId, out Guid DbId);

                var userNameValue = request.Users.UserName;
                var userLastNameValue = request.Users.UserLastName;
                var userEmailValue = request.Users.UserEmail;
                var userPhoneNumberValue = request.Users.UserPhoneNumber;
                var userDirectionValue = request.Users.UserDirection;

                var newUser = new User(
                    UserId.Create(DbId),
                    UserName.Create(userNameValue),
                    UserLastName.Create(userLastNameValue),
                    UserEmail.Create(userEmailValue),
                    UserPhoneNumber.Create(userPhoneNumberValue),
                    UserDirection.Create(userDirectionValue),
                    Enum.Parse<UserType>(request.Users.UserType!)
                );

                using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                                
                 try
                    {
                        _logger.LogInformation("Creando usuario en base de datos");
                        await _userRepository.CreateAsync(newUser);
                        await _keycloakRepository.SendVerificationEmailAsync(keycloakUserId, token);
                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation("Usuario creado exitosamente:", newUser.UserId);
                        return newUser.UserId.Value.ToString();
                    }        
                catch (Exception ex)
                    {
                    await transaction.RollbackAsync(cancellationToken);

                    if (ex is DbUpdateException) 
                        {
                        _logger.LogError(ex, "Error al guardar usuario en base de datos");
                        throw new UserException("Error al guardar usuario en base de datos", ex);
                        }
                    else
                        {
                        _logger.LogError(ex, "Error al crear usuario en Keycloak");
                        throw new KeycloakException("Error al crear usuario en Keycloak o enviar email", ex);
                        }
                }     
        }
    }
}