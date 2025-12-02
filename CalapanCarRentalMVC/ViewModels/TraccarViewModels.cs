namespace CalapanCarRentalMVC.ViewModels
{
    public class TraccarDeviceVm
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UniqueId { get; set; } = string.Empty;
    }

    public class TraccarPositionVm
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Accuracy { get; set; }
        public DateTime DeviceTime { get; set; }
        public string? Address { get; set; }
        public string? Protocol { get; set; }
        public IDictionary<string, object>? Attributes { get; set; }
    }
}
