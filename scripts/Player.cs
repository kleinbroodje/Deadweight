using Godot;

public partial class Player : CharacterBody3D
{
	// constants
	public const float walkSpeed = 5.0f;
	public const float jumpHeight = 7.0f;
	public const float floatForce = 1.0f;
	public const float waterHeight = 1.0f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public const float sensAt1280 = 0.0012f;
	public float resRatio = (float)ProjectSettings.GetSetting("display/window/size/viewport_width") / 1280.0f;
	public float sens;
	public float CameraMaxAngle;

	// nodes
	Camera3D camera;
	RayCast3D raycast;
	Node3D holder;
	RigidBody3D heldObject;
	InfoText infoText;

	// methods
	public override void _Ready()
	{
		sens = sensAt1280 / resRatio;
		FloorMaxAngle = Mathf.DegToRad(60);
		CameraMaxAngle = Mathf.DegToRad(90);
		camera = GetNode<Camera3D>("Camera");
		raycast = GetNode<RayCast3D>("Camera/Raycast");
		holder = GetNode<Node3D>("Camera/Holder");
		infoText = GetParent().GetNode<Label>("UI/InfoText") as InfoText;
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
			if (GetRaycast() != null)
			{
				// check if something is already in hand
				if (heldObject != null)
				{
					// drop
					heldObject.CollisionLayer = 1;
				}
				// get new pickupable and set it to see-through
				heldObject = GetRaycast();
				heldObject.CollisionLayer = 2;
			}
		}
		else if (Input.IsActionJustPressed("drop"))
		{
			heldObject.CollisionLayer = 1;
			Vector3 forward = -camera.GlobalTransform.Basis.Z.Normalized();
			heldObject.LinearVelocity = 3 * forward + 4 * Vector3.Up;
			heldObject = null;
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		Node3D r = GetRaycast();
		if (GetRaycast() != null)
		{
			infoText.Rename(r.Name);
		}
		else
		{
			infoText.Rename("");
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

	public RigidBody3D GetRaycast()
    {
        if (raycast.IsColliding())
		{
        	var collider = raycast.GetCollider() as RigidBody3D;
			if (collider != null && collider.IsInGroup("pickupable"))
			{
				return collider;
			}
		}
        return null;
    }
}
