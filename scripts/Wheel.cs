using Godot;
using System;

public partial class Wheel : RigidBody3D
{
	Ship ship;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PinJoint3D joint = GetNode<PinJoint3D>("../JointShipWheel");
		joint.GlobalPosition = GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
