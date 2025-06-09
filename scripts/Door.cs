using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class Door : StaticBody3D, IDescription
{
    Node3D hinge;
    bool toggle = false;
    CharacterBody3D player;
    float maxAngle;
    float angleVel;
    //what axis to rotate around, set to 1 if you want to rotate around that axis, else 0
    float x;
    float y;
    float z;


    public override void _Ready()
    {
        player = GetNode<CharacterBody3D>("../../../../Player");
        hinge = GetNode<Node3D>("..");
        maxAngle = (float)GetMeta("maxAngle");
        angleVel = (float)GetMeta("angleVel");
        x = (float)GetMeta("x");
        y = (float)GetMeta("y");
        z = (float)GetMeta("z");
    }
    public string Description()
    {
        return "Press <E> to open or close";
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
            if (hinge.Rotation.Y <= Mathf.DegToRad(maxAngle) && hinge.Rotation.X <= Mathf.DegToRad(maxAngle) && hinge.Rotation.Z <= Mathf.DegToRad(maxAngle))
            {
                hinge.Rotation += new Vector3(Mathf.DegToRad(angleVel) * x, Mathf.DegToRad(angleVel) * y, Mathf.DegToRad(angleVel) * z);
            }
        }
        else
        {
            if (hinge.Rotation.Y > 0 || hinge.Rotation.X > 0 || hinge.Rotation.Z > 0)
            {
                hinge.Rotation -= new Vector3(Mathf.DegToRad(angleVel) * x, Mathf.DegToRad(angleVel) * y, Mathf.DegToRad(angleVel) * z);
            }
            else
            {
                hinge.Rotation = new Vector3(0, 0, 0); // reset to closed position
            }
        }
    }
}
