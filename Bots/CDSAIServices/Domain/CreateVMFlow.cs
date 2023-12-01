using System.Collections.Generic;

namespace CDSAIServices.Domain
{
    public class CreateVMFlow
    {
        public List<VirtualMachineImage> VMImages { get; set; }
        public List<StorageAccount> StorageAccounts { get; set; }
    }
}
