using System.Text.Json.Serialization;
using SpotifyApi.Models.Authorization;

namespace App.UserAuthorization.SpotifyAuthorization.Models;

public record AuthorizationResponse(AuthorizationResult Result, string? Code);

public enum AuthorizationResult
{
    Success,
    Error,
    Canceled
}

public record AuthorizationPkceResponse(
    AuthorizationResult Result,
    string? Code,
    string? CodeVerifier,
    string? ErrorInfo)
{
}

public enum PkceAccessTokenResult
{
    Success,
    ExchangeError,
    RefreshError
}

public class PkceAccessToken
{
    [JsonPropertyName("access_token")] public string? AccessToken { get; set; } = null;
    [JsonPropertyName("token_type")] public string? TokenType { get; set; } = null;
    [JsonPropertyName("expires_in")] public int? ExpiresIn { get; set; } = null;
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; } = null;

    public PkceAccessTokenResult Result { get; set; } = PkceAccessTokenResult.Success;
    [JsonInclude] public DateTime CreationDate { get; } = DateTime.UtcNow;

    [JsonConstructor]
    public PkceAccessToken(
        string? accessToken,
        string? tokenType,
        int? expiresIn,
        string? refreshToken,
        PkceAccessTokenResult result)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        RefreshToken = refreshToken;
        Result = result;
        
    }

    public PkceAccessToken()
    {
    }

    public SpotifyAccessTokenDto ConvertToAccessTokenDto()
    {
        var expiresDate = CreationDate.AddSeconds(ExpiresIn!.Value);
        return new SpotifyAccessTokenDto()
        {
            AccessToken = AccessToken!,
            TokenType = TokenType!,
            ExpiresDate = expiresDate,
            RefreshToken = RefreshToken!
        };
    }
}