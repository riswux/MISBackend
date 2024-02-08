namespace MISBackend.DAL.Model.Payload
{
    public class LoginCredentialModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
