using System;
using Godot;

public partial class Asteroid : RigidBody2D
{
	[Export] private float _minSpeed = 0.0f;
	[Export] private float _maxSpeed = 4.0f;
	[Export] private float _minSpinSpeed = 0.5f;
	[Export] private float _maxSpinSpeed = 5.0f;
	[Export] private int _damage = 10;
	[Export] private float _collisionSpinImpulse = 2.0f;
	private float _size = 1.0f;
	private int _health = 3;
	private float _angularVelocity = 0.0f;
	public Vector2 _velocity = Godot.Vector2.Zero;

	private RandomNumberGenerator _rng = new RandomNumberGenerator();

	public override void _PhysicsProcess(double delta)
	{
		Rotation += _angularVelocity * (float)delta;
		_velocity = LinearVelocity;
	}

	void Collider(RigidBody2D body, CollisionShape2D collider)
	{
		_velocity = Vector2.Zero;
		
		collider.Disabled = false;
		CollisionLayer = 1;
		CollisionMask = 1;
		body.LinearVelocity = Vector2.Zero;
		body.AngularVelocity = 0.0f;
		body.ApplyCentralImpulse(Vector2.Zero);

		_health = 3;
		_angularVelocity = 0.0f;
	}



	public override void _Ready()
	{

		GravityScale = 0.0f;
		Vector2 velocity = new Vector2((float)GD.RandRange(-1.0f, 2.0f), (float)GD.RandRange(-1.0f, 2.0f)).Normalized();
		_rng.Randomize();
		float scaleFactor = _rng.RandfRange(0.5f, 2.0f) * _size;
		Scale = new Vector2(scaleFactor, scaleFactor);
		var sprite = GetNode<Sprite2D>("Sprite2D");
		//sprite.Rotation = (float)GD.RandRange(0, 360);
		var rand = (float)GD.RandRange(0.8f, 4.0f);
		sprite.Scale = new Vector2(rand, rand);
		
		// Set random spin direction and speed
		int direction = _rng.RandiRange(0, 1) == 0 ? -1 : 1;  // Random direction (left or right)
		_angularVelocity = _rng.RandfRange(_minSpinSpeed, _maxSpinSpeed) * direction;  // Random speed
		
		velocity *= _rng.RandfRange(_minSpeed, _maxSpeed);
		LinearVelocity = velocity;
		AngularVelocity = _angularVelocity;
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
