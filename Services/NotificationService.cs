using GLMS.Interfaces;

namespace GLMS.Services
{
    public class NotificationService : IObserver
    {
        public void Update(string message, object data)
        {
            // In a real app, this would send email/SMS
            Console.WriteLine($"[NOTIFICATION] {message}");
            // Log to file or database
        }
    }
}