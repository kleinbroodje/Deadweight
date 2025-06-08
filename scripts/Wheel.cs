using Godot;
using System;

public partial class Wheel : StaticBody3D, IDescription
{
	// custom physics (better than godot physics)
	float angularAcc = 0f;
	float angularVel = 0f;
	float maxAngularVel = 1f;
	CharacterBody3D player;
	RigidBody3D ship;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<CharacterBody3D>("../../Player");
		ship = GetNode<RigidBody3D>("../../Ship");
	}

	public string Description()
	{
		return "<Q> and <E> to steer";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// key presses
		if (Input.IsActionPressed("steer_left") || Input.IsActionPressed("steer_right"))
		{
			var ray = (player as Player).GetRaycast();
			if (ray != null && ray.Name == Name)
			{
				// get the local Z axis of the wheel
				if (Input.IsActionPressed("steer_left"))
				{
					Steer(1);
				}
				else if (Input.IsActionPressed("steer_right"))
				{
					Steer(-1);
				}
			}
		}
		else
		{
			Decelerate();
		}

		// physics
		angularVel += angularAcc;
		if (angularVel >= 0)
		{
			// left
			angularVel = Mathf.Min(angularVel, maxAngularVel);
		}
		else
		{
			// right
			angularVel = Mathf.Max(angularVel, -maxAngularVel);
		}
		if (Mathf.Abs(angularVel) <= 0.02)
		{
			angularVel = 0;
		}
		Rotation += new Vector3(0, 0, Mathf.DegToRad(angularVel));

		Vector3 torque = new Vector3(0, 30 * angularVel, 0);
		ship.ApplyTorque(torque);
	}

	public void Steer(int direc)
	{
		angularAcc = 0.03f * direc;
	}

	public void Decelerate()
	{
		angularAcc = -0.03f * Mathf.Sign(angularVel);
	}
}
