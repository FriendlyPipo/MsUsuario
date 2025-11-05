namespace Users.Domain.Entities
{
    public class User
    {
        public UserId UserId { get; private set; }
        public UserName UserName { get; private set; }
        public UserLastName UserLastName { get; private set; }
        public UserEmail UserEmail { get; private set; }
        public UserPhoneNumber UserPhoneNumber { get; private set; }
        public UserDirection UserDirection { get; private set; }
        public UserType UserType { get; private set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } 
        public User(UserId userId, UserName userName, UserLastName userLastName, UserEmail userEmail, UserPhoneNumber userPhoneNumber, UserDirection userDirection, UserType userType)
        {
            UserId = userId;
            UserName = userName;
            UserLastName = userLastName;
            UserEmail = userEmail;
            UserPhoneNumber = userPhoneNumber;
            UserDirection = userDirection;
            UserType = userType;
            CreatedAt = DateTime.UtcNow;
        }


        public void UpdateUserName(UserName userName)
        {
            UserName = userName;
        }

        public void UpdateUserLastName(UserLastName userLastName)
        {
            UserLastName = userLastName;
        }
        public void UpdateUserPhoneNumber(UserPhoneNumber userPhoneNumber)
        {
            UserPhoneNumber = userPhoneNumber;
        }

        public void UpdateUserDirection(UserDirection userDirection)
        {
            UserDirection = userDirection;
        }

        public void UpdateUserType(UserType userType)
        {
            UserType = userType;
        }

        public string UserTypeToString()
        {
            return UserType.ToString();
        }

    }
}