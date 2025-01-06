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
            UserId = locationModel.UserId,
            Latitude = locationModel.Latitude!.Value,
            Longitude = locationModel.Longitude!.Value,
            LastUpdate = locationModel.LastUpdate
        };
    }
}