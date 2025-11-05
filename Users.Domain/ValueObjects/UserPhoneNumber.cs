namespace Users.Domain.Entities
{
    public class UserPhoneNumber : ValueObject
    {
        public string Value { get; }

        private UserPhoneNumber(string value)
        {
            Value = value;
        }

        public static UserPhoneNumber Create(string value)
        {
            return new UserPhoneNumber(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
