using System.Diagnostics;
using System;
using System.IO;
using System.Threading;

namespace Editor;

internal static class GitIntegration
{
	internal sealed record GitResult( int ExitCode, string Output, string Error )
	{
		public bool Success => ExitCode == 0;
	}

	static int _operationInProgress;

	public static bool HasRepository
	{
		get
		{
			var root = Project.Current?.GetRootPath();
			if ( string.IsNullOrWhiteSpace( root ) ) return false;

			return Directory.Exists( Path.Combine( root, ".git" ) ) ||
				File.Exists( Path.Combine( root, ".git" ) );
		}
	}

	public static void RefreshStatus()
	{
		_ = RunAsync( "status", "--short", "--branch" );
	}

	internal static Task<GitResult> ExecuteAsync( params string[] arguments ) => ExecuteCoreAsync( arguments );

	public static void Pull()
	{
		ConfirmRepositoryOperation( "Pull changes from the remote repository?", () => _ = RunAsync( "pull", "--ff-only" ) );
	}

	public static void CommitAll()
	{
		Dialog.AskString( CommitAllWithMessage, "Commit all current project changes with message:", "Commit", "Cancel", "", "Git Commit", 1 );
	}

	static void CommitAllWithMessage( string message )
	{
		Dialog.AskConfirm(
			() => _ = CommitAllAsync( message.Trim() ),
			"This stages and commits all changes in the project repository.",
			"Commit all changes?",
			"Commit",
			"Cancel" );
	}

	static async Task CommitAllAsync( string message )
	{
		if ( !await RunAsync( "add", "--all" ) ) return;
		await RunAsync( "commit", "-m", message );
	}

	public static void Push()
	{
		ConfirmRepositoryOperation( "Push local commits to the remote repository?", () => _ = RunAsync( "push" ) );
	}

	public static void OpenRepository()
	{
		var root = Project.Current?.GetRootPath();
		if ( string.IsNullOrWhiteSpace( root ) ) return;

		EditorUtility.OpenFolder( root );
	}

	static void ConfirmRepositoryOperation( string question, Action operation )
	{
		if ( !HasRepository )
		{
			Log.Warning( "[Git] The current project is not a Git repository." );
			return;
		}

		if ( Volatile.Read( ref _operationInProgress ) != 0 )
		{
			Log.Warning( "[Git] Another Git operation is already in progress." );
			return;
		}

		Dialog.AskConfirm(
			operation,
			question,
			"Git",
			"Continue",
			"Cancel" );
	}

	static async Task<bool> RunAsync( params string[] arguments )
	{
		var result = await ExecuteCoreAsync( arguments );
		return result.Success;
	}

	static async Task<GitResult> ExecuteCoreAsync( params string[] arguments )
	{
		if ( Interlocked.Exchange( ref _operationInProgress, 1 ) != 0 )
		{
			Log.Warning( "[Git] Another Git operation is already in progress." );
			return new GitResult( -1, "", "Another Git operation is already in progress." );
		}

		try
		{
			var root = Project.Current?.GetRootPath();
			if ( string.IsNullOrWhiteSpace( root ) )
			{
				Log.Warning( "[Git] No project is currently open." );
				return new GitResult( -1, "", "No project is currently open." );
			}

			var info = new ProcessStartInfo
			{
				FileName = "git",
				WorkingDirectory = root,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			foreach ( var argument in arguments )
				info.ArgumentList.Add( argument );

			info.Environment["GIT_TERMINAL_PROMPT"] = "0";

			Log.Info( $"[Git] git {string.Join( " ", arguments )}" );

			using var process = Process.Start( info );
			if ( process is null )
			{
				Log.Error( "[Git] Failed to start git." );
				return new GitResult( -1, "", "Failed to start git." );
			}

			var outputTask = process.StandardOutput.ReadToEndAsync();
			var errorTask = process.StandardError.ReadToEndAsync();
			await process.WaitForExitAsync();

			var output = await outputTask;
			var error = await errorTask;

			LogLines( output, false );
			LogLines( error, true );

			if ( process.ExitCode != 0 )
			{
				Log.Error( $"[Git] Command failed with exit code {process.ExitCode}." );
				return new GitResult( process.ExitCode, output, error );
			}

			Log.Info( "[Git] Command completed successfully." );
			return new GitResult( process.ExitCode, output, error );
		}
		catch ( Exception exception )
		{
			Log.Error( exception, "[Git] Command failed." );
			return new GitResult( -1, "", exception.Message );
		}
		finally
		{
			Volatile.Write( ref _operationInProgress, 0 );
		}
	}

	static void LogLines( string text, bool error )
	{
		foreach ( var line in text.Split( '\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ) )
		{
			if ( error ) Log.Warning( $"[Git] {line}" );
			else Log.Info( $"[Git] {line}" );
		}
	}
}
