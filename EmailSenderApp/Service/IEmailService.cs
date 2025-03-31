using EmailSenderApp.Domain;

namespace EmailSenderApp.Service;

public interface IEmailService
{
    public Task<bool> Send(Email email);
}