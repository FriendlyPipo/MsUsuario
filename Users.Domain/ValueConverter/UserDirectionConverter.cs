using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserDirectionConverter : ValueConverter<UserDirection, string>
    {
        public UserDirectionConverter() 
            : base(
                  v => v.Value,
                  v => UserDirection.Create(v))
        {
        }
    }
}