using System.Threading.Tasks;

namespace Sandbox;

public static class Launcher
{
	public static int Main()
	{
		var appSystem = new GameAppSystem();
		appSystem.Run();

		return 0;
	}
}

public class GameAppSystem : AppSystem
{
	public override void Init()
	{
		LoadSteamDll();
		TestSystemRequirements();

		base.Init();

		CreateGame();
		CreateMenu();

		var createInfo = new AppSystemCreateInfo()
		{
			WindowTitle = "Pyro",
			Flags = AppSystemFlags.IsGameApp
		};

		InitGame( createInfo );
	}
}
