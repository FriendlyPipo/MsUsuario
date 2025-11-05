using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserEmailConverter : ValueConverter<UserEmail, string>
    {
        public UserEmailConverter() 
            : base(
                  v => v.Value,
                  v => UserEmail.Create(v))
        {
        }
    }
}