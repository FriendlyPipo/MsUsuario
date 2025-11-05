namespace Users.Domain.Entities
{
    public class UserDirection : ValueObject
    {
        public string Value { get; }

        private UserDirection(string value)
        {
            Value = value;
        }

        public static UserDirection Create(string value)
        {
            return new UserDirection(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}