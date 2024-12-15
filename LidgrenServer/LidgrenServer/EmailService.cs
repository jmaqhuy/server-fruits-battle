using System.Net.Mail;
using System.Net;
using LidgrenServer.Models;

namespace LidgrenServer
{
    public class EmailService
    {
        private string fromEmail = "fruitsbattlegame@gmail.com";
        private string fromPassword = "wisjxkcigjafxlvr";

        public EmailService()
        {
        }

        private async Task<bool> SendMail(string toEmail, string subject, string htmlBody)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, "Fruits Battle Game");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);
                Logging.Info($"Send {subject} Mail Successful");
                return true;
            }
            catch (Exception ex)
            {
                Logging.Error($"Error sending {subject} mail: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendMailResetPassword(string username, string userEmail, string newPassword)
        {
            
            string htmlBody = $@"
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        text-align: center;
                    }}
                    .container {{
                        background-color: #ffffff;
                        margin: 50px auto;
                        padding: 0px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        max-width: 600px;
                        text-align: left;
                    }}
                    .logo {{
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    .content {{
                        font-size: 16px;
                        color: #333333;
                    }}
                    .highlight {{
                        font-weight: bold;
                        color: #1a73e8;
                    }}
                    .reset-code-container {{
                        text-align: center;
                        margin: 30px 0;
                    }}
                    .reset-code {{
                        display: inline-block;
                        padding: 15px 30px;
                        font-size: 20px;
                        color: #ffffff;
                        background-color: #1a73e8;
                        border-radius: 5px;
                    }}
                    .contact-info {{
                        margin-top: 20px;
                        font-size: 14px;
                        color: #555555;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 12px;
                        color: #777777;
                        text-align: center;
                    }}
                    a {{
                        color: #1a73e8;
                        text-decoration: none;
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""logo"">
                        <img src='https://i.postimg.cc/QtgB3VQq/logo-Fruits-Battle-Game-2d.png' alt='Game Logo' height='100' />
                    </div>
                    <div class=""content"">
                        <p>Dear <span class=""highlight"">{username}</span>,</p>
                        <p>You are receiving this email because a request has been made to reset the password for your Fruits Battle Game account. For security reasons, please do not share this email with anyone. Your new password is:</p>
                        <div class=""reset-code-container"">
                            <div class=""reset-code"">{newPassword}</div>
                        </div>
                        <p>If you did not request a password reset, please ignore this email.</p>
                        <p class=""contact-info"">If you have any questions, feel free to contact our support team at <a href=""mailto:fruitsbattlegame@gmail.com"">fruitsbattlegame@gmail.com</a>.</p>
                        <p style=""font-size: 14px; color: #1d4579;"">
                            Sincerely,
                            <br>
                            The Fruits Battle Game Support Team
                        </p>
                    </div>
                    <div class=""footer"">
                        <p><small>This email was sent automatically. Please do not reply to this email.</small></p>
                    </div>
                </div>
            </body>
            </html>";

            return await SendMail(userEmail, "Request Reset Password", htmlBody);
        }

        public async Task<bool> SendMailVerifyRegistration(string username, string userEmail, DateTime registeredAt, string otp)
        {
            
            string htmlBody = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Account Verification</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f9;
                        margin: 0;
                        padding: 0;
                        color: #333333;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 30px auto;
                        background: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}
                    .header {{
                        background: #4CAF50;
                        color: #ffffff;
                        padding: 20px;
                        text-align: center;
                    }}
                    .header img {{
                        max-width: 100px;
                        margin-bottom: 10px;
                    }}
                    .content {{
                        padding: 20px;
                        line-height: 1.6;
                    }}
                    .content h2 {{
                        color: #4CAF50;
                        margin-bottom: 15px;
                    }}
                    .content p {{
                        margin: 10px 0;
                    }}
                    .content .highlight {{
                        background: #e8f5e9;
                        padding: 10px;
                        border-left: 4px solid #4CAF50;
                        font-size: 16px;
                    }}
                    .btn {{
                        display: block;
                        width: 30%;
                        margin: 20px auto;
                        text-align: center;
                        background: #4CAF50;
                        color: #ffffff;
                        text-decoration: none;
                        padding: 15px 20px;
                        font-size: 18px;
                        border-radius: 4px;
                        transition: background 0.3s;
                    }}
                    .btn:hover {{
                        background: #45a049;
                    }}
                    .footer {{
                        text-align: center;
                        font-size: 12px;
                        color: #777777;
                        padding: 10px;
                        background: #f4f4f9;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <img src='https://i.postimg.cc/QtgB3VQq/logo-Fruits-Battle-Game-2d.png' alt='Fruits Battle Game'>
                        <h1>Welcome to Fruits Battle Game!</h1>
                    </div>
                    <div class='content'>
                        <h2>Hello, {username}!</h2>
                        <p>Thank you for joining <strong>Fruits Battle Game</strong>. Your account has been successfully created. Below are your account details and we have a small gift for you:</p>
                        <div class='highlight'>
                            <p><strong>Username:</strong> {username}</p>
                            <p><strong>Registered at:</strong> {registeredAt.ToString("yyyy-MM-dd HH:mm:ss")}</p>
                            <p><strong>Your Gift Code:</strong> <span style='color: #4CAF50; font-weight: bold;'>1234</span></p>
                        </div>
                        <p>Your One-Time Password (OTP) is displayed below. Please use it to verify your account:</p>
                        <a class='btn'>{otp}</a>
                        <p>If you did not register for this account, please ignore this email or contact our support team.</p>
                    </div>
                    <div class='footer'>
                        <p>&copy; {DateTime.Now.Year} Fruits Battle Game. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";

            return await SendMail(userEmail, "Fruits Battle Game Verification", htmlBody);
        }
    }
}
