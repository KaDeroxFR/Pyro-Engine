using Sandbox.Engine;
using Sandbox.Modals;

namespace Sandbox;

public static partial class Game
{
        /// <summary>
        /// Provides static methods for displaying various modal overlays in the game UI.
        /// <para>
        /// The <see cref="Overlay"/> class allows you to open modals for packages, maps, news, organizations, reviews, friends lists, server lists, settings, input bindings, and player profiles.
        /// It serves as a central point for invoking user interface overlays that interact with core game and community features.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// // Show a modal for a specific game package
        /// Game.Overlay.ShowGameModal("facepunch.sandbox");
        ///
        /// // Check if any overlay is currently open
        /// if (Game.Overlay.IsOpen)
        /// {
        ///     // Pause game logic or input
        /// }
        /// </code>
        /// </example>
        public partial class Overlay
        {
                /// <summary>
                /// Opens a modal for the specified game package
                /// </summary>
                /// <param name="packageIdent"></param>
                public static void ShowGameModal( string packageIdent )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Game( packageIdent );
                }

                /// <summary>
                /// Opens a modal for the specified map package
                /// </summary>
                /// <param name="packageIdent"></param>
                public static void ShowMapModal( string packageIdent )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Map( packageIdent );
                }

                /// <summary>
                /// Opens a modal for the specified package
                /// </summary>
                /// <param name="ident"></param>
                public static void ShowPackageModal( string ident )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Package( ident, "" );
                }

                /// <summary>
                /// Opens a modal for the specified package on the specified page
                /// </summary>
                /// <param name="ident"></param>
                /// <param name="page"></param>
                public static void ShowPackageModal( string ident, string page )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Package( ident, page );
                }

                /// <summary>
                /// Opens a modal for the news item
                /// </summary>
                public static void ShowNewsModal( Sandbox.Services.News newsitem )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.News( newsitem );
                }

                /// <summary>
                /// Opens a modal for the specified organization. 
                /// This is most likely called from a Package - so get the organization from there.
                /// </summary>
                /// <param name="org"></param>
                public static void ShowOrganizationModal( Package.Organization org )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Organization( org );
                }

                /// <summary>
                /// Opens a modal to review the specified package
                /// </summary>
                /// <param name="package"></param>
                public static void ShowReviewModal( Package package )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Review( package );
                }

                /// <summary>
                /// Opens a modal to report the specified package
                /// </summary>
                public static void ShowReportModal( string packageIdent )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Report( packageIdent );
                }

                /// <summary>
                /// Opens a modal for selecting a package
                /// </summary>
                public static void ShowPackageSelector( string query, Action<Package> onSelect, Action<string> onFilterChanged = null )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.PackageSelect( query, onSelect, onFilterChanged );
                }

                /// <summary>
                /// Opens a modal for selecting a map.
                /// This can be either a map package from the workshop (an ident), or a scene from a mount (a path)
                /// </summary>
                public static void ShowMapSelector( Action<string> onSelect, string selected = null )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.MapSelect( onSelect, selected );
                }

                [Obsolete( "Use ShowFriendsList with FriendsListModalOptions instead." )]
                public static void ShowFriendsList() => ShowFriendsList( new() );

                /// <summary>
                /// Opens a modal that shows the user's friends list
                /// </summary>
                /// <param name="options"></param>
                public static void ShowFriendsList( in FriendsListModalOptions options )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.FriendsList( options );
                }

                /// <summary>
                /// Opens a modal that shows a list of active servers
                /// </summary>
                public static void ShowServerList( in ServerListConfig config )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.ServerList( config );
                }

                /// <summary>
                /// Opens the settings. In the menu that's a tab; in a game it goes up over the top.
                /// Optionally, you can specify a category to open directly to: "display", "graphics",
                /// "audio", "controls", "keybinds", "game", "storage", "developer", "about".
                /// The old page names "video" and "input" still work, and land on display and controls.
                /// </summary>
                public static void ShowSettingsModal( string page = "" )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Settings( page );
                }

                /// <summary>
                /// Shows modal to edit the streaming settings
                /// </summary>
                public static void ShowServiceConnector()
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.ServiceConnector();
                }

                /// <summary>
                /// Opens a modal that lets you view and rebind game input actions.
                /// </summary>
                public static void ShowBinds()
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Settings( "keybinds" );
                }

                /// <summary>
                /// Opens a modal to create a game with a bunch of settings. We use this in the menu when you click "Create Game"
                /// and the game has options.
                /// </summary>
                public static void CreateGame( in CreateGameOptions options )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.CreateGame( options );
                }

                /// <summary>
                /// View a selected user's profile
                /// </summary>
                public static void ShowPlayer( SteamId steamid, string page = "" )
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.Player( steamid, page );
                }

                /// <summary>
                /// Open a modal that shows a list of players currently in the game
                /// </summary>
                public static void ShowPlayerList()
                {
                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.PlayerList();
                }

                /// <summary>
                /// Open a modal that prompts the user to publish content to the workshop
                /// </summary>
                public static void WorkshopPublish( in WorkshopPublishOptions options )
                {
                        using var scope = GlobalContext.MenuScope();
                        IModalSystem.Current?.WorkshopPublish( options );
                }

                /// <summary>
                /// Starts a local benchmark run from within the game. The results panel is shown when all packages complete.
                /// </summary>
                /// <param name="packageFilter">Package names to include (null = all configured packages)</param>
                /// <param name="testFilter">Individual test names to run (null = all). Passed to packages via the benchmark.tests game setting.</param>
                public static void RunLocalBenchmarks( string[] packageFilter = null, string[] testFilter = null )
                {
                        BenchmarkOrchestrator.Run( packageFilter, testFilter );
                }

                /// <summary>
                /// Opens the benchmark results modal with the given batch ID and per-test summaries.
                /// </summary>
                public static void ShowBenchmarkResults( Guid batchId, IReadOnlyList<BenchmarkTestSummary> summaries )
                {
                        using var scope = GlobalContext.MenuScope();
                        IModalSystem.Current?.BenchmarkResults( batchId, summaries );
                }

                /// <summary>
                /// Opens the pause menu overlay. This is the same menu that appears when pressing Escape,
                /// or Shift+Escape in editor play mode.
                /// </summary>
                public static void ShowPauseMenu()
                {
                        if ( IModalSystem.Current?.IsModalOpen == true )
                                return;

                        using var scope = GlobalContext.MenuScope();

                        IModalSystem.Current?.PauseMenu();
                }

                /// <summary>
                /// Closes the top overlay if one exists
                /// </summary>
                public static void Close()
                {
                        if ( IModalSystem.Current?.IsModalOpen != true )
                                return;

                        IModalSystem.Current?.PauseMenu();
                }

                /// <summary>
                /// Close all open overlays
                /// </summary>
                /// <param name="immediate">If <see langword="true"/>, will skip any outros</param>
                public static void CloseAll( bool immediate = false )
                {
                        IModalSystem.Current?.CloseAll( immediate );
                }

                /// <summary>
                /// Returns true if any overlay is open
                /// </summary>
                public bool IsOpen => IModalSystem.Current?.IsModalOpen ?? false;

                /// <summary>
                /// Returns true if the pause menu overlay is open
                /// </summary>
                public bool IsPauseMenuOpen => IModalSystem.Current?.IsPauseMenuOpen ?? false;

                // ================================================================
                //  NATIVE STEAM OVERLAY API (PATCHED IN)
                //  ---------------------------------------------------------------
                //  These methods open the real Steam overlay — the same one that
                //  appears when you press Shift+Tab. They work in standalone games,
                //  unlike ShowFriendsList() which is a s&box modal.
                // ================================================================

                /// <summary>
                /// Opens the native Steam overlay on the specified dialog.
                /// Works in standalone games.
                /// </summary>
                /// <param name="dialog">
                /// Valid options: "friends", "community", "players", "settings",
                /// "officialgamegroup", "stats", "achievements"
                /// </param>
                public static void OpenSteamOverlay( string dialog = "friends" )
                {
                        Steamworks.SteamFriends.OpenOverlayInternal( dialog );
                }

                public static void OpenSteamFriends() => OpenSteamOverlay( "friends" );
                public static void OpenSteamCommunity() => OpenSteamOverlay( "community" );
                public static void OpenSteamRecentPlayers() => OpenSteamOverlay( "players" );
                public static void OpenSteamSettings() => OpenSteamOverlay( "settings" );
                public static void OpenSteamStats() => OpenSteamOverlay( "stats" );
                public static void OpenSteamAchievements() => OpenSteamOverlay( "achievements" );
                public static void OpenSteamOfficialGameGroup() => OpenSteamOverlay( "officialgamegroup" );

                /// <summary>
                /// Opens the native Steam overlay to a specific user dialog.
                /// Valid types: "steamid", "chat", "jointrade", "stats", "achievements",
                /// "friendadd", "friendremove", "friendrequestaccept", "friendrequestignore"
                /// </summary>
                public static void OpenSteamUserOverlay( SteamId steamId, string type )
                {
                        Steamworks.SteamFriends.OpenUserOverlayInternal( ToSteamworksId( steamId ), type );
                }

                public static void OpenSteamUserProfile( SteamId steamId ) => OpenSteamUserOverlay( steamId, "steamid" );
                public static void OpenSteamUserChat( SteamId steamId ) => OpenSteamUserOverlay( steamId, "chat" );
                public static void OpenSteamUserStats( SteamId steamId ) => OpenSteamUserOverlay( steamId, "stats" );
                public static void OpenSteamUserAchievements( SteamId steamId ) => OpenSteamUserOverlay( steamId, "achievements" );
                public static void OpenSteamUserTrade( SteamId steamId ) => OpenSteamUserOverlay( steamId, "jointrade" );
                public static void AddSteamFriend( SteamId steamId ) => OpenSteamUserOverlay( steamId, "friendadd" );
                public static void RemoveSteamFriend( SteamId steamId ) => OpenSteamUserOverlay( steamId, "friendremove" );
                public static void AcceptSteamFriendRequest( SteamId steamId ) => OpenSteamUserOverlay( steamId, "friendrequestaccept" );
                public static void IgnoreSteamFriendRequest( SteamId steamId ) => OpenSteamUserOverlay( steamId, "friendrequestignore" );

                /// <summary>
                /// Opens the native Steam overlay browser at the specified URL.
                /// </summary>
                public static void OpenSteamWebPage( string url, bool modal = false )
                {
                        var mode = modal
                                ? Steamworks.ActivateGameOverlayToWebPageMode.Modal
                                : Steamworks.ActivateGameOverlayToWebPageMode.Default;
                        Steamworks.SteamFriends.OpenWebPageInternal( url, mode );
                }

                /// <summary>
                /// Opens the native Steam overlay at the store page for the given AppID.
                /// flag: "none" (default), "cart", "cart_show"
                /// </summary>
                public static void OpenSteamStorePage( uint appId, string flag = "none" )
                {
                        var f = flag.ToLowerInvariant() switch
                        {
                                "cart"      => Steamworks.OverlayToStoreFlag.AddToCart,
                                "cart_show" => Steamworks.OverlayToStoreFlag.AddToCartAndShow,
                                _           => Steamworks.OverlayToStoreFlag.None,
                        };
                        Steamworks.SteamFriends.OpenStorePageInternal(
                                new Steamworks.AppId { Value = appId }, f );
                }

                // ------------------------------------------------------------
                //  INVITE DIALOGS (for multiplayer)
                // ------------------------------------------------------------

                /// <summary>
                /// Opens the native Steam "Invite Friends" dialog for the CURRENT lobby.
                /// Auto-detects the active lobby from Networking. Works in standalone games.
                /// </summary>
                public static void OpenSteamInviteDialog()
                {
                        var lid = GetCurrentLobbySteamId();
                        if ( lid == 0 )
                        {
                                Log.Warning( "OpenSteamInviteDialog: no active lobby. Call Networking.CreateLobby first." );
                                return;
                        }
                        Steamworks.SteamFriends.OpenInviteDialogInternal(
                                new Steamworks.SteamId { Value = lid } );
                }

                /// <summary>
                /// Opens the native Steam "Invite Friends" dialog for a specific lobby SteamId.
                /// </summary>
                public static void OpenSteamInviteDialog( SteamId lobbyId )
                {
                        Steamworks.SteamFriends.OpenInviteDialogInternal( ToSteamworksId( lobbyId ) );
                }

                /// <summary>
                /// Opens the native Steam invite dialog using a connect string (IP:port or custom).
                /// </summary>
                public static void OpenSteamInviteDialog( string connectString )
                {
                        Steamworks.SteamFriends.OpenInviteDialogConnectStringInternal( connectString );
                }

                public static void OpenSteamRemotePlayTogetherInvite( SteamId lobbyId )
                {
                        Steamworks.SteamFriends.OpenRemotePlayTogetherInviteInternal( ToSteamworksId( lobbyId ) );
                }

                // ------------------------------------------------------------
                //  Internal helpers
                // ------------------------------------------------------------

                /// <summary>
                /// Returns the SteamId (as ulong) of the lobby we're currently in, or 0 if none.
                /// </summary>
                private static ulong GetCurrentLobbySteamId()
                {
                        var sys = Networking.System;
                        if ( sys is null ) return 0;

                        foreach ( var socket in sys.Sockets )
                        {
                                if ( socket is Sandbox.Network.SteamLobbySocket lobbySocket )
                                {
                                        return lobbySocket.LobbySteamId;
                                }
                        }
                        return 0;
                }

                private static Steamworks.SteamId ToSteamworksId( SteamId sandboxId )
                {
                        return new Steamworks.SteamId { Value = (ulong)sandboxId };
                }

                // ------------------------------------------------------------
                //  STANDALONE LOBBY JOIN HANDLER
                //  ------------------------------------------------------------
                //  Problem: In standalone games, when a friend clicks "Join Game"
                //  from the Steam overlay, s&box's Sandbox.Menu calls
                //  IGameInstanceDll.Current.CloseGame() to close your game and
                //  return to the menu before joining. This makes standalone
                //  multiplayer impossible.
                //
                //  Fix: We install our own handler on OnGameLobbyJoinRequested
                //  that intercepts the callback FIRST and calls Networking.Connect
                //  directly, staying in the game. We also set a flag that the
                //  menu's handler checks to skip the CloseGame call.
                // ------------------------------------------------------------

                private static bool _standaloneJoinHandlerInstalled = false;

                /// <summary>
                /// Installs a handler that intercepts "Join Game" from Steam overlay
                /// in standalone mode and connects directly to the lobby via
                /// Sandbox.Networking, WITHOUT closing the game.
                ///
                /// Call this once at game startup (e.g. in your MainMenu.OnStart).
                /// </summary>
                public static void EnableStandaloneLobbyJoin()
                {
                        if ( _standaloneJoinHandlerInstalled ) return;
                        _standaloneJoinHandlerInstalled = true;

                        Steamworks.SteamFriends.OnGameLobbyJoinRequested += OnStandaloneLobbyJoinRequested;
                }

                private static async void OnStandaloneLobbyJoinRequested( SteamId lobbySteamId )
                {
                        Log.Info( $"[Standalone] Join requested for lobby {lobbySteamId}" );

                        // Tell Sandbox.Menu's handler to skip — we're handling it ourselves.
                        SuppressMenuJoinHandler = true;

                        try
                        {
                                // Connect via Sandbox.Networking (uses SteamNetworkingSockets under the hood)
                                var ok = await Networking.TryConnectSteamId( lobbySteamId );
                                Log.Info( $"[Standalone] Join result: {ok}" );
                        }
                        catch ( Exception e )
                        {
                                Log.Warning( $"[Standalone] Join failed: {e.Message}" );
                        }
                        finally
                        {
                                // Re-enable the menu handler after a short delay
                                // (in case the join legitimately fails and user needs to retry)
                                _ = Task.Delay( 3000 ).ContinueWith( _ => SuppressMenuJoinHandler = false );
                        }
                }

                /// <summary>
                /// When true, the patched SteamCallbacks in Sandbox.Menu will skip
                /// the CloseGame+TryJoinLobby flow. Set by OnStandaloneLobbyJoinRequested.
                /// </summary>
                public static bool SuppressMenuJoinHandler { get; private set; } = false;
        }
}

