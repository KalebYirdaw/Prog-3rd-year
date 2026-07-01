using GLMS.Models;

namespace GLMS.Factories
{
    public abstract class ServiceRequestCreator
    {
        public abstract ServiceRequest CreateServiceRequest(Contract contract, string description, decimal amountUSD);

        protected bool ValidateContract(Contract contract)
        {
            return contract != null && contract.IsValidForService;
        }
    }
}