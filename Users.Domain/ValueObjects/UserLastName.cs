namespace Users.Domain.Entities
{
    public class UserLastName : ValueObject
    {
        public string Value { get; }

        private UserLastName(string value)
        {
            Value = value;
        }

        public static UserLastName Create(string value)
        {
            return new UserLastName(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}