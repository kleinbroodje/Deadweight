using Godot;
using System;

public partial class Ship3 : RigidBody3D
{
    float floatForce = 3.0f;
    float waterHeight = 1.0f;
    float waterDrag = 0.05f;
    float waterAngularDrag = 0.05f;
    public float gravity;
    public MeshInstance3D customOcean;

    bool submerged = false;

    public override void _Ready()
    {
        gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
        customOcean = GetNode<MeshInstance3D>("CustomOcean");
    }

    public override void _PhysicsProcess(double delta)
    {
        submerged = false;
        GD.Print("getHeight: ", customOcean.Call("getHeight"));
        float depth = customOcean.Call("getHeight").As<float>() - GlobalPosition.Y;
        if (depth > 0)
        {
            submerged = true;
            ApplyCentralForce(Vector3.Up * floatForce * gravity * depth);
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
