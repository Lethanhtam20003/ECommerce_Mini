namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public User(string UserName, string Email, string PasswordHash)
        {
            this.UserName = UserName;
            this.Email = Email;
            this.PasswordHash = PasswordHash;
        }

        internal static User Create(string UserName, string email, string password)
        {
            return new User(UserName,email, password) { UserName = UserName, Email = email, PasswordHash = password};
        }
    }
}
