using Godot;
using System;

public partial class InfoText : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Rename(StringName name)
	{
		Text = name;
		GlobalPosition = new Vector2(GetViewport().GetVisibleRect().Size.X / 2 - Size.X * Scale.X / 2, GetViewport().GetVisibleRect().Size.Y * (5.0f / 7.0f));
	}
}
