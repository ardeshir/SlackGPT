namespace CDSAIServices.Domain
{
    public class VirtualMachineImage
    {
        public string Name { get; set; }
        public string AppName { get; set; }
        public string OSVersion { get; set; }
        public string SQLVersion { get; set; }
        public string BuildDate { get; set; }
        public string BuildType { get; set; }
        public string Notes { get; set; }
    }
}