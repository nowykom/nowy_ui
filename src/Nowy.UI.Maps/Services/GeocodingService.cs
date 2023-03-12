using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Web;
using MudBlazor;
using Nowy.UI.Maps.Components;

namespace Nowy.UI.Maps.Services;

public class GeocodingService
{
    private readonly HttpClient _http;
    private readonly ISnackbar? _snackbar;

    public GeocodingService(HttpClient http, ISnackbar? snackbar)
    {
        this._http = http;
        this._snackbar = snackbar;
    }

    public async Task<NxLeafletLocation?> TryGeocodeForwardAsync(string? address)
    {
        address = address?.Trim();
        if (string.IsNullOrEmpty(address))
            return null;

        try
        {
            NxLeafletLocation? location = await this.GeocodeForwardAsync(address);
            return location;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            this._snackbar?.Add($"Die Geocoding-API ist nicht erreichbar.", Severity.Error);
        }

        return null;
    }

    [return: NotNullIfNotNull("location")]
    public async Task<NxLeafletResolvedLocation?> TryGeocodeReverseAsync(NxLeafletLocation? location)
    {
        if (location is null)
            return null;

        string? address = null;
        try
        {
            address = await this.GeocodeReverseAsync(lat: location.Lat, lng: location.Lng);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            this._snackbar?.Add($"Die Reverse-Geocoding-API ist nicht erreichbar.", Severity.Error);
        }

        return new NxLeafletResolvedLocation(Location: location, Address: address);
    }

    public async Task<NxLeafletLocation?> GeocodeForwardAsync(string address)
    {
        try
        {
            string? json = await this._http.GetStringAsync(
                $"https://www.mapquestapi.com/geocoding/v1/address?key=QcH0d7dhBp0m92DdQZsxzL1u2hOPnrXC&location={HttpUtility.UrlEncode(address)}");
            MapquestGeocodingResponse json_decoded = json.FromJson<MapquestGeocodingResponse>();

            MapquestGeocodingResponse._Result._Location._LatLng? latlng = json_decoded.Results?.FirstOrDefault()?.Locations?.FirstOrDefault()?.LatLng;
            if (latlng is { })
            {
                return new(Lat: latlng.Lat, Lng: latlng.Lng);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to do forward geocoding: {ex}");
            throw new GeocodingException("Failed to do forward geocoding", ex);
        }

        return null;
    }

    public async Task<string?> GeocodeReverseAsync(double lat, double lng)
    {
        try
        {
            string? json = await this._http.GetStringAsync(
                $"https://www.mapquestapi.com/geocoding/v1/reverse?key=QcH0d7dhBp0m92DdQZsxzL1u2hOPnrXC&location={FormattableString.Invariant($"{lat:0.000000000000},{lng:0.000000000000}&includeRoadMetadata=true&includeNearestIntersection=true")}");

            MapquestGeocodingReverseResponse json_decoded = json.FromJson<MapquestGeocodingReverseResponse>();

            Console.WriteLine($"json: {json}");
            Console.WriteLine($"json2: {json_decoded.ToJson()}");

            if (json_decoded.Results?.FirstOrDefault()?.Locations?.FirstOrDefault() is { } r)
            {
                return $"{MapsStringHelper.MoveHouseNumberToEnd(r.Street)}, {r.PostalCode} {r.AdminArea5}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to do reverse geocoding: {ex}");
            throw new GeocodingException("Failed to do reverse geocoding", ex);
        }

        return null;
    }
}

public sealed class GeocodingException : Exception
{
    public GeocodingException(string message, Exception ex)
        : base(message, ex)
    {
    }
}

public sealed class MapquestGeocodingResponse
{
    [JsonPropertyName("results")] public List<_Result> Results { get; init; } = new();

    public sealed class _Result
    {
        [JsonPropertyName("locations")] public List<_Location> Locations { get; init; } = new();

        public sealed class _Location
        {
            [JsonPropertyName("latLng")] public _LatLng LatLng { get; set; } = new();

            public sealed class _LatLng
            {
                [JsonPropertyName("lat")] public double Lat { get; set; } = 0;
                [JsonPropertyName("lng")] public double Lng { get; set; } = 0;
            }
        }
    }
}

public sealed class MapquestGeocodingReverseResponse
{
    [JsonPropertyName("results")] public List<_Result> Results { get; init; } = new();

    public sealed class _Result
    {
        [JsonPropertyName("locations")] public List<_Location> Locations { get; init; } = new();

        public sealed class _Location
        {
            [JsonPropertyName("street")] public string? Street { get; set; }
            [JsonPropertyName("postalCode")] public string? PostalCode { get; set; }
            [JsonPropertyName("adminArea5")] public string? AdminArea5 { get; set; }
            [JsonPropertyName("adminArea1")] public string? AdminArea1 { get; set; }


            [JsonPropertyName("latLng")] public _LatLng LatLng { get; set; } = new();

            public sealed class _LatLng
            {
                [JsonPropertyName("lat")] public double Lat { get; set; } = 0;
                [JsonPropertyName("lng")] public double Lng { get; set; } = 0;
            }
        }
    }
}
