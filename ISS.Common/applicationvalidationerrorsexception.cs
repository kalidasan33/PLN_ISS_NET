using System;
using System.Collections.Generic;

namespace ISS.Common
{
    public class ApplicationValidationErrorsException : Exception
    {
        private readonly IEnumerable<string> _validationErrors;

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>
        /// The validation errors.
        /// </value>
        public IEnumerable<string> ValidationErrors
        {
            get { return _validationErrors; }
        }

        public ApplicationValidationErrorsException(IEnumerable<string> validationErrors)
            : base("Invalid type, expected is RegisterTypesMapConfigurationElement")
        {
            _validationErrors = validationErrors;
        }
    }
}