using System.Net;
using RestSharp;

namespace SpotifyApi.ManageApiResult;

public static class ResultManager
{
    public static ApiResult ManageResponseResult<T>(RestResponse<T> response)
    {
        if (response.Data is null)
            return ApiResult.DataIsNull;
        if ((int)response.StatusCode is >= 200 and < 300)
            return ApiResult.Ok;
        return response.StatusCode switch
        {
            HttpStatusCode.Forbidden => ApiResult.Forbidden,
            HttpStatusCode.BadRequest => ApiResult.BadRequest,
            HttpStatusCode.NotFound => ApiResult.NotFound,
            _ => ApiResult.ResponseError
        };
    }
}