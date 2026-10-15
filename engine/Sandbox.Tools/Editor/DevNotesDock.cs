using System;
using System.IO;
using System.Text;

namespace Editor;

[Dock( "Editor", "Dev Notes", "note", DockArea.Bottom )]
public sealed class DevNotesDock : Widget
{
	readonly ListView notes;
	readonly LineEdit fileName;
	readonly TextEdit content;
	readonly Label summary;

	DevNote selected;

	public DevNotesDock( Widget parent ) : base( parent )
	{
		MinimumSize = 240;
		Layout = Layout.Column();

		var toolbar = Layout.AddRow();
		toolbar.Spacing = 4;
		toolbar.Margin = 4;
		toolbar.Add( new Button( "New Note", this ) { Clicked = () => CreateNote( "note" ) } );
		toolbar.Add( new Button( "New TODO", this ) { Clicked = () => CreateNote( "todo" ) } );
		toolbar.Add( new Button( "Refresh", this ) { Clicked = Refresh } );

		var body = Layout.AddRow( 1 );
		notes = body.Add( new ListView( this ), 1 );
		notes.ItemSize = new Vector2( 0, 30 );
		notes.ItemSpacing = 1;
		notes.ItemPaint = PaintNote;
		notes.ItemActivated = item => OpenNote( (DevNote)item );
		notes.ItemClicked = item => OpenNote( (DevNote)item );

		var editor = body.AddColumn( 2 );
		var titleRow = editor.AddRow();
		titleRow.Spacing = 4;
		titleRow.Margin = 4;
		fileName = titleRow.Add( new LineEdit(), 1 );
		fileName.PlaceholderText = "Note file name";
		titleRow.Add( new Button( "Save", this ) { Clicked = SaveNote } );
		titleRow.Add( new Button( "Delete", this ) { Clicked = DeleteNote } );

		content = editor.Add( new TextEdit(), 1 );
		content.PlaceholderText = "Write a note in Markdown...";

		summary = Layout.Add( new Label( "" ) );
		summary.Margin = 4;

		Refresh();
	}

	void Refresh()
	{
		var folder = GetNotesFolder();
		if ( folder is null ) return;

		try
		{
			Directory.CreateDirectory( folder );
			var items = Directory.EnumerateFiles( folder, "*.md", SearchOption.TopDirectoryOnly )
				.OrderBy( Path.GetFileName, StringComparer.OrdinalIgnoreCase )
				.Select( path => new DevNote( path ) )
				.ToList();

			notes.SetItems( items );
			summary.Text = $"{items.Count} note(s) in .devnotes";

			if ( selected is not null && items.Any( x => string.Equals( x.Path, selected.Path, StringComparison.OrdinalIgnoreCase ) ) )
				OpenNote( items.First( x => string.Equals( x.Path, selected.Path, StringComparison.OrdinalIgnoreCase ) ) );
			else if ( items.Count == 0 )
				ClearEditor();
		}
		catch ( IOException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Failed to read '{folder}'." );
			summary.Text = "Unable to read .devnotes.";
		}
		catch ( UnauthorizedAccessException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Access denied for '{folder}'." );
			summary.Text = "Access denied for .devnotes.";
		}
	}

	void CreateNote( string kind )
	{
		var folder = GetNotesFolder();
		if ( folder is null ) return;

		try
		{
			Directory.CreateDirectory( folder );
			var prefix = kind == "todo" ? "TODO" : "note";
			var path = Path.Combine( folder, $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}.md" );
			var text = kind == "todo"
				? "# TODO\n\n- [ ] \n"
				: "# Developer note\n\n";
			File.WriteAllText( path, text, Encoding.UTF8 );
			Refresh();
			OpenNote( new DevNote( path ) );
		}
		catch ( IOException exception )
		{
			Log.Warning( exception, "[Dev Notes] Failed to create a note." );
			summary.Text = "Unable to create note.";
		}
	}

	void OpenNote( DevNote note )
	{
		try
		{
			selected = note;
			fileName.Text = Path.GetFileNameWithoutExtension( note.Path );
			content.PlainText = File.ReadAllText( note.Path );
			summary.Text = note.Path;
		}
		catch ( IOException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Failed to open '{note.Path}'." );
			summary.Text = "Unable to open note.";
		}
		catch ( UnauthorizedAccessException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Access denied for '{note.Path}'." );
			summary.Text = "Access denied for note.";
		}
	}

	void SaveNote()
	{
		if ( selected is null )
		{
			summary.Text = "Select or create a note first.";
			return;
		}

		var name = SanitizeFileName( fileName.Text );
		if ( string.IsNullOrWhiteSpace( name ) )
		{
			summary.Text = "Enter a valid note name.";
			return;
		}

		var folder = GetNotesFolder();
		if ( folder is null ) return;

		var target = Path.Combine( folder, $"{name}.md" );
		try
		{
			File.WriteAllText( target, content.PlainText ?? "", Encoding.UTF8 );
			selected = new DevNote( target );
			Refresh();
			summary.Text = $"Saved {target}";
		}
		catch ( IOException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Failed to save '{target}'." );
			summary.Text = "Unable to save note.";
		}
	}

	void DeleteNote()
	{
		if ( selected is null ) return;

		try
		{
			File.Delete( selected.Path );
			selected = null;
			ClearEditor();
			Refresh();
		}
		catch ( IOException exception )
		{
			Log.Warning( exception, $"[Dev Notes] Failed to delete '{selected.Path}'." );
			summary.Text = "Unable to delete note.";
		}
	}

	void ClearEditor()
	{
		selected = null;
		fileName.Text = "";
		content.Clear();
		summary.Text = "Create a note in .devnotes.";
	}

	void PaintNote( VirtualWidget item )
	{
		var note = (DevNote)item.Object;
		Paint.ClearPen();
		Paint.SetBrush( item.Selected ? Theme.Blue.WithAlpha( 0.3f ) : Theme.ControlBackground );
		Paint.DrawRect( item.Rect.Shrink( 1 ), 3 );
		Paint.SetPen( Theme.Text );
		Paint.DrawText( item.Rect.Shrink( 8 ), Path.GetFileNameWithoutExtension( note.Path ), TextFlag.LeftCenter );
	}

	static string GetNotesFolder()
	{
		var root = Project.Current?.GetRootPath();
		if ( string.IsNullOrWhiteSpace( root ) || !Directory.Exists( root ) )
			return null;

		return Path.Combine( root, ".devnotes" );
	}

	static string SanitizeFileName( string name )
	{
		if ( string.IsNullOrWhiteSpace( name ) ) return "";

		var invalid = Path.GetInvalidFileNameChars();
		var builder = new StringBuilder( name.Trim() );
		foreach ( var character in invalid )
			builder.Replace( character, '-' );

		return builder.ToString().Trim( '.', ' ' );
	}

	sealed record DevNote( string Path );
}
