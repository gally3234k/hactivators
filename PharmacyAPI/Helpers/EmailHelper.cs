using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace PharmacyAPI.Helpers
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public interface IEmailHelper
    {
        Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId, decimal totalAmount, string orderStatus);
        Task SendPrescriptionStatusAsync(string toEmail, string customerName, int prescriptionId, string status, string? reason);
        Task SendOrderStatusUpdateAsync(string toEmail, string customerName, int orderId, string newStatus);
    }

    public class EmailHelper : IEmailHelper
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailHelper> _logger;

        public EmailHelper(IConfiguration configuration, ILogger<EmailHelper> logger)
        {
            _settings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId, decimal totalAmount, string orderStatus)
        {
            var subject = $"Order #{orderId} Confirmation — MediCare Pharmacy";
            var body = $@"
            <html>
            <body style='font-family: Inter, Arial, sans-serif; background: #f8faf8; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <div style='background: linear-gradient(135deg, #2E7D32, #4CAF50); padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 24px;'>🏥 MediCare Pharmacy</h1>
                    </div>
                    <div style='padding: 30px;'>
                        <h2 style='color: #2E7D32;'>Order Confirmed!</h2>
                        <p>Dear {customerName},</p>
                        <p>Your order has been placed successfully. Here are the details:</p>
                        <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                            <tr style='background: #f0f7f0;'>
                                <td style='padding: 12px; font-weight: bold;'>Order ID</td>
                                <td style='padding: 12px;'>#{orderId}</td>
                            </tr>
                            <tr>
                                <td style='padding: 12px; font-weight: bold;'>Total Amount</td>
                                <td style='padding: 12px;'>₹{totalAmount:F2}</td>
                            </tr>
                            <tr style='background: #f0f7f0;'>
                                <td style='padding: 12px; font-weight: bold;'>Status</td>
                                <td style='padding: 12px;'>{orderStatus}</td>
                            </tr>
                        </table>
                        <p>You can track your order status in the <a href='#' style='color: #2E7D32;'>Orders</a> section.</p>
                        <p style='color: #666; font-size: 14px; margin-top: 30px;'>Thank you for choosing MediCare Pharmacy!</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPrescriptionStatusAsync(string toEmail, string customerName, int prescriptionId, string status, string? reason)
        {
            var statusColor = status == "Approved" ? "#2E7D32" : "#d32f2f";
            var statusIcon = status == "Approved" ? "✅" : "❌";

            var subject = $"Prescription #{prescriptionId} {status} — MediCare Pharmacy";
            var body = $@"
            <html>
            <body style='font-family: Inter, Arial, sans-serif; background: #f8faf8; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <div style='background: linear-gradient(135deg, #2E7D32, #4CAF50); padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 24px;'>🏥 MediCare Pharmacy</h1>
                    </div>
                    <div style='padding: 30px;'>
                        <h2 style='color: {statusColor};'>{statusIcon} Prescription {status}</h2>
                        <p>Dear {customerName},</p>
                        <p>Your prescription (ID: #{prescriptionId}) has been <strong style='color: {statusColor};'>{status}</strong>.</p>
                        {(reason != null ? $"<p><strong>Reason:</strong> {reason}</p>" : "")}
                        {(status == "Approved" ? "<p>You can now proceed to order medicines that require this prescription.</p>" : "<p>Please upload a new valid prescription to proceed.</p>")}
                        <p style='color: #666; font-size: 14px; margin-top: 30px;'>Thank you for choosing MediCare Pharmacy!</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderStatusUpdateAsync(string toEmail, string customerName, int orderId, string newStatus)
        {
            var subject = $"Order #{orderId} Status Update — MediCare Pharmacy";
            var body = $@"
            <html>
            <body style='font-family: Inter, Arial, sans-serif; background: #f8faf8; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <div style='background: linear-gradient(135deg, #2E7D32, #4CAF50); padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0; font-size: 24px;'>🏥 MediCare Pharmacy</h1>
                    </div>
                    <div style='padding: 30px;'>
                        <h2 style='color: #2E7D32;'>Order Status Updated</h2>
                        <p>Dear {customerName},</p>
                        <p>Your order <strong>#{orderId}</strong> status has been updated to: <strong style='color: #2E7D32;'>{newStatus}</strong></p>
                        <p style='color: #666; font-size: 14px; margin-top: 30px;'>Thank you for choosing MediCare Pharmacy!</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.SmtpServer))
                {
                    _logger.LogWarning("Email settings not configured. Skipping email to {Email}", toEmail);
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }
    }
}
