using System.Net;
using System.Net.Mail;

namespace VitaClinic.WebAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAppointmentConfirmationAsync(string toEmail, string clientName, string petName, DateTime appointmentDate, string appointmentType)
        {
            var subject = "Appointment Confirmation - VitaClinic";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #667eea;'>Appointment Confirmed</h2>
                    <p>Dear {clientName},</p>
                    <p>Your appointment has been confirmed for your pet <strong>{petName}</strong>.</p>
                    <div style='background-color: #f0f0f0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Date & Time:</strong> {appointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
                        <p><strong>Type:</strong> {appointmentType}</p>
                    </div>
                    <p>Please arrive 10 minutes early. If you need to reschedule, please contact us.</p>
                    <p>Best regards,<br>VitaClinic Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAppointmentReminderAsync(string toEmail, string clientName, string petName, DateTime appointmentDate, string appointmentType)
        {
            var subject = "Appointment Reminder - VitaClinic";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #667eea;'>Appointment Reminder</h2>
                    <p>Dear {clientName},</p>
                    <p>This is a friendly reminder about your upcoming appointment for <strong>{petName}</strong>.</p>
                    <div style='background-color: #f0f0f0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Date & Time:</strong> {appointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
                        <p><strong>Type:</strong> {appointmentType}</p>
                    </div>
                    <p>We look forward to seeing you and {petName}!</p>
                    <p>Best regards,<br>VitaClinic Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Check if SendGrid API key is configured
                var sendGridApiKey = _configuration["SENDGRID_API_KEY"] ?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                
                if (string.IsNullOrEmpty(sendGridApiKey))
                {
                    _logger.LogWarning("SendGrid API key not configured. Email not sent to {Email}", toEmail);
                    return;
                }

                // Use SendGrid API
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {sendGridApiKey}");

                var fromEmail = _configuration["Email:FromAddress"] ?? "noreply@vitaclinic.com";
                var fromName = _configuration["Email:FromName"] ?? "VitaClinic";

                var emailData = new
                {
                    personalizations = new[]
                    {
                        new
                        {
                            to = new[] { new { email = toEmail } },
                            subject = subject
                        }
                    },
                    from = new { email = fromEmail, name = fromName },
                    content = new[]
                    {
                        new
                        {
                            type = "text/html",
                            value = body
                        }
                    }
                };

                var json = System.Text.Json.JsonSerializer.Serialize(emailData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {Email}. Status: {Status}, Error: {Error}", 
                        toEmail, response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
            }
        }
    }
}
