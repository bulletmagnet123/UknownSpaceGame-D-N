using Godot;

namespace BulletmagnetAndIzendaleGameProject.scenes;

public partial class Laser : Node2D
{
	[Export] private int damage = 30;
	[Export] private float beamTweenDuration = 0.18f;
	[Export] private Line2D line2D;
	[Export] private RayCast2D laser;
	[Export] private CharacterBody2D player;
	private Tween beamTween;

	public override void _Ready()
	{
		line2D = GetNode<Line2D>("Line2D");
		laser = GetNode<RayCast2D>("Laser");
		player = GetParent() as CharacterBody2D;

		line2D.ClearPoints();
		line2D.AddPoint(Vector2.Zero);
		line2D.AddPoint(Vector2.Zero);
		line2D.Visible = false;

		if (player != null)
		{
			laser.AddException(player);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateRaycast();
		if (Input.IsActionPressed("ui_shoot") && (beamTween == null || !beamTween.IsRunning()))
		{
			FireBeamPulse();
		}
		

		base._PhysicsProcess(delta);
	}

	private void UpdateRaycast()
	{
		laser.GlobalPosition = GlobalPosition;
		laser.GlobalRotation = GlobalRotation;
		laser.ForceRaycastUpdate();
	}

	private void FireBeamPulse()
	{
		Vector2 endPoint = laser.IsColliding()
			? line2D.ToLocal(laser.GetCollisionPoint())
			: line2D.ToLocal(laser.ToGlobal(laser.TargetPosition));

		line2D.Visible = true;
		line2D.SetPointPosition(1, Vector2.Zero);

		beamTween = CreateTween();
		beamTween.TweenMethod(
			Callable.From<float>(progress => SetBeamProgress(progress, endPoint)),
			0.0f,
			1.0f,
			beamTweenDuration
		).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		beamTween.TweenCallback(Callable.From(DamageTarget));
		beamTween.TweenMethod(
			Callable.From<float>(progress => SetBeamProgress(progress, endPoint)),
			1.0f,
			0.0f,
			beamTweenDuration
		).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
		beamTween.TweenCallback(Callable.From(HideBeam));
	}

	private void SetBeamProgress(float progress, Vector2 endPoint)
	{
		line2D.SetPointPosition(1, endPoint * progress);
	}

	private void HideBeam()
	{
		line2D.Visible = false;
		line2D.SetPointPosition(1, Vector2.Zero);
		beamTween = null;
	}

	private void DamageTarget()
	{
		if (laser.IsColliding())
		{
			var collider = laser.GetCollider();
			
			if (collider is Asteroid hitAsteroid)
			{
				hitAsteroid.TakeDamage(damage);
			}
			else if (collider is EnemyScript directEnemy)
			{
				directEnemy.TakeDamage(damage);
			}
			else if (collider is Node node && node.GetParent() is EnemyScript parentEnemy)
			{
				parentEnemy.TakeDamage(damage);
			}
		}
	}
}
