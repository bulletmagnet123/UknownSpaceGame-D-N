using Godot;

namespace BulletmagnetAndIzendaleGameProject.scenes;

public partial class EnemyLaser : Node2D
{
	[Export] private float _beamTweenDuration = 0.18f;
	private Line2D _line2D;
	private RayCast2D _laser;
	private Tween _beamTween;

	[Export] public int Damage = 1;

	public override void _Ready()
	{
		_line2D = GetNode<Line2D>("Line2D");
		_laser = GetNode<RayCast2D>("Laser");
		_line2D.ClearPoints();
		_line2D.AddPoint(Vector2.Zero);
		_line2D.AddPoint(Vector2.Zero);
		_line2D.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateRaycast();
		base._PhysicsProcess(delta);
	}

	private void UpdateRaycast()
	{
		_laser.GlobalRotation = GlobalRotation;
		_laser.ForceRaycastUpdate();
	}

	private void StartBeamPulse()
	{
		UpdateRaycast();
		Vector2 endPoint = _laser.IsColliding()
			? _line2D.ToLocal(_laser.GetCollisionPoint())
			: _line2D.ToLocal(_laser.ToGlobal(_laser.TargetPosition));

		_line2D.Visible = true;
		_line2D.SetPointPosition(1, Vector2.Zero);

		_beamTween = CreateTween();
		_beamTween.TweenMethod(
			Callable.From<float>(progress => SetBeamProgress(progress, endPoint)),
			0.0f,
			1.0f,
			_beamTweenDuration
		).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		_beamTween.TweenCallback(Callable.From(DamageTarget));
		_beamTween.TweenMethod(
			Callable.From<float>(progress => SetBeamProgress(progress, endPoint)),
			1.0f,
			0.0f,
			_beamTweenDuration
		).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
		_beamTween.TweenCallback(Callable.From(HideBeam));
	}

	private void SetBeamProgress(float progress, Vector2 endPoint)
	{
		_line2D.SetPointPosition(1, endPoint * progress);
	}

	private void HideBeam()
	{
		_line2D.Visible = false;
		_line2D.SetPointPosition(1, Vector2.Zero);
		_beamTween = null;
	}

	private void DamageTarget()
	{
		if (_laser.IsColliding())
		{
			var collider = _laser.GetCollider();
			if (collider is PlayerController player)
			{
				player.TakeDamage(Damage);
			}
			else if (collider is Asteroid hitAsteroid)
			{
				hitAsteroid.TakeDamage(Damage);
			}
		}
	}

	public void FireLaser(bool active)
	{
		if (!active)
		{
			return;
		}

		while (_beamTween == null || !_beamTween.IsRunning())
		{
			StartBeamPulse();
		}
	}
}
