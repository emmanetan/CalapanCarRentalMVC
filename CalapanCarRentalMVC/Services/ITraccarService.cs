using CalapanCarRentalMVC.ViewModels;

namespace CalapanCarRentalMVC.Services
{
    public interface ITraccarService
    {
        Task<IReadOnlyList<TraccarDeviceVm>> GetDevicesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TraccarPositionVm>> GetLatestPositionsAsync(CancellationToken ct = default);
    }
}
