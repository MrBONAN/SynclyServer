using SynclyServer.Dtos.Location;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class LocationMappers
{
    public static LocationDto ToLocationDto(this Location locationModel)
    {
        return new LocationDto()
        {
            Id = locationModel.Id,
            //UserId = locationModel.UserId!.Value,
            Latitude = locationModel.Latitude!.Value,
            Longitude = locationModel.Longitude!.Value,
            LastUpdate = locationModel.LastUpdate
        };
    }

    public static Location ToLocationFromUpdate(this UpdateLocationDto locationModel)
    {
        return new Location()
        {
            Latitude = locationModel.Latitude,
            Longitude = locationModel.Longitude
        };
    }
}