using DNA.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface IEmailService {
        Task<bool> SendAsync(string body, string subject, string toAddress, string toName, string[] attachments, params MailAddress[] cc);
        Task<bool> SendAsync(string smtpConfigSectionName, Exception e, IModel model);
        Task<bool> SendAsync(IConfigurationSection section, IModel model, string[] attachments, params MailAddress[] cc);
        string GetHtmlBody(string title, string comment, string buttonText = null, string urlPart = "x", string confirmationCode = null);
        string CreateBodyForConfirmEmail(string confirmationCode);
        string CreateBodyForRecoveryPassword(string confirmationCode);
    }
}
