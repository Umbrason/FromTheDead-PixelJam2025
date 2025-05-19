using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public static class Authentication
{
    public static bool Initialized { get; private set; }
    public static async Task ChangePlayerName(string playerName)
    {
        await Initialize();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
    }

    public static async Task<string> GetPlayerName()
    {
        await Initialize();
        return (AuthenticationService.Instance.PlayerName??"_____")[..^5];
    }

    public static async Task Initialize()
    {
        if(Initialized) return;
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Initialized = true;
    }
}