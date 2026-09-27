using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyScript : Node2D
{
	[Export]
	public float Speed { get; set; } = 100f;

	public Vector2 Velocity { get; set; } = Vector2.Zero;

	private PlayerController _player;

	public override void _Ready()
	{
		base._Ready();
		_player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
	}
	
	public int Health { get; set; } = 100;
	public enum MovementSatesAnimationEnemy
	{
		idle,
		move,
		shoot
	}

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
		if (player == null || !IsInstanceValid(player))
		{
			Velocity = Vector2.Zero;
			return;
		}

		Vector2 direction = player.GlobalPosition - GlobalPosition;

		if (direction.LengthSquared() > 0.001f)
		{
			direction = direction.Normalized();
			Rotation = Mathf.Atan2(direction.Y, direction.X);
		}

		Velocity = direction * Speed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_player == null || !IsInstanceValid(_player))
		{
			_player = GetTree().GetFirstNodeInGroup("Player") as PlayerController;
			if (_player == null || !IsInstanceValid(_player))
			{
				return;
			}
		}

		LookAt(_player.GlobalPosition);
		SetMovementSatesAnimationPlayer(MovementSatesAnimationEnemy.move);

		MoveToPlayer(_player, delta);
		// Apply displacement cleanly in a single location
		GlobalPosition += Velocity * (float)delta;
	}
}
