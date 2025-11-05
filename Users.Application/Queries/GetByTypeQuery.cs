using MediatR;
using Users.Application.DTO.Respond;
using Users.Domain.Entities;

namespace Users.Application.Queries
{
    public class GetByTypeQuery : IRequest<List<GetUserDTO>>
    {
        public UserType UserType { get; set; }

        public GetByTypeQuery(UserType userType)
        {
            UserType = userType;
        }
    }
}