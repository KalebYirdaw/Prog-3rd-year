using GLMS.Models;

namespace GLMS.Factories
{
    public static class ServiceRequestFactorySelector
    {
        public static ServiceRequestCreator GetCreator(ServiceLevel serviceLevel)
        {
            return serviceLevel switch
            {
                ServiceLevel.Hazardous => new HazardousRequestCreator(),
                ServiceLevel.Standard => new StandardRequestCreator(),
                ServiceLevel.Premium => new StandardRequestCreator(), // Premium uses same logic for now
                _ => new StandardRequestCreator()
            };
        }
    }
}