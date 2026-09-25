using Godot;

public partial class Laser : Node2D
{
	[Export] private int damage = 1;

	private float beamLength = 1000.0f;
	private bool isActive;
	private Line2D line2d;
	private RayCast2D laser;
	private CharacterBody2D player;

	public override void _Ready()
	{
		line2d = GetNode<Line2D>("Line2D");
		laser = GetNode<RayCast2D>("Laser");
		player = GetParent() as CharacterBody2D;
		isActive = false;

		line2d.Visible = false;
		line2d.SetPointPosition(0, Vector2.Zero);
		line2d.SetPointPosition(1, Vector2.Zero);

		if (player != null)
		{
			laser.AddException(player);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateRaycast();

		if (Input.IsActionPressed("ui_shoot"))
		{
			isActive = true;
			line2d.Visible = true;
			UpdateBeamLength();
			line2d.SetPointPosition(1, new Vector2(beamLength, 0));

			if (laser.IsColliding() && laser.GetCollider() is Asteroid asteroid)
			{
				asteroid.TakeDamage(damage);
			}
		}
		else if (isActive)
		{
			isActive = false;
			line2d.Visible = false;
			line2d.SetPointPosition(1, Vector2.Zero);
		}
	}

	private void UpdateRaycast()
	{
		laser.GlobalPosition = GlobalPosition;
		laser.GlobalRotation = GlobalRotation;
		laser.ForceRaycastUpdate();
	}

	private void UpdateBeamLength()
	{
		if (laser.IsColliding())
		{
			beamLength = laser.GlobalPosition.DistanceTo(laser.GetCollisionPoint());
		}
		else
		{
			beamLength = 1000.0f;
		}
	}
}
