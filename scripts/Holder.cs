using Godot;
using System;

public partial class Holder : Node3D
{
	float followSpeed = 0.5f * 165;
	Camera3D camera;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
