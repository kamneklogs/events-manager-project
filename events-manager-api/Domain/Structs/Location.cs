namespace events_manager_api.Domain.Structs;

public struct Location
{
    public double latitude, longitude;

    public Location(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public override string ToString()
    {
        return $"Latitude: {latitude}, Longitude: {longitude}";
    }
}