using System;
using System.Collections.Generic;
using LoggingUtil;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Bootstrapper
{
    public class MethodCallInterceptor : IInterceptionBehavior
    {
        /// <summary>
        /// Intercept a method.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="getNext">The get next.</param>
        /// <returns></returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            // BEFORE the target method execution
            AuditLogger.Audit(String.Format("Invoking method {0}", input.MethodBase));

            // Yield to the next module in the pipeline
            var methodReturn = getNext().Invoke(input, getNext);

            // AFTER the target method execution
            AuditLogger.Audit(methodReturn.Exception == null
                ? String.Format("Method {0} successfully returned {1}", input.MethodBase, methodReturn.ReturnValue)
                : String.Format("Method {0} threw exception {1}", input.MethodBase, methodReturn.Exception.Message));

            return methodReturn;
        }

        /// <summary>
        /// Gets the required interfaces.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Flag to indicate if interceptor will be run.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [will execute]; otherwise, <c>false</c>.
        /// </value>
        public bool WillExecute
        {
            get { return true; }
        }
    }
}
