namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string PasswordHash { get; set; }

        public User(string Name, string PasswordHash)
        {
            this.Name = Name;
            this.PasswordHash = PasswordHash;
        }

        internal static User Create(string email, string password)
        {
            return new User(email, password) { Name = email, PasswordHash = password};
        }
    }
}
