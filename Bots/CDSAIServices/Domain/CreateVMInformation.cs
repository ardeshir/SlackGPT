using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;

namespace CDSAIServices.Domain
{
    public enum EnvironmentTypes
    {
        None = 0,
        Development = 1,
        Test = 2,
        StageUAT = 3
    }
    public class CreateVMInformation
    {
        public string Name { get; set; }
        public int EndOfLifeWeeks { get; set; }
        //public GeneratedVirtualMachineName VirtualMachineName { get; set; }
        public VirtualMachineImage Image { get; set; }
        public string ImageName { get; set; }
        public string RequestedBy { get; set; }
        //public VMSize VMSize { get; set; }
        //public EnvironmentTypes Environment { get; set; }
        // public string EnvironmentString => Enum.GetName<EnvironmentTypes>(Environment);
        public bool? InstallApplication { get; set; }
        //public AddAppToVirtualMachine InstallApplicationInformation { get; set; }

        public string EndOfLifeString => DateTime.Now.AddDays(EndOfLifeWeeks * 7).ToShortDateString();

        public List<string> CreatedVirtualMachines = new();

        public List<Tuple<string, string>> RunbookJobIds = new();

        //public List<DurableFunctionJobStatus> DurableFunctionJobStatues = new();

        public ConversationReference ConversationReference { get; set; }

        public string ConfirmInputText
        {
            get
            {
                return "Please confirm,\n\n" +
                    "We are going to create Virtual Machine(s) using the following values:\n" +
                    $"Total Virtual Machines: \n" +
                    $"Virtual Machine Size: \n" +
                    $"Weeks To Keep: {EndOfLifeWeeks}\n" +
                    $"Image OS: {Image?.OSVersion}\n" +
                    $"Image SQL: {Image?.SQLVersion}\n" +
                    $"Image Build Date: {Image?.BuildDate}\n" +
                    "Is this correct ?";
            }
        }
        public List<string> ChangeOptions
        {
            get 
            {
                List<string> listofOptions = new()
                {
                    $"Weeks To Keep VM(s) - {EndOfLifeWeeks}",
                    $"Base Image for VM(s)"
                };

                if(InstallApplication.HasValue && InstallApplication.Value)
                {

                }

                return listofOptions;
            }
        }

        public CreateVMInformation()
        {
        }
    }
}
