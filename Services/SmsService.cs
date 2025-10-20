namespace VitaClinic.WebAPI.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAppointmentReminderSmsAsync(string toPhoneNumber, string clientName, string petName, DateTime appointmentDate)
        {
            var message = $"Hi {clientName}, reminder: {petName}'s appointment is on {appointmentDate:MMM dd} at {appointmentDate:hh:mm tt}. - VitaClinic";
            await SendSmsAsync(toPhoneNumber, message);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                // Check if Twilio credentials are configured
                var twilioAccountSid = _configuration["TWILIO_ACCOUNT_SID"] ?? Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                var twilioAuthToken = _configuration["TWILIO_AUTH_TOKEN"] ?? Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
                var twilioPhoneNumber = _configuration["TWILIO_PHONE_NUMBER"] ?? Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

                if (string.IsNullOrEmpty(twilioAccountSid) || string.IsNullOrEmpty(twilioAuthToken) || string.IsNullOrEmpty(twilioPhoneNumber))
                {
                    _logger.LogWarning("Twilio credentials not configured. SMS not sent to {PhoneNumber}", toPhoneNumber);
                    return;
                }

                // Use Twilio API
                using var client = new HttpClient();
                var authString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{twilioAccountSid}:{twilioAuthToken}"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {authString}");

                var formData = new Dictionary<string, string>
                {
                    { "To", toPhoneNumber },
                    { "From", twilioPhoneNumber },
                    { "Body", message }
                };

                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync($"https://api.twilio.com/2010-04-01/Accounts/{twilioAccountSid}/Messages.json", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", toPhoneNumber);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send SMS to {PhoneNumber}. Status: {Status}, Error: {Error}", 
                        toPhoneNumber, response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", toPhoneNumber);
            }
        }
    }
}
