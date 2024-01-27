using Godot;
using System;

public partial class RotateWithSpeed : Node3D
{
	[Export] public Vector3 rotationAngle = Vector3.Zero;
	public override void _Process(double delta) => RotationDegrees += rotationAngle;
}
