using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserLastNameConverter : ValueConverter<UserLastName, string>
    {
        public UserLastNameConverter() 
            : base(
                  v => v.Value,
                  v => UserLastName.Create(v))
        {
        }
    }
}