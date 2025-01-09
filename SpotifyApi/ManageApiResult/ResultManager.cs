using RestSharp;

namespace SpotifyApi.ManageApiResult;

public static class ResultManager
{
    public static ApiResult ManageResponseResult<T>(RestResponse<T> response)
    {
        return ApiResult.Error;
    }
}