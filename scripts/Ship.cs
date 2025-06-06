using Godot;
using System;

public partial class Ship : RigidBody3D
{
	float floatForce = 3.0f;
	float waterHeight = 1.0f;
	float waterDrag = 0.05f;
	float waterAngularDrag = 0.05f;
	float gravity;

	MeshInstance3D customOcean;
	Godot.Collections.Array<Node> probes;

	bool submerged = false;

	public override void _Ready()
	{
		gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
		customOcean = GetNode<MeshInstance3D>("../CustomOcean");
		probes = GetNode<Node3D>("BuoyancyContainer").GetChildren();
		Mass = 1;
	}

	public override void _PhysicsProcess(double delta)
	{
		submerged = false;
		foreach (Node3D p in probes)
		{
			float depth = customOcean.Call("getHeight", p.GlobalPosition).As<float>() - p.GlobalPosition.Y;
			if (depth > 0)
			{
				submerged = true;
				ApplyForce(Vector3.Up * floatForce * gravity * depth, p.GlobalPosition - GlobalPosition);
			}
		}
	}

	public override void _IntegrateForces(PhysicsDirectBodyState3D state)
	{
		if (submerged)
		{
			state.LinearVelocity *= 1 - waterDrag;
			state.AngularVelocity *= 1 - waterAngularDrag;
		}

	}
}
