namespace Editor;


class LogoWidget : Widget
{
	public LogoWidget( Widget parent ) : base( parent )
	{
	}

	protected override Vector2 SizeHint()
	{
		return new Vector2( 150, 40 );
	}

	protected override void OnPaint()
	{
		Paint.ClearPen();
		Paint.ClearBrush();

		var logoRect = LocalRect.Align( 32, TextFlag.LeftCenter );
		Paint.Draw( logoRect, "logo_rounded.png" );

		var textRect = LocalRect.Shrink( 42, 0, 0, 0 );
		Paint.SetHeadingFont( 18, 700 );
		Paint.Pen = Theme.Text;
		Paint.DrawText( textRect, "Engine", TextFlag.LeftCenter );
	}
}
