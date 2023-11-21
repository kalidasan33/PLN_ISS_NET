
using System.Collections.Generic;
namespace ISS.BusinessRules
{
    /// <summary>
    /// Sample implementation of a WCF service which can be used along with the MVC application
    /// </summary>
    public class BusinessRuleService : IBusinessRuleService
    {
        /// <summary>
        /// Validates the profile - Sample implementation
        /// </summary>
        /// <param name="inputProfile">The input profile.</param>
        /// <returns></returns>
        public string ValidateProfile(string name)
        {
            ///Do The validation here
            return name;
        }
    }
}
