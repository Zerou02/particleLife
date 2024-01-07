using Godot;
using System;
public partial class Camera : Camera3D
{
	Vector3 vel = Vector3.Zero;
	Vector3 rotVec = Vector3.Zero;
	Vector2 lastMousePos = Vector2.Zero;
	bool isActive = false;
	float speed = 100;
	public override void _Ready()
	{
	}

	public void setActive(bool isActive)
	{
		this.isActive = isActive;
	}

	public override void _Process(double delta)
	{
		if (!isActive) { return; }
		var vel = Vector3.Zero;
		if (Input.IsActionPressed("moveLeft")) { vel += -Transform.Basis.X; }
		if (Input.IsActionPressed("moveRight")) { vel += Transform.Basis.X; }
		if (Input.IsActionPressed("moveForward")) { vel += -Transform.Basis.Z; }
		if (Input.IsActionPressed("moveBackward")) { vel += Transform.Basis.Z; }
		if (Input.IsActionPressed("fastMove"))
		{
			this.speed = 10;
		}
		else
		{
			this.speed = 100;
		}
		this.Position += vel.LimitLength((float)(1.0 / speed));

		var rot = this.Rotation;
		this.Rotation = Rotation with { X = rot.X + rotVec.X, Y = rot.Y + rotVec.Y, Z = rot.Z };
		rotVec = Vector3.Zero;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if (!isActive) { return; }
		if (@event is InputEventMouseMotion)
		{
			var speed = 0.05f;
			var e = @event as InputEventMouseMotion;
			rotVec.Y += -speed * Math.Sign(e.Relative.X);
			rotVec.X += -speed * Math.Sign(e.Relative.Y);
		}
	}
}
