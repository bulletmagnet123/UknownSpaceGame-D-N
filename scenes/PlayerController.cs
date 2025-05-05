using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 500;

	[Export]
	public int Health { get; set; } = 100;

	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		var speed = 500;
		var velocity = Vector2.Zero;
		var direction = new Vector2(
		Input.GetActionStrength("RIGHT") - Input.GetActionStrength("LEFT"),
		Input.GetActionStrength("DOWN") - Input.GetActionStrength("UP") );
		
		if (direction.Length() > 1.0f)
		{
		 	direction = direction.Normalized();
		}
		
		Velocity = direction * Speed;
		MoveAndSlide();
		
		if (Input.IsActionPressed("RIGHT"))
		{
			velocity.X += speed;
			
		}
		if (Input.IsActionPressed("LEFT"))
		{
			velocity.X -= speed;
			
		}
		if (Input.IsActionPressed("DOWN"))
		{
			velocity.Y += speed;
			
		}
		if (Input.IsActionPressed("UP"))
		{
			velocity.Y -= speed;
			
		}
		var mousePosition = GetGlobalMousePosition();
		var directionToMouse = mousePosition - GlobalPosition;

		if (directionToMouse.Length() > 0)
		{
			LookAt(mousePosition);
		}
		
	}

	private void OnCollision(Node2D body)
	{
		if (body is Asteroid asteroid)
		{
			Health -= asteroid.Damage;
			asteroid.QueueFree();
		}
	}
}
