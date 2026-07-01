using GLMS.Models;

namespace GLMS.Factories
{
    public class StandardRequestCreator : ServiceRequestCreator
    {
        public override ServiceRequest CreateServiceRequest(Contract contract, string description, decimal amountUSD)
        {
            if (!ValidateContract(contract))
            {
                throw new InvalidOperationException("Cannot create service request: Contract is not valid");
            }

            return new ServiceRequest
            {
                ContractId = contract.ContractId,
                Description = description,
                AmountUSD = amountUSD,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.Now
            };
        }
    }
}