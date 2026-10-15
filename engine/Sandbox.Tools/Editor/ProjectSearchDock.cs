using System.IO;
using System.Threading;
using System;

namespace Editor;

[Dock( "Editor", "Project Search", "search", DockArea.Bottom )]
public sealed class ProjectSearchDock : Widget
{
	readonly LineEdit query;
	readonly ListView results;
	readonly Label summary;
	CancellationTokenSource searchCancellation;

	public ProjectSearchDock( Widget parent ) : base( parent )
	{
		MinimumSize = 240;
		Layout = Layout.Column();

		var toolbar = Layout.AddRow();
		toolbar.Spacing = 4;
		toolbar.Margin = 4;
		query = toolbar.Add( new LineEdit(), 1 );
		query.PlaceholderText = "Search in project files...";
		query.TextEdited += _ => Search();
		query.ReturnPressed += Search;
		toolbar.Add( new Button( "Search", this ) { Clicked = Search } );

		results = Layout.Add( new ListView( this ), 1 );
		results.ItemSize = new Vector2( 0, 32 );
		results.ItemSpacing = 1;
		results.ItemPaint = PaintResult;
		results.ItemActivated = item => OpenResult( (SearchResult)item );
		results.ItemClicked = item => OpenResult( (SearchResult)item );

		summary = Layout.Add( new Label( "Enter text to search." ) );
		summary.Margin = 4;
	}

	void Search()
	{
		searchCancellation?.Cancel();
		searchCancellation = new CancellationTokenSource();
		_ = SearchAsync( query.Text?.Trim() ?? "", searchCancellation.Token );
	}

	async Task SearchAsync( string text, CancellationToken token )
	{
		MainThread.Queue( () =>
		{
			if ( !token.IsCancellationRequested && results.IsValid )
				results.Clear();
		} );

		if ( string.IsNullOrWhiteSpace( text ) )
		{
			MainThread.Queue( () => summary.Text = "Enter text to search." );
			return;
		}

		var root = Project.Current?.GetRootPath();
		if ( string.IsNullOrWhiteSpace( root ) || !Directory.Exists( root ) )
		{
			MainThread.Queue( () => summary.Text = "No project is open." );
			return;
		}

		try
		{
			var found = await Task.Run( () => FindMatches( root, text, token ) );
			if ( token.IsCancellationRequested ) return;

			MainThread.Queue( () =>
			{
				if ( token.IsCancellationRequested || !results.IsValid ) return;

				results.SetItems( found );
				summary.Text = $"{found.Count} result(s)";
			} );
		}
		catch ( Exception exception )
		{
			Log.Warning( exception, $"[Search] Failed to search '{root}'." );
			MainThread.Queue( () =>
			{
				if ( !token.IsCancellationRequested && summary.IsValid )
					summary.Text = "Search failed. See the console for details.";
			} );
		}
	}

	static List<SearchResult> FindMatches( string root, string text, CancellationToken token )
	{
		var found = new List<SearchResult>();
		var ignored = new[] { ".git", ".vs", "bin", "obj", ".sbox" };

		IEnumerable<string> files;
		try
		{
			files = Directory.EnumerateFiles( root, "*", SearchOption.AllDirectories );
		}
		catch ( IOException )
		{
			return found;
		}
		catch ( UnauthorizedAccessException )
		{
			return found;
		}

		foreach ( var file in files )
		{
			if ( token.IsCancellationRequested ) break;
			if ( ignored.Any( x => file.Split( Path.DirectorySeparatorChar ).Any( part => string.Equals( part, x, StringComparison.OrdinalIgnoreCase ) ) ) ) continue;

			try
			{
				if ( new FileInfo( file ).Length > 2 * 1024 * 1024 ) continue;
				var lineNumber = 0;
				foreach ( var line in File.ReadLines( file ) )
				{
					lineNumber++;
					if ( line.Contains( text, StringComparison.OrdinalIgnoreCase ) )
					{
						found.Add( new SearchResult( file, lineNumber, line.Trim() ) );
						if ( found.Count >= 2000 ) return found;
					}
				}
			}
			catch ( IOException ) { }
			catch ( UnauthorizedAccessException ) { }
		}

		return found;
	}

	void PaintResult( VirtualWidget item )
	{
		var result = (SearchResult)item.Object;
		Paint.ClearPen();
		Paint.SetBrush( item.Selected ? Theme.Blue.WithAlpha( 0.3f ) : Theme.ControlBackground );
		Paint.DrawRect( item.Rect.Shrink( 1 ), 3 );
		Paint.SetPen( Theme.Text );
		Paint.DrawText( item.Rect.Shrink( 8 ), $"{Path.GetFileName(result.File)}:{result.Line}  {result.Text}", TextFlag.LeftCenter );
	}

	static void OpenResult( SearchResult result )
	{
		CodeEditor.OpenFile( result.File, result.Line );
	}

	sealed record SearchResult( string File, int Line, string Text );
}
