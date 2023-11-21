using System;
using log4net;
using System.Web;

namespace LoggingUtil
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public Logger()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        private String SessionID
        {
            get
            {
                return (HttpContext.Current != null && HttpContext.Current.Session != null) ? ("Session[" + HttpContext.Current.Session.SessionID + "] >> ") : String.Empty;
            }
        }
        /// <summary>
        /// Log the specified message as information.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {

            _logger.Info(SessionID + message);
        }

        /// <summary>
        /// Log the specified message as warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        /// <summary>
        /// Log the specified message as debug.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        /// <summary>
        /// Log the specified message  as error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            _logger.Error(SessionID + message);
        }

        /// <summary>
        /// Log the specified exception  as error.
        /// </summary>
        /// <param name="x">The exception.</param>
        public void Error(Exception x)
        {
            Error(SessionID + BuildExceptionMessage(x));
        }

        /// <summary>
        /// Log the specified message  as error with exception details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="x">The x.</param>
        public void Error(string message, Exception x)
        {
            _logger.Error(SessionID + message, x);
        }

        /// <summary>
        /// Log the specified message  as fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        /// <summary>
        /// Log the specified exception  as fatal error.
        /// </summary>
        /// <param name="x">The x.</param>
        public void Fatal(Exception x)
        {
            Fatal(BuildExceptionMessage(x));
        }

        /// <summary>
        /// Builds the exception message.
        /// </summary>
        /// <param name="x">The exception.</param>
        /// <returns></returns>
        private static string BuildExceptionMessage(Exception x)
        {
            var logException = x;
            if (x.InnerException != null)
                logException = x.InnerException;

            // Get the error message
            var strErrorMsg = Environment.NewLine + "Message :" + logException.Message;

            // Source of the message
            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

            // Stack Trace of the error
            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            // Method where the error occurred
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;
            return strErrorMsg;
        }
    }
}