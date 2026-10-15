using System.IO;
using System;

namespace Editor;

[Dock( "Editor", "Git Changes", "source_control", DockArea.Bottom )]
public sealed class GitChangesDock : Widget
{
	readonly Label branchLabel;
	readonly Label summaryLabel;
	readonly LineEdit commitMessage;
	readonly ListView changes;
	readonly TextEdit diff;

	public GitChangesDock( Widget parent ) : base( parent )
	{
		MinimumSize = 240;
		Layout = Layout.Column();

		var toolbar = Layout.AddRow();
		toolbar.Spacing = 4;
		toolbar.Margin = 4;
		toolbar.Add( new Button( "Refresh", this ) { Clicked = Refresh } );
		toolbar.Add( new Button( "Pull", this ) { Clicked = GitIntegration.Pull } );
		toolbar.Add( new Button( "Push", this ) { Clicked = GitIntegration.Push } );
		toolbar.AddStretchCell();

		branchLabel = toolbar.Add( new Label( "Git" ) );

		var body = Layout.AddRow( 1 );
		changes = new ListView( this ) { ItemSize = new Vector2( 0, 28 ), ItemSpacing = 1 };
		body.Add( changes, 1 );
		changes.ItemPaint = PaintChange;
		changes.ItemActivated = item => ShowDiff( (GitChange)item );
		changes.ItemClicked = item => ShowDiff( (GitChange)item );

		var right = body.AddColumn( 1 );
		diff = right.Add( new TextEdit(), 1 );
		diff.Editable = false;

		var commitRow = right.AddRow();
		commitRow.Spacing = 4;
		commitRow.Margin = 4;
		commitMessage = commitRow.Add( new LineEdit(), 1 );
		commitMessage.PlaceholderText = "Commit message";
		commitRow.Add( new Button( "Commit all", this ) { Clicked = Commit } );
		commitRow.Add( new Button( "Stage all", this ) { Clicked = StageAll } );

		summaryLabel = Layout.Add( new Label( "" ) );
		summaryLabel.Margin = 4;

		Refresh();
	}

	void Refresh()
	{
		_ = RefreshAsync();
	}

	async Task RefreshAsync()
	{
		var status = await GitIntegration.ExecuteAsync( "status", "--short", "--branch" );
		if ( !status.Success )
		{
			branchLabel.Text = "Git unavailable";
			summaryLabel.Text = status.Error;
			changes.Clear();
			return;
		}

		var lines = status.Output.Split( '\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
		branchLabel.Text = lines.FirstOrDefault()?.Trim() ?? "No branch";

		var items = lines.Skip( 1 ).Select( ParseChange ).Where( x => x is not null ).ToList();
		changes.SetItems( items );
		summaryLabel.Text = $"{items.Count} changed file(s)";
	}

	static GitChange ParseChange( string line )
	{
		if ( line.Length < 4 ) return null;
		var status = line[..2].Trim();
		var path = line[3..].Trim();
		return new GitChange( status, path );
	}

	void PaintChange( VirtualWidget item )
	{
		var change = (GitChange)item.Object;
		var color = change.Status.Contains( 'A' ) ? Color.Green : change.Status.Contains( 'D' ) ? Color.Red : Theme.Text;
		Paint.ClearPen();
		Paint.SetBrush( item.Selected ? Theme.Blue.WithAlpha( 0.3f ) : Theme.ControlBackground );
		Paint.DrawRect( item.Rect.Shrink( 1 ), 3 );
		Paint.SetPen( color );
		Paint.DrawText( item.Rect.Shrink( 8, 0 ), $"{change.Status,-2}  {change.Path}", TextFlag.LeftCenter );
	}

	async void ShowDiff( GitChange change )
	{
		var result = await GitIntegration.ExecuteAsync( "diff", "--", change.Path );
		diff.PlainText = result.Success && !string.IsNullOrWhiteSpace( result.Output ) ? result.Output : result.Error;
	}

	void StageAll()
	{
		_ = GitIntegration.ExecuteAsync( "add", "--all" ).ContinueWith( _ => Refresh() );
	}

	void Commit()
	{
		var message = commitMessage.Text?.Trim();
		if ( string.IsNullOrWhiteSpace( message ) )
		{
			Log.Warning( "[Git] Enter a commit message first." );
			return;
		}

		_ = CommitAsync( message );
	}

	async Task CommitAsync( string message )
	{
		var result = await GitIntegration.ExecuteAsync( "add", "--all" );
		if ( !result.Success ) return;
		await GitIntegration.ExecuteAsync( "commit", "-m", message );
		commitMessage.Text = "";
		Refresh();
	}

	sealed record GitChange( string Status, string Path );
}
