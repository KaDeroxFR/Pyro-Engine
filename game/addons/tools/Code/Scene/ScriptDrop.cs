namespace Editor;

static class ScriptDrop
{
	public static List<TypeDescription> FindComponentTypes( string absolutePath )
	{
		var target = System.IO.Path.GetFullPath( absolutePath );

		return EditorTypeLibrary.GetTypes<Component>()
			.Where( t => !t.IsAbstract && !t.IsGenericType && t.SourceFile is not null )
			.Where( t => string.Equals( Resolve( t.SourceFile ), target, System.StringComparison.OrdinalIgnoreCase ) )
			.ToList();
	}

	static string Resolve( string source )
	{
		if ( System.IO.Path.IsPathRooted( source ) ) return System.IO.Path.GetFullPath( source );

		foreach ( var p in EditorUtility.Projects.GetAll() )
		{
			if ( !p.Active || !p.HasCompiler ) continue;
			var full = System.IO.Path.GetFullPath( System.IO.Path.Combine( p.GetCodePath(), source ) );
			if ( System.IO.File.Exists( full ) ) return full;
		}
		return null;
	}
}
