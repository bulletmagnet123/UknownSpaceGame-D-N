using Godot;

public partial class EnemyLaser : Node2D
{
	[Export] private int _damage = 1;

	private float _beamLength = 1000.0f;
	private bool _isActive;
	private Line2D _line2D;
	private RayCast2D _laser;
	private CharacterBody2D _player;
		
	public override void _Ready()
	{
		_line2D = GetNode<Line2D>("Line2D");
		_laser = GetNode<RayCast2D>("Laser");
		_player = GetParent() as CharacterBody2D;
		_isActive = false;

		_line2D.Visible = false;
		_line2D.SetPointPosition(0, Vector2.Zero);
		_line2D.SetPointPosition(1, Vector2.Zero);

		if (_player != null)
		{
			_laser.AddException(_player);
		}
		Tween tween = CreateTween();
	}
	public Tween tween
	{
		get;
		private set;
	}
	

	public override void _PhysicsProcess(double delta)
	{
		UpdateRaycast();

		if (Input.IsActionPressed("ui_shoot"))
		{
			_isActive = true;
			_line2D.Visible = true;
			UpdateBeamLength();
			_line2D.SetPointPosition(1, new Vector2(_beamLength, 0));

			if (_laser.IsColliding() && _laser.GetCollider() is Asteroid asteroid)
			{
				asteroid.TakeDamage(_damage);
			}
		}
		else if (_isActive)
		{
			_isActive = false;
			_line2D.Visible = false;
			_line2D.SetPointPosition(1, Vector2.Zero);
		}
	}

	private void UpdateRaycast()
	{
		_laser.GlobalPosition = GlobalPosition;
		_laser.GlobalRotation = GlobalRotation;
		_laser.ForceRaycastUpdate();
	}

	private void UpdateBeamLength()
	{
		if (_laser.IsColliding())
		{
			_beamLength = _laser.GlobalPosition.DistanceTo(_laser.GetCollisionPoint());
		}
		else
		{
			_beamLength = 1000.0f;
		}
	}
}
