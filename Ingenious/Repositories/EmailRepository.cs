using DAS.DataAccess.Helpers;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using Ingenious.Models.EmailDtos;

namespace Ingenious.Repositories
{
    public interface IEmailRepository
    {
        Task<EmailSettingDto> GetEmailSetting(string type);
        Task<EmailTemplateDto> GetEmailTemplate(string type);
        Task<bool> SendEmail(EmailSettingDto emailSetting, string mailTo, string mailSubject, string body);
    }
    public class EmailRepository : IEmailRepository
    {
        private static SemaphoreSlim _smtpSemaphore = new SemaphoreSlim(1);
        private readonly ConnectionStrings _connectionStrings;

        public EmailRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<EmailSettingDto> GetEmailSetting(string type)
        {
            MySqlParameter[] parameters =
            [
                new MySqlParameter("@p_Type", type)
            ];

            return await DbHelper.Get<EmailSettingDto>("GetEmailSetting", parameters, _connectionStrings);
        }

        public async Task<EmailTemplateDto> GetEmailTemplate(string type)
        {
            MySqlParameter[] parameters =
            [
                new MySqlParameter("@p_Type", type)
            ];

            return await DbHelper.Get<EmailTemplateDto>("GetEmailTemplate", parameters, _connectionStrings);
        }

        public async Task<bool> SendEmail(EmailSettingDto emailSetting, string mailTo, string mailSubject, string body)
        {
            try
            {
                await _smtpSemaphore.WaitAsync();
                MailMessage mailMessage = new MailMessage(emailSetting.MailFrom, mailTo);
                mailMessage.Subject = mailSubject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;

                using (SmtpClient smtpClient = new SmtpClient(emailSetting.SMTPType, Convert.ToInt32(emailSetting.Port)))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSetting.Username, emailSetting.Password);

                    await smtpClient.SendMailAsync(mailMessage);

                    smtpClient.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _smtpSemaphore.Release();
            }
        }
    }
}
