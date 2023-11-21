using log4net;

namespace LoggingUtil
{
    public static class AuditLogger
    {
        private static readonly ILog Logger;

        static AuditLogger()
        {
            Logger = LogManager.GetLogger("AuditLog");
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Audit(string message)
        {
            Logger.Info(message);
        }
    }
}