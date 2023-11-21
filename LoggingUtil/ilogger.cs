using System;

namespace LoggingUtil
{
    public interface ILogger
    {
        /// <summary>
        /// Log the specified message as information.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Log the specified message as warning.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Log the specified message as debug.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Log the specified message  as error.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(string message);

        /// <summary>
        /// Log the specified exception  as error.
        /// </summary>
        /// <param name="x">The exception.</param>
        void Error(Exception x);

        /// <summary>
        /// Log the specified message  as error with exception details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="x">The x.</param>
        void Error(string message, Exception x);

        /// <summary>
        /// Log the specified message  as fatal error.
        /// </summary>
        /// <param name="message">The message.</param>
        void Fatal(string message);

        /// <summary>
        /// Log the specified exception  as fatal error.
        /// </summary>
        /// <param name="x">The x.</param>
        void Fatal(Exception x);
    }
}