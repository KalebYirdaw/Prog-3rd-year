using GLMS.Models;

namespace GLMS.Factories
{
    public class HazardousRequestCreator : ServiceRequestCreator
    {
        public override ServiceRequest CreateServiceRequest(Contract contract, string description, decimal amountUSD)
        {
            if (!ValidateContract(contract))
            {
                throw new InvalidOperationException("Cannot create service request: Contract is not valid");
            }

            // Additional validation for hazardous goods
            if (string.IsNullOrWhiteSpace(description) || !description.ToLower().Contains("hazardous"))
            {
                // Auto-tag as hazardous if contract type is hazardous
                description = $"[HAZARDOUS] {description}";
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