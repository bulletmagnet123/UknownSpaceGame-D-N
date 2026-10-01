using Godot;
using System;

public partial class LaserCollider : Node
{
	private PlayerController playerController;
	private Line2D line2D;
	private CollisionObject2D Collider;
	private Asteroid asteroid;
		
	public override void _Ready()
	{
		base._Ready();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
		base._PhysicsProcess(delta);
			Collider = Collider as CollisionObject2D;
			if (Collider != null && Collider is Asteroid)
			{
				var asteroid = Collider as Asteroid;
				asteroid.TakeDamage(1);
			}
			_OnArea2DBodyEntered(Collider);
	}

	public void _OnArea2DBodyEntered(Node2D body)
	{
		if (Collider != null && Collider is Asteroid)
		{
			var Enemy = Collider as Asteroid;
			Enemy.TakeDamage(1);
		}
		if (body is PlayerController player)
		{
			player.Health -= 1;
		}
	}

	
	public override void _ExitTree()
	{
		base._ExitTree();
	}
}
