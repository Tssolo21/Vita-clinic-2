namespace VitaClinic.WebAPI.Services
{
    public interface ISmsService
    {
        Task SendAppointmentReminderSmsAsync(string toPhoneNumber, string clientName, string petName, DateTime appointmentDate);
        Task SendSmsAsync(string toPhoneNumber, string message);
    }
}
