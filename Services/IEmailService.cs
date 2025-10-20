namespace VitaClinic.WebAPI.Services
{
    public interface IEmailService
    {
        Task SendAppointmentConfirmationAsync(string toEmail, string clientName, string petName, DateTime appointmentDate, string appointmentType);
        Task SendAppointmentReminderAsync(string toEmail, string clientName, string petName, DateTime appointmentDate, string appointmentType);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
