using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// constants
	public const float walkSpeed = 5.0f;
	public const float jumpHeight = 7.0f;
	public const float floatForce = 1.0f;
	public const float waterHeight = 1.0f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public const float sensAt1280 = 0.0012f;
	public float resMult = (float)ProjectSettings.GetSetting("display/window/size/viewport_width") / 1280.0f;
	public bool holding = false;
	public float sens;

	// nodes
	Camera3D camera;
	RayCast3D raycast;
	Node3D holder;
	Node3D heldObject;
	Label infoText;

	// methods
	public override void _Ready()
	{
		sens = sensAt1280 / resMult;
		FloorMaxAngle = Mathf.DegToRad(60);
		camera = GetNode<Camera3D>("Camera");
		raycast = GetNode<RayCast3D>("Camera/Raycast");
		holder = GetNode<Node3D>("Holder");
		infoText = GetParent().GetNode<Label>("UI/InfoText");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			RotateY(-motion.Relative.X * sens);
			camera.RotateX(-motion.Relative.Y * sens);
			camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-40), Mathf.DegToRad(60)), camera.Rotation.Y, camera.Rotation.Z);
		}
		else if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (GetRaycast() != null)
				{
					heldObject = GetRaycast();
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (GetRaycast() != null)
		{
			infoText.Text = "Wheel";
		}
		else
		{
			infoText.Text = "";
		}

		if (heldObject != null)
		{
			heldObject.GlobalPosition = holder.GlobalPosition;
			heldObject.GlobalRotation = holder.GlobalRotation;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = jumpHeight;

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * walkSpeed;
			velocity.Z = direction.Z * walkSpeed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, walkSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public Node3D GetRaycast()
	{
		var collider = raycast.GetCollider() as StaticBody3D;
		if (raycast.IsColliding())
		{
			if (collider.IsInGroup("pickupable"))
			{
				return collider.GetParent() as Node3D;
			}
		}
		return null;
	}
}
