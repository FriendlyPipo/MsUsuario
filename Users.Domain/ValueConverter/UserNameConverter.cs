using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserNameConverter : ValueConverter<UserName, string>
    {
        public UserNameConverter() 
            : base(
                  v => v.Value,
                  v => UserName.Create(v))
        {
        }
    }
}