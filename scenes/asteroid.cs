using Godot;
using System;

public partial class Asteroid : CharacterBody2D
{

	[Export] private float _minSpeed = 150.0f;
	[Export] private float _maxSpeed = 250.0f;
	[Export] private int _damage = 10;
	private float _size = 1.0f;
	private int _health = 3;
	
	
	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	public override void _Process(double delta){
		if (Input.IsActionJustPressed("ui_accept")){
			
		}
	}
		public override void _Ready()
	{
		_rng.Randomize();
		float scaleFactor = _rng.RandfRange(0.5f, 2.0f) * _size;
		Scale = new Vector2(scaleFactor, scaleFactor);
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Rotation = (float)GD.RandRange(0, 360);
		var rand = (float)GD.RandRange(0.8f, 1.2f);
		sprite.Scale = new Vector2(rand, rand);
	}

	 public void Explode()
	{
		QueueFree();
	}

	 public void TakeDamage(int amount)
	{
		_health -= amount;
		if (_health <= 0)
		{
			Explode();
		}
	}

	public int Damage => _damage;

}
