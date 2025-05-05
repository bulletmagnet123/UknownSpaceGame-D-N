using Godot;
using System;

public partial class Game : Node2D
{
	private PackedScene _asteroidScene;
	public int Health = 100;
	
	public enum AsteroidSize
	{
		SMALL,
		MEDIUM,
		LARGE
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_asteroidScene = GD.Load<PackedScene>("res://scenes/asteroid.tscn");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Health <= 0)
		{
			SpawnAsteroid(GlobalPosition, AsteroidSize.SMALL);
		}
		
		if (Input.IsActionJustPressed("ui_select"))  // Assuming "ui_select" is mapped to a key
		{
			Health = 0;
			return;
		}
	}
	
	public void SpawnAsteroid(Vector2 pos, AsteroidSize size)
	{
		var asteroid = _asteroidScene.Instantiate<Node2D>();
		asteroid.GlobalPosition = pos;
		asteroid.Set("size", (int)size);
		
		AddChild(asteroid);
	}
	
	public void OnAsteroidCollided(Vector2 pos, AsteroidSize size)
	{
		// Spawn new asteroids based on the size of the collided asteroid
		if (size == AsteroidSize.LARGE)
		{
			SpawnAsteroid(pos, AsteroidSize.MEDIUM);
			SpawnAsteroid(pos, AsteroidSize.MEDIUM);
		}
		else if (size == AsteroidSize.MEDIUM)
		{
			SpawnAsteroid(pos, AsteroidSize.SMALL);
			SpawnAsteroid(pos, AsteroidSize.SMALL);
		}
	}
}
