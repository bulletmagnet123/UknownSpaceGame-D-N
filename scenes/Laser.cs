using Godot;
using System;

public partial class Laser : Node2D
{
	private Tween tween;
	private float beamLength = 1000;
	private bool isActive = false;
	private bool shoot = false;
	
	[Export]
	private Line2D line2d;
	
	[Export]
	private RayCast2D laser;
	
	private CharacterBody2D player;
	
	public override void _Ready()
	{
		line2d = GetNode<Line2D>("Line2D");
		laser = GetNode<RayCast2D>("Laser");
		player = GetParent() as CharacterBody2D;  // Get the parent node as Player
		
		tween = GetTree().CreateTween();
		Vector2 mousePos = GetLocalMousePosition();
		isActive = false;
		
		if (player != null)
		{
			laser.AddException(player);  // Exclude the player from collision detection
		}
		else
		{
			GD.Print("Player node not found");
		}
	}
	
	private void UpdateLaser(float value)
	{
		line2d.Points = new Vector2[] { new Vector2(0, 0), new Vector2(beamLength * value, 0) };
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("ui_shoot"))
		{
			if (!isActive)  // Only activate if not already active
			{
				isActive = true;
				line2d.Visible = true;
				
				// Update beam length before animation
				if (laser.IsColliding())
				{
					Vector2 collisionPoint = laser.GetCollisionPoint();
					beamLength = laser.GlobalPosition.DistanceTo(collisionPoint);
				}
				else
				{
					beamLength = 1000;
				}
				
				tween = GetTree().CreateTween();
				tween.TweenMethod(
					Callable.From<float>(UpdateLaser), 
					0.0f, 
					1.0f, 
					0.5
				).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Linear);
			}
		}
		else if (Input.IsActionJustReleased("ui_shoot"))
		{
			if (isActive)  // Only deactivate if currently active
			{
				isActive = false;
				tween = GetTree().CreateTween();
				tween.TweenMethod(
					Callable.From<float>(UpdateLaser), 
					1.0f, 
					0.0f, 
					0.5
				).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Linear);
			}
		}
		
		// Continuous updates while laser is active
		if (isActive && player != null)
		{
			laser.GlobalPosition = player.GlobalPosition;
			laser.ForceRaycastUpdate();
			
			if (laser.IsColliding())
			{
				Vector2 collisionPoint = laser.GetCollisionPoint();
				beamLength = laser.GlobalPosition.DistanceTo(collisionPoint);
				line2d.SetPointPosition(1, new Vector2(beamLength, 0));  // Update end point in real-time
			}
			else
			{
				beamLength = 1000;
				line2d.SetPointPosition(1, new Vector2(beamLength, 0));
			}
		}
	}
}
