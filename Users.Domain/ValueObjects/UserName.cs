namespace Users.Domain.Entities
{
    public class UserName : ValueObject
    {
        public string Value { get; }

        private UserName(string value)
        {
            Value = value;
        }

        public static UserName Create(string value)
        {
            return new UserName(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}