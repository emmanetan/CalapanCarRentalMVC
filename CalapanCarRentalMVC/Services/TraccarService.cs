using System.Net.Http.Headers;
using System.Text.Json;
using CalapanCarRentalMVC.ViewModels;

namespace CalapanCarRentalMVC.Services
{
    public class TraccarService : ITraccarService
    {
        private readonly HttpClient _http;
        private readonly TraccarOptions _options;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public TraccarService(HttpClient http, TraccarOptions options)
        {
            _http = http;
            _options = options;
            _http.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        }

        public async Task<IReadOnlyList<TraccarDeviceVm>> GetDevicesAsync(CancellationToken ct = default)
        {
            using var res = await _http.GetAsync("api/devices", ct);
            res.EnsureSuccessStatusCode();
            await using var stream = await res.Content.ReadAsStreamAsync(ct);
            var devices = await JsonSerializer.DeserializeAsync<List<DeviceDto>>(stream, _jsonOptions, ct) ?? new();
            return devices.Select(d => new TraccarDeviceVm
            {
                Id = d.id,
                Name = d.name ?? string.Empty,
                UniqueId = d.uniqueId ?? string.Empty
            }).ToList();
        }

        public async Task<IReadOnlyList<TraccarPositionVm>> GetLatestPositionsAsync(CancellationToken ct = default)
        {
            // Get positions
            using var res = await _http.GetAsync("api/positions", ct);
            res.EnsureSuccessStatusCode();
            await using var stream = await res.Content.ReadAsStreamAsync(ct);
            var positions = await JsonSerializer.DeserializeAsync<List<PositionDto>>(stream, _jsonOptions, ct) ?? new();

            // Map
            var positionVms = positions.Select(p => new TraccarPositionVm
            {
                Id = p.id,
                DeviceId = p.deviceId,
                Latitude = p.latitude,
                Longitude = p.longitude,
                Accuracy = p.accuracy,
                DeviceTime = p.deviceTime,
                Address = p.address,
                Protocol = p.protocol,
                Attributes = p.attributes
            }).ToList();

            // Attach device names
            var devices = await GetDevicesAsync(ct);
            var deviceMap = devices.ToDictionary(d => d.Id, d => d.Name);
            foreach (var vm in positionVms)
            {
                if (deviceMap.TryGetValue(vm.DeviceId, out var name))
                {
                    vm.DeviceName = name;
                }
            }

            return positionVms;
        }

        // Internal DTOs match typical Traccar JSON fields (camelCase)
        private record DeviceDto(long id, string? name, string? uniqueId);
        private record PositionDto
        {
            public long id { get; init; }
            public long deviceId { get; init; }
            public double latitude { get; init; }
            public double longitude { get; init; }
            public double? accuracy { get; init; }
            public DateTime deviceTime { get; init; }
            public string? address { get; init; }
            public string? protocol { get; init; }
            public Dictionary<string, object>? attributes { get; init; }
        }
    }
}
