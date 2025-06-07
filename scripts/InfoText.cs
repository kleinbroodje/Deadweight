using Godot;
using System;

public partial class InfoText : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Recenter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Rename(StringName name)
	{
		Text = name;
		Vector2 size = GetMinimumSize();
		Position = new Vector2(DisplayServer.WindowGetSize().X / 2 - size.X / 2, DisplayServer.WindowGetSize().Y * (5.0f / 7.0f));
	}
}
