using MediatR;
using Users.Domain.Entities;
using Users.Application.DTO.Respond;

namespace Users.Application.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserDTO>
    {
        public UserId UserId { get; set; }

        public GetUserByIdQuery(UserId id)
        {
            UserId = id; 
        }
    }
}