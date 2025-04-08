namespace Library.Api.Repositories
{
    public interface IEmailServiceInterface
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
