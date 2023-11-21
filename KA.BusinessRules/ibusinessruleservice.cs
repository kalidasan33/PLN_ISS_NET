using System.ServiceModel;

namespace ISS.BusinessRules
{
    [ServiceContract]
    public interface IBusinessRuleService
    {
        [OperationContract]
        string ValidateProfile(string name);
    }
}
