using System;
using Godot;

public partial class Asteroid : RigidBody2D
{
	[Export] private float _minSpeed = 0.0f;
	[Export] private float _maxSpeed = 0.0f;
	[Export] private float _minSpinSpeed = 0.5f;
	[Export] private float _maxSpinSpeed = 2.0f;
	[Export] private int _damage = 10;
	[Export] private float _collisionSpinImpulse = 2.0f;
	private float _size = 1.0f;
	private int _health = 3;
	private float _angularVelocity = 0.0f;
	
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	public override void _PhysicsProcess(double delta)
	{
		Rotation += _angularVelocity * (float)delta;
	}

	
	
	public override void _Ready()
	{
		GravityScale = 0.0f;
		var Velocity = Vector2.Zero;
		_rng.Randomize();
		float scaleFactor = _rng.RandfRange(0.5f, 2.0f) * _size;
		Scale = new Vector2(scaleFactor, scaleFactor);
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Rotation = (float)GD.RandRange(0, 360);
		var rand = (float)GD.RandRange(0.8f, 1.2f);
		sprite.Scale = new Vector2(rand, rand);
		
		// Set random spin direction and speed
		int direction = _rng.RandiRange(0, 1) == 0 ? -1 : 1;  // Random direction (left or right)
		_angularVelocity = _rng.RandfRange(_minSpinSpeed, _maxSpinSpeed) * direction;  // Random speed
		
		// If you're not setting velocity elsewhere, set it here
		Vector2 direction2D = new Vector2(_rng.RandfRange(-1.0f, 1.0f), _rng.RandfRange(-1.0f, 1.0f)).Normalized();
		float speed = _rng.RandfRange(_minSpeed, _maxSpeed);
		Velocity = direction2D * speed;
	}
	
	public void Explode()
	{
		QueueFree();
	}
	
	public int Damage => _damage;

	public void TakeDamage(int amount)
	{
		_health -= amount;
		if (_health <= 0)
		{
			Explode();
		}
	}
	
}
