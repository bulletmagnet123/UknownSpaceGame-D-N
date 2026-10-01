using Godot;
using System;
using BulletmagnetAndIzendaleGameProject.scenes;

public partial class EnemyScript : Node2D
{
	[Export]
	public float Speed { get; set; } = 100f;

	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public AStarGrid2D astarGrid { get; set; } = new AStarGrid2D();

	private PlayerController _player;
	private EnemyLaser _enemyLaser;

	public int Health { get; set; } = 100;
	
	private Vector2[] _currentPath = Array.Empty<Vector2>();
	private int _pathIndex = 0;
	private double _pathRecalcTimer = 0.0;
	private const double PathRecalcInterval = 0.5; // 
	


	public enum MovementSatesAnimationEnemy
	{
		idle,
		move,
		shoot
	}

	public override void _Ready()
	{
		_enemyLaser = GetNodeOrNull<EnemyLaser>("EnemyLasser") ?? GetNodeOrNull<EnemyLaser>("EnemyLaser");
		_player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;

		astarGrid = new AStarGrid2D();
		astarGrid.Region = new Rect2I(-100, -100, 200, 200);
		astarGrid.CellSize = new Vector2I(16, 16);
		astarGrid.Update();
		foreach (Node2D obstacle in GetTree().GetNodesInGroup("Obstacles"))
		{
			Vector2I cell = WorldToCell(obstacle.GlobalPosition);
			astarGrid.SetPointSolid(cell, true);
		}
	}

	public void TakeDamage(int damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Die();
		}
	}

	public void takeDamage(int damage) => TakeDamage(damage);

	public void SetMovementSatesAnimationPlayer(MovementSatesAnimationEnemy state)
	{
		var animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (animationPlayer != null)
		{
			animationPlayer.Play(state.ToString());
		}
	}
	

	public void MoveToPlayer(PlayerController player, double delta)
	{
		// public void MoveToPlayer(PlayerController player, double delta)
		// {
			if (player == null || !IsInstanceValid(player))
			{
				Velocity = Vector2.Zero;
				return;
			}

			Vector2 direction = player.GlobalPosition - GlobalPosition;
			if (direction.LengthSquared() > 0.001f)
			{
				Rotation = Mathf.Atan2(direction.Y, direction.X);
			}

			_pathRecalcTimer -= delta;
			if (_pathRecalcTimer <= 0.0)
			{
				_pathRecalcTimer = PathRecalcInterval;

				Vector2I startCell = WorldToCell(GlobalPosition);
				Vector2I endCell = WorldToCell(player.GlobalPosition);

				_currentPath = astarGrid.GetPointPath(startCell, endCell); // already world coordinates
				_pathIndex = 0;
			}

			if (_currentPath.Length == 0)
			{
				Velocity = direction.Normalized() * Speed; // fallback: no path found, go straight
				return;
			}

			Vector2 target = _currentPath[_pathIndex];
			if (GlobalPosition.DistanceTo(target) < 4f)
			{
				_pathIndex = Mathf.Min(_pathIndex + 1, _currentPath.Length - 1);
				target = _currentPath[_pathIndex];
			}

			Velocity = (target - GlobalPosition).Normalized() * Speed;
		}


	public override void _Process(double delta)
	{
		if (_player == null || !IsInstanceValid(_player))
		{
			_player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
			if (_player == null || !IsInstanceValid(_player))
			{
				return;
			}
			var astar = new AStar2D();
			astar.AddPoint(1, _player.GlobalPosition, 4);
			astar.AddPoint(2, GlobalPosition, 4);
			astar.ConnectPoints(1, 2);
			Velocity = (astar.GetPointPath(1, 2)[1] - _player.GlobalPosition).Normalized() * Speed;
		}
		

		LookAt(_player.GlobalPosition);
		float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

		if (distance <= 10f)
		{
			SetMovementSatesAnimationPlayer(MovementSatesAnimationEnemy.shoot);
			_enemyLaser?.FireLaser(true);
		}
		else
		{
			SetMovementSatesAnimationPlayer(MovementSatesAnimationEnemy.move);
			_enemyLaser?.FireLaser(false);
			MoveToPlayer(_player, delta);
			GlobalPosition += Velocity * (float)delta;
		}
	}
	public void  Die()
	{
		QueueFree();
	}
	private Vector2I WorldToCell(Vector2 worldPos)
	{
		return new Vector2I(
			Mathf.FloorToInt(worldPos.X / astarGrid.CellSize.X),
			Mathf.FloorToInt(worldPos.Y / astarGrid.CellSize.Y)
		);
	}
}
