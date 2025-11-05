using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Domain.Entities;

namespace Users.Domain.ValueConverter
{
    public class UserPhoneNumberConverter : ValueConverter<UserPhoneNumber, string>
    {
        public UserPhoneNumberConverter() 
            : base(
                  v => v.Value,
                  v => UserPhoneNumber.Create(v))
        {
        }
    }
}