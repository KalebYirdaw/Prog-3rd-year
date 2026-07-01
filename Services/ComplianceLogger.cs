using GLMS.Interfaces;

namespace GLMS.Services
{
    public class ComplianceLogger : IObserver
    {
        public void Update(string message, object data)
        {
            // Log compliance-related events
            Console.WriteLine($"[COMPLIANCE LOG] {DateTime.Now}: {message}");
            // Would write to a compliance log file
        }
    }
}