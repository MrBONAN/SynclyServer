using dotenv.net;
using Infrastructure.API.SpotifyAPI.Models;
using RestSharp;
using Infrastructure.API.SpotifyAPI.SearchQuestionCreator;

namespace Infrastructure.API.SpotifyAPI;

public static partial class SpotifyApi
{
    public static string ClientId { get; private set; }
    private static string ClientSecret { get; set; }
    private static RestClient SpotifyClient = new RestClient("https://api.spotify.com/v1/");

    static SpotifyApi()
    {
        LoadEnvFiles();
    }

    private static void LoadEnvFiles()
    {
        LoadEnvFile("../SpotifyApi/Spotify.env");
        ClientId = Environment.GetEnvironmentVariable("CLIENT_ID") ??
                   throw new FileLoadException("App client id was not found");
        ClientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") ??
                       throw new FileLoadException("App client secret was not found");
    }
    
    public static void LoadEnvFile(string filePath)
    {
        var path = Directory.GetCurrentDirectory();
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл не найден: {filePath}");
        }

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex < 0)
            {
                Console.WriteLine($"Некорректная строка в .env файле: {line}");
                continue;
            }

            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static async Task<AccessToken?> GetAppAccessToken()
    {
        var client = new RestClient("https://accounts.spotify.com/api/token");
        var request = new RestRequest()
            .AddHeader("Content-Type", "application/x-www-form-urlencoded")
            .AddParameter("grant_type", "client_credentials")
            .AddParameter("client_id", ClientId)
            .AddParameter("client_secret", ClientSecret);

        var accessToken = (await client.ExecutePostAsync<AccessToken>(request)).Data;
        return accessToken;
    }

    public static IAddAccessTokenStage SearchFor()
    {
        return new SearchQuestionCreator.SearchQuestionCreator();
    }
}