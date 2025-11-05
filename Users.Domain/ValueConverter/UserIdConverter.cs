using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserIdConverter : ValueConverter<UserId, Guid>
    {
        public UserIdConverter() 
            : base(
                  v => v.Value,
                  v => UserId.Create(v))
        {
        }
    }
}