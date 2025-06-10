using System;
using Godot;

public partial class Player : CharacterBody3D
{
	// constants
	public const float walkSpeed = 5.0f;
	public const float maxWheelVel = 2f;
	public const float jumpHeight = 7.0f;
	public const float floatForce = 1.0f;
	public const float waterHeight = 1.0f;
	public const float wheelAccel = 0.07f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public const float sensAt1280 = 0.0025f;
	public float resRatio = (float)ProjectSettings.GetSetting("display/window/size/viewport_width") / 1280.0f;
	public float sens;
	public float CameraMaxAngle;

	// necessary misc nodes
	Camera3D camera;
	RayCast3D raycast;
	// UI
	InfoText infoText;
	// interactable nodes
	Node3D holder;
	RigidBody3D heldObject;
	Wheel wheel;

	// methods
	public override void _Ready()
	{
		sens = sensAt1280 / resRatio;
		FloorMaxAngle = Mathf.DegToRad(60);
		CameraMaxAngle = Mathf.DegToRad(90);
		camera = GetNode<Camera3D>("Camera");
		raycast = GetNode<RayCast3D>("Camera/Raycast");
		infoText = GetNode<Label>("/root/Main/CanvasLayer/UI/InfoText") as InfoText;
		holder = GetNode<Node3D>("Camera/Holder");
		wheel = GetNode<Wheel>("../Ship/Wheel");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion motion)
		{
			RotateY(-motion.Relative.X * sens);
			camera.RotateX(-motion.Relative.Y * sens);
			camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.X, -CameraMaxAngle, CameraMaxAngle), camera.Rotation.Y, camera.Rotation.Z);
		}
		else if (Input.IsActionJustPressed("interact"))
		{
			// first, if there is anything in hand, drop it
			if (heldObject != null)
			{
				heldObject.CollisionLayer = 1;
				Vector3 forward = -camera.GlobalTransform.Basis.Z.Normalized();
				heldObject.LinearVelocity = 3 * forward + 4 * Vector3.Up;
				heldObject = null;
			}

			// now you can check whether we can pick something up
			var ray = GetRaycast();
			if (ray != null)
			{
				// check whether object is pickable or object is a special object
				if (ray.IsInGroup("pickupable"))
				{
					// its pickable, so its a rigidbody
					// check if something is already in hand
					if (heldObject != null)
					{
						// drop
						heldObject.CollisionLayer = 1;
					}
					// get new pickupable and set it to see-through
					heldObject = GetRaycast() as RigidBody3D;
					heldObject.CollisionLayer = 2;
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// check if the ray hits an object
		PhysicsBody3D r = GetRaycast();
		if (r != null && r.IsInGroup("pickupable"))
		{
			// ray hits pickupable physics object
			infoText.Rename($"{r.Name}\n<F> to pick up");
		}
		else if (r is IDescription descib)
		{
			// ray hits a non-physics object that still has displayed info
			infoText.Rename(descib.Description());
		}
		else
		{
			// ray hits nothing, so show no text
			infoText.Rename(" ");
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

	public PhysicsBody3D GetRaycast()
	{
		if (raycast.IsColliding())
		{
			return raycast.GetCollider() as PhysicsBody3D;
		}
		return null;
	}
}
