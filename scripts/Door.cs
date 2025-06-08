using Godot;
using System;

public partial class Door : StaticBody3D, IDescription
{
    Node3D hinge;
    bool toggle = false;
    CharacterBody3D player;
    float maxAngle = 145f;
    float angleVel = 4f;
    public override void _Ready()
    {
        player = GetNode<CharacterBody3D>("/root/Ocean/Player");
        hinge = GetNode<Node3D>("..");
    }
    public string Description()
    {
        return "Press <E> to open or close the door";
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("toggle_door"))
        {
            var ray = (player as Player).GetRaycast();
            if (ray != null && ray.Name == Name)
                toggle = !toggle;
        }
        if (toggle)
        {
            if (hinge.Rotation.Y <= Mathf.DegToRad(maxAngle))
            {
                hinge.Rotation += new Vector3(0, Mathf.DegToRad(angleVel), 0);
            }
        }
        else
        {
            if (hinge.Rotation.Y > 0)
            {
                hinge.Rotation -= new Vector3(0, Mathf.DegToRad(angleVel), 0);
            }
            else
            {
                hinge.Rotation = new Vector3(0, 0, 0); // reset to closed position
            }
        }
    }
}
