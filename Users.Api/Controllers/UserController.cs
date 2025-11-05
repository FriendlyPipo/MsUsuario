using Microsoft.AspNetCore.Mvc;
using Users.Application.DTO.Request;
using Users.Application.DTO.Respond;
using Users.Application.Queries;
using Users.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System; 
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Users.Domain.Entities;

namespace Users.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("CreateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(CreateUserDTO createUserDTO)
        {
            try
            {
                var command = new CreateUserCommand(createUserDTO);
                var userId = await _mediator.Send(command);
                _logger.LogInformation("Usuario creado exitosamente con ID: {UserId}", userId);
                return Ok(new { UserId = userId, Message = "Usuario creado exitosamente. Por favor, revisa tu correo para confirmar." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al crear usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

       /* 
        [HttpDelete("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(DeleteUserDTO deleteUserDTO)
        {
            try
            {
                var command = new DeleteUserCommand(deleteUserDTO);
                var userId = await _mediator.Send(command);
                _logger.LogInformation("Usuario eliminado exitosamente con ID: {UserId}", userId);
                return Ok(new { UserId = userId, Message = "Usuario eliminado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al eliminar usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }
        */

        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            try
            {
                var command = new UpdateUserCommand(updateUserDTO);
                var userId = await _mediator.Send(command);
                _logger.LogInformation("Usuario actualizado exitosamente con ID: {UserId}", userId);
                return Ok(new { UserId = userId, Message = "Usuario actualizado exitosamente." });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al actualizar usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("GetUserById")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid usersId)
        {
            try
            {
                UserId userId = UserId.Create(usersId);
                var query = new GetUserByIdQuery(userId);
                var user = await _mediator.Send(query);
                if (user == null)
                {
                    _logger.LogWarning("GetUserById: Usuario con ID {UserId} no encontrado.", usersId);
                    return NotFound(new { Message = $"Usuario con ID {usersId} no encontrado." });
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener usuario por ID: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var query = new GetAllUsersQuery();
                var users = await _mediator.Send(query);
                _logger.LogInformation("GetAllUsers: Obtenidos {Count} usuarios.", users?.Count ?? 0);
                return Ok(users);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener todos los usuarios: {ErrorMessage}", e.Message);
                return BadRequest(new { Message = e.Message });
            }
        }

        /*
        [HttpGet("GetActivity")]
        [Authorize]
        public async Task<IActionResult> GetUserActivity(Guid userId)
        {
            try
            {
                var query = new GetUserActivityQuery(userId);
                var activity = await _mediator.Send(query);
                if (activity == null || activity.Count == 0)
                {
                    _logger.LogWarning("GetUserActivity: No se encontraron actividades para el usuario con ID {UserId}.", userId);
                    return NotFound(new { Message = $"No se encontraron actividades para el usuario con ID {userId}." });
                }
                return Ok(activity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener actividad del usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { message = e.Message });
            }
        }
        

        [HttpGet("GetActivityByType")]
        [Authorize]
        public async Task<IActionResult> GetUserActivityByType(Guid userId, string activityType)
        {
            try 
            {
                var query = new GetUserActivityByTypeQuery(userId, activityType);
                var activity = await _mediator.Send(query);
                if (activity == null || activity.Count == 0)
                {
                    _logger.LogWarning("GetUserActivityByType: No se encontraron actividades para el usuario con ID {UserId} y tipo {ActivityType}.", userId, activityType);
                    return NotFound(new { Message = $"No se encontraron actividades para el usuario con ID {userId} y tipo {activityType}." });
                }
                return Ok(activity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener actividad del usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("GetActivityByDate")]
        [Authorize]
        public async Task<IActionResult> GetUserActivityByDate(Guid userId, DateTime activityDate)
        {
            try
            {
                var query = new GetUserActivityByDateQuery(userId, activityDate);
                var activity = await _mediator.Send(query);
                if (activity == null || activity.Count == 0)
                {
                    _logger.LogWarning("No se encontraron actividades para el usuario con ID {UserId} y fecha {ActivityDate}.", userId, activityDate);
                    return NotFound(new { Message = $"No se encontraron actividades para el usuario con ID {userId} y fecha {activityDate}." });
                }
                return Ok(activity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al obtener actividad del usuario: {ErrorMessage}", e.Message);
                return BadRequest(new { message = e.Message });
            }
        }

        */

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO requestDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Solicitud de ForgotPassword inválida: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new ForgotPasswordCommand(requestDto);
                var resultMessage = await _mediator.Send(command);

                return Ok(new { message = resultMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar solicitud de ForgotPassword para email: {Email}", requestDto.UserEmail);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al procesar tu solicitud." });
            }
        }

        /*
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Solicitud de Login inválida: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new LoginUserCommand(loginUserDTO);
                var resultMessage = await _mediator.Send(command);

                return Ok(new { message = resultMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al procesar solicitud de Login para email: {Email}", loginUserDTO.UserEmail);
                return StatusCode(500, new { message = "Ocurrió un error inesperado al procesar tu solicitud." });
            }
        }
        */
    }
}