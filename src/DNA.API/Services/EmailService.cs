using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IEmailService), Lifetime.Scoped)]
    public class EmailService : IEmailService {

        readonly IConfiguration _config;
        readonly ILogger<EmailService> _logger;
        readonly IValuerService _valuer;
        IConfigurationSection _smtpConfig;

        const string _blueTable = @"
            <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;'>
	            <tr style='border-collapse:collapse;'>
		            <td align='center' style='padding:0;margin:0;'>
			            <table class='es-content-body' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:transparent;' width='600' cellspacing='0' cellpadding='0' align='center'>
				            <tr style='border-collapse:collapse;'>
					            <td align='left' style='padding:0;margin:0;'>
						            <table width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
							            <tr style='border-collapse:collapse;'>
								            <td width='600' valign='top' align='center' style='padding:0;margin:0;'>
									            <table style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;background-color:{Color:Primary};border-radius:4px;' width='100%' cellspacing='0' cellpadding='0' bgcolor='{Color:Primary}' role='presentation'>
										            [BLUE_TABLE_CONTENT]
									            </table>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr style='border-collapse:collapse;'>
					            <td align='left' style='padding:0;margin:0;'>
						            <table cellpadding='0' cellspacing='0' width='100%' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
							            <tr style='border-collapse:collapse;'>
								            <td width='600' align='center' valign='top' style='padding:0;margin:0;'>
									            <table cellpadding='0' cellspacing='0' width='100%' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
										            <tr style='border-collapse:collapse;'>
											            <td align='center' style='padding:20px;margin:0;font-size:0;'>
												            <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;'>
													            <tr style='border-collapse:collapse;'>
														            <td style='padding:0;margin:0px;border-bottom:1px solid #323545;background:none;height:1px;width:100%;margin:0px;'/>
													            </tr>
												            </table>
											            </td>
										            </tr>
									            </table>
								            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
			            </table>
		            </td>
	            </tr>
            </table>";

        const string _confirmationCodeRows = @"

            <tr style='border-collapse:collapse;'>
              <td align='center' style='margin:0;padding-left:10px;padding-right:10px;padding-top:35px;padding-bottom:35px;'>
                <span class='es-button-border' style='border-style:solid;border-color:{Color:Primary};background:{Color:Primary};border-width:1px;display:inline-block;border-radius:8px;width:auto;'>
                  <a href='{Host}/{Url}/[CONFIRMATION_CODE]' class='es-button' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica,arial,verdana,sans-serif;font-size:20px;color:#FFFFFF;border-style:solid;border-color:{Color:Primary};border-width:15px 30px;display:inline-block;background:{Color:Primary};border-radius:8px;font-weight:normal;font-style:normal;line-height:24px;width:auto;text-align:center;'>
	              {ButtonText}
                  </a>
                </span>
              </td>
            </tr>
            <tr style='border-collapse:collapse;'>
	            <td class='es-m-txt-l' align='left' style='padding:0;margin:0;padding-top:30px;padding-left:30px;padding-right:30px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:18px;font-family:helvetica,arial,verdana,sans-serif;line-height:27px;color:#323545;'>
			            Bu işe yaramazsa, aşağıdaki bağlantıyı kopyalayıp tarayıcınıza yapıştırın:
		            </p>
	            </td>
            </tr>
            <tr style='border-collapse:collapse;'>
	            <td class='es-m-txt-l' align='left' style='padding:0;margin:0;padding-top:20px;padding-left:30px;padding-right:30px;'>
		            <a target='_blank' href='{Host}/{Url}/[CONFIRMATION_CODE]' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica,arial,verdana,sans-serif;font-size:18px;text-decoration:underline;color:#008000;'>
			            {Host}/{Url}/[CONFIRMATION_CODE]
		            </a>
	            </td>
            </tr>";

        const string _linksRow = @"
            <tr style='border-collapse:collapse;'>
                <td align='left' style='padding:0;margin:0;'>
                    <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:21px;color:#999999;'>
                        <strong>
                            [LINKS]
                        </strong>
                    </p>
                </td>
            </tr>";
        const string _linkRow = @"<a target='_blank' href='{Href}' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica, arial, verdana, sans-serif;font-size:14px;text-decoration:underline;color:#999999;'>{Text}</a>";

        const string _helpTextRow = @"
            <tr style='border-collapse:collapse;'>
	            <td class='es-m-txt-l' align='left' style='padding:0;margin:0;padding-top:20px;padding-left:30px;padding-right:30px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:18px;font-family:helvetica, arial, verdana, sans-serif;line-height:27px;color:#323545;'>
			            {HelpText:Text}
		            </p>
	            </td>
            </tr>";

        const string _helpTextExRow = @"
            <tr style='border-collapse:collapse;'>
	            <td align='center' style='padding:0;margin:0;padding-top:30px;padding-left:30px;padding-right:30px;'>
		            <h3 style='margin:0;line-height:24px;mso-line-height-rule:exactly;font-family:lato, helvetica, arial, sans-serif;font-size:20px;font-style:normal;font-weight:normal;color:#323545;'>
			            {HelpTextEx:Text}
		            </h3>
	            </td>
            </tr>
            <tr style='border-collapse:collapse;'>
	            <td esdev-links-color='#ffa73b' align='center' style='margin:0;padding-top:10px;padding-bottom:30px;padding-left:30px;padding-right:30px;'>
		            <a target='_blank' href='{HelpTextEx:Link:Href}' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica, arial, verdana, sans-serif;font-size:16px;text-decoration:underline;color:#FFFFFF;'>
			            {HelpTextEx:Link:Text}
		            </a>
	            </td>
            </tr>";

        const string _signatureRow = @"
            <tr style='border-collapse:collapse;'>
	            <td class='es-m-txt-l' align='left' style='margin:0;padding-left:30px;padding-right:30px;padding-top:40px;padding-bottom:40px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:18px;font-family:helvetica, arial, verdana, sans-serif;line-height:27px;color:#323545;'>
			            {Signature:Text}
		            </p>
	            </td>
            </tr>";

        const string _viewInBrowserRow = @"
            <tr style='border-collapse:collapse;'>
	            <td align='left' style='padding:0;margin:0;padding-top:25px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:21px;color:#999999;'>
			            {ViewInBrowser:Text}
			            <a class='view' target='_blank' href='{ViewInBrowser:Link:Href}' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica, arial, verdana, sans-serif;font-size:14px;text-decoration:underline;color:#999999;'>
				            {ViewInBrowser:Link:Text}
			            </a>
		            </p>
	            </td>
            </tr>";
        const string _unsubscribeRow = @"
            <tr style='border-collapse:collapse;'>
	            <td align='left' style='padding:0;margin:0;padding-top:25px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:21px;color:#666666;'>
			            {Unsubscribe:Text}
			            <a target='_blank' class='unsubscribe' href='{Unsubscribe:Link:Href}' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:helvetica, arial, verdana, sans-serif;font-size:14px;text-decoration:underline;color:#666666;'>
				            {Unsubscribe:Link:Text}
			            </a>
		            </p>
	            </td>
            </tr>";
        const string _addressRow = @"
            <tr style='border-collapse:collapse;'>
	            <td align='left' bgcolor='transparent' style='padding:0;margin:0;padding-top:25px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:21px;color:#666666;'>
			            {Address:Text}
		            </p>
	            </td>
            </tr>";
        const string _sendDateRow = @"
            <tr style='border-collapse:collapse;'>
	            <td align='left' bgcolor='transparent' style='padding:0;margin:0;padding-top:25px;'>
		            <p style='margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:21px;color:#666666;'>
			            [SEND_DATE]
		            </p>
	            </td>
            </tr>";
        const string _exceptionRow = @"
            <p style='margin:16px 0 8px 0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:18px;font-family:helvetica, arial, verdana, sans-serif;line-height:27px;color:#323545;'>
	            [MESSAGE]
            </p>
            <p style='margin:8px 0 32px 0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:helvetica, arial, verdana, sans-serif;line-height:27px;color:#323545;'>
	            [STACK_TRACE]
            </p>
        ";

        string _Address;
        int _Port;
        string _FromName;
        string _FromAddress;
        string _UserName;
        string _Password;
        string _ReplyTo;
        bool _EnableSsl;
        string _EmailsPath;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, IValuerService valuerService) {
            _config = config;
            _smtpConfig = config.GetSection("Config:Smtp");
            _logger = logger;
            _valuer = valuerService;
            _EmailsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", "emails");
            if (!Directory.Exists(_EmailsPath))
                Directory.CreateDirectory(_EmailsPath);
        }

        void LoadSmtpFromConfig() {
            _Address = _smtpConfig.GetValue<string>("Address");
            _Port = _smtpConfig.GetValue<int>("Port");
            _FromName = _valuer.Get(_smtpConfig.GetValue<string>("FromName"));
            _FromAddress = _valuer.Get(_smtpConfig.GetValue<string>("FromAddress"));
            _UserName = _smtpConfig.GetValue<string>("UserName");
            if (string.IsNullOrWhiteSpace(_FromAddress))
                _FromAddress = _UserName;
            _Password = _smtpConfig.GetValue<string>("Password");
            _ReplyTo = _valuer.Get(_smtpConfig.GetValue<string>("ReplyTo"));
            _EnableSsl = _smtpConfig.GetValue<bool>("EnableSsl");
            //#if DEBUG
            //            if (Environment.MachineName == "MAO") {
            //                _Address = "smtp.gmail.com";
            //                _Password = "T9czgYm%q7ZR";
            //                _UserName = "mehmet.orakci@dnaproje.com.tr";
            //                _Port = 587;
            //                _EnableSsl = true;
            //            }
            //#endif
        }

        private async Task<bool> SendInternalAsync(string body, string subject, string toAddress, string toName, string[] attachments, params MailAddress[] cc) {

            LoadSmtpFromConfig();

            MailAddress from = new MailAddress(_FromAddress, _FromName);
            MailAddress to = new MailAddress(toAddress, toName);

            using SmtpClient client = new SmtpClient(_Address, _Port);
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(_UserName, _Password);
            client.EnableSsl = _EnableSsl;

            using MailMessage message = new MailMessage(from, to);

            if (attachments != null)
                if (attachments.Length > 0)
                    foreach (var item in attachments) {
                        message.Attachments.Add(new Attachment(item));
                    }
            if (!string.IsNullOrWhiteSpace(_ReplyTo))
                message.ReplyToList.Add(new MailAddress(_ReplyTo, _FromName));
            message.Priority = MailPriority.Normal;
            message.Subject = subject;

            if (cc != null)
                if (cc.Length > 0)
                    foreach (var item in cc) {
                        message.CC.Add(item);
                    }

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");
            message.AlternateViews.Add(htmlView);

            client.TargetName = string.Empty; // gönderimde smtp client'ın hata vermemesi için eklendi

            client.SendCompleted += (s, e) => { };
            try {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.EmailSendError, ex, ("to", toAddress), ("name", toName), ("subject", subject));
                throw ex;
            }
        }

        public async Task<bool> SendAsync(Exception e, IModel model) {

            var section = _smtpConfig.GetSection($"ExceptionEmailSettings");
            var bodySection = section.GetSection("Body");
            //var title = model == null ? bodySection.GetValue<string>("Title") : model.Format(bodySection.GetValue<string>("Title"));
            var title = _valuer.Get(bodySection.GetValue<string>("Title"));
            //var comment = model == null ? bodySection.GetValue<string>("Comment") : model.Format(bodySection.GetValue<string>("Comment"));
            var comment = _valuer.Get(bodySection.GetValue<string>("Comment"));

            if (string.IsNullOrWhiteSpace(comment))
                comment = "[EXCEPTION]";
            else if (!comment.Contains("[EXCEPTION]"))
                comment += "[EXCEPTION]";

            string SetExcetion(string c, Exception ex) {
                c += _exceptionRow
                    .Replace("[MESSAGE]", e.Message)
                    .Replace("[STACK_TRACE]", e.StackTrace);

                if (ex.InnerException != null)
                    SetExcetion(c, ex.InnerException);

                return c;
            }

            var rows = SetExcetion("", e);

            var uniqueId = Guid.NewGuid().ToString();

            var htmlBody = GetHtmlBody(title, comment.Replace("[EXCEPTION]", rows), null, null, null, uniqueId);

            //var subject = model == null ? section.GetValue<string>("Subject") : model.Format(section.GetValue<string>("Subject"));
            var subject = _valuer.Get(bodySection.GetValue<string>("Subject"));

            var toAddress = section.GetValue<string>("To");
            var toName = section.GetValue<string>("ToName");

            SaveToFile(uniqueId, new {
                toName,
                toAddress,
                subject,
                replyTo = _ReplyTo,
                htmlView = htmlBody
            });

            return await SendInternalAsync(htmlBody, subject, toAddress, toName, null);
        }

        public async Task<bool> SendAsync(string fullSectionPath, string[] attachments, params MailAddress[] cc) {
            return await SendAsync(fullSectionPath, confirmCode: null, attachments: attachments, cc: cc);
        }

        public async Task<bool> SendAsync(string fullSectionPath, string confirmCode, string[] attachments, params MailAddress[] cc) {
            var section = _config.GetSection(fullSectionPath);

            if (!section.Exists()) {
                section = _config.GetSection("Config:" + fullSectionPath);
                if (!section.Exists()) {
                    section = _config.GetSection("Config:EMails:" + fullSectionPath);
                    if (!section.Exists()) {
                        section = _config.GetSection("Config:Smtp:" + fullSectionPath);
                        if (!section.Exists())
                            throw new Exception("'" + fullSectionPath + "' not found.");
                    }
                }
            }

            if (section.GetSection("Enabled").Exists())
                if (!section.GetValue<bool>("Enabled"))
                    return false;

            return await SendAsync(section, model: null, confirmCode: confirmCode, attachments: attachments, cc: cc);
        }

        private async Task<bool> SendAsync(IConfigurationSection section, IModel model, string[] attachments, params MailAddress[] cc) {
            return await SendAsync(section, model, null, attachments, cc);
        }
        private async Task<bool> SendAsync(IConfigurationSection section, IModel model, string confirmCode, string[] attachments, params MailAddress[] cc) {

            var enabled = section.GetValue<bool>("Enabled");
            if (!enabled)
                return false;
            var uniqueId = Guid.NewGuid().ToString();
            var bodySection = section.GetSection("Body");
            var comment = _valuer.Get(bodySection["Comment"]);
            var subject = _valuer.Get(section["Subject"]);
            var title = _valuer.Get(bodySection["Title"]);
            var buttonText = _valuer.Get(bodySection["ButtonText"]);
            var confirmationCode = _valuer.Get(confirmCode ?? bodySection["ConfirmationCode"]);
            var urlPart = _valuer.Get(bodySection["Url"]);
            var htmlBody = GetHtmlBody(title, comment, buttonText, urlPart, confirmationCode, uniqueId);
            var to = string.IsNullOrWhiteSpace(section["To"]) ? _valuer.Get("{ApplicationUser.Email}") : _valuer.Get(section["To"]);
            var name = string.IsNullOrWhiteSpace(section["ToName"]) ? _valuer.Get("{ApplicationUser.FullName}") : _valuer.Get(section["ToName"]);

            List<MailAddress> address = new List<MailAddress>();
            if (cc != null)
                if (cc.Length > 0)
                    foreach (var item in cc) {
                        address.Add(item);
                    }

            SaveToFile(uniqueId, new {
                toName = name,
                toAddress = to,
                cc,
                subject,
                replyTo = _ReplyTo,
                attachments,
                htmlView = htmlBody
            });

            return await SendInternalAsync(htmlBody, subject, to, name, attachments, address.ToArray());
        }

        private string GetHtmlBody(string title, string comment, string buttonText = null, string urlPart = "x", string confirmationCode = null, string uniqueId = null) {

            var bodySection = _smtpConfig.GetSection("Body");

            var linksVisible = bodySection.GetSection("LinksRows").GetValue<bool>("Visible");
            var viewInBrowserVisible = bodySection.GetSection("ViewInBrowser").GetValue<bool>("Visible");
            var unsubscribeVisible = bodySection.GetSection("Unsubscribe").GetValue<bool>("Visible");
            var addressVisible = bodySection.GetSection("Address").GetValue<bool>("Visible");
            var helpTextVisible = bodySection.GetSection("HelpText").GetValue<bool>("Visible");
            var signatureVisible = bodySection.GetSection("Signature").GetValue<bool>("Visible");
            var helpTextExVisible = bodySection.GetSection("HelpTextEx").GetValue<bool>("Visible");

            var htmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\assets\\EmailTemplate.html");
            var body = File.ReadAllText(htmlFile);

            body = body
                .Replace("[CONFIRMATION_CODE_ROWS]", !string.IsNullOrWhiteSpace(confirmationCode) ? _confirmationCodeRows : "")
                .Replace("[CONFIRMATION_CODE]", !string.IsNullOrWhiteSpace(confirmationCode) ? confirmationCode : "-")
                .Replace("[HELP_TEXT_ROW]", helpTextVisible ? _helpTextRow : "")
                .Replace("[SIGNATURE_ROW]", signatureVisible ? _signatureRow : "")
                .Replace("[LINKS_ROW]", linksVisible ? _linksRow : "")
                .Replace("[VIEW_IN_BROWSER_ROW]", viewInBrowserVisible ? _viewInBrowserRow : "")
                .Replace("[ADDRESS_ROW]", addressVisible ? _addressRow : "")
                .Replace("[SEND_DATE_ROW]", _sendDateRow).Replace("[SEND_DATE]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz") + " " + Environment.MachineName)
                ;

            body = body.Replace("[UNSUBSCRIBE_ROW]", unsubscribeVisible ? _unsubscribeRow : "");

            if (helpTextExVisible) {
                body = body
                    .Replace("[BLUE_TABLE]", _blueTable)
                    .Replace("[BLUE_TABLE_CONTENT]", "[HELP_TEXT_EX_ROW]")
                    .Replace("[HELP_TEXT_EX_ROW]", _helpTextExRow)
                    ;
            }
            else
                body = body.Replace("[BLUE_TABLE]", "");

            if (linksVisible) {
                var links = bodySection.GetSection("LinksRows:Links").GetChildren().ToArray();
                var linkRows = new string[links.Length];
                var i = 0;
                foreach (var item in links) {
                    linkRows[i] = _linkRow.Replace("{Href}", item["Href"]).Replace("{Text}", item["Text"]);
                    i++;
                }
                body = body.Replace("[LINKS]", string.Join(" - ", linkRows));
            }
            body = body
                .Replace("{Title}", title)
                .Replace("{Comment}", comment)
                ;
            if (!string.IsNullOrWhiteSpace(buttonText))
                body = body.Replace("{ButtonText}", buttonText);

            if (!string.IsNullOrWhiteSpace(urlPart))
                body = body.Replace("{Url}", urlPart);

            var result = "";
            var pattern = @"{(?<field>[\w:]+)}";
            void Replace(string b) {
                result = Regex.Replace(b, pattern, (m) => {
                    var val = bodySection[m.Groups["field"].Value];
                    return val;
                });
                result = result.Replace("[CONFIRMATION_CODE]", !string.IsNullOrWhiteSpace(confirmationCode) ? confirmationCode : "-");
                result = result.Replace("[UniqueId]", uniqueId);
                if (Regex.IsMatch(result, pattern))
                    Replace(result);
            }
            Replace(body);

            return Regex.Replace(result.Replace("\r\n", "").Replace("\t", ""), @"\s+", " ");
        }

        private void SaveToFile(string uniqueId, dynamic data) {
            try {
                File.WriteAllText(Path.Combine(_EmailsPath, $"{uniqueId}.json"), JsonConvert.SerializeObject(data));
            }
            finally {
            }
        }

    }
}
