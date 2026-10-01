using System;
using System.Runtime.InteropServices.JavaScript;
using Godot;

public partial class Asteroid : RigidBody2D
{
	[Export] private float _minSpeed = 0.0f;
	[Export] private float _maxSpeed = 4.0f;
	[Export] private float _minSpinSpeed = 0.5f;
	[Export] private float _maxSpinSpeed = 5.0f;
	[Export] private int _damage = 1;
	[Export] private float _collisionSpinImpulse = 2.0f;
	private float _size = 1.0f;
	[Export] public int Health = 3;
	private float _angularVelocity = 0.0f;
	public Vector2 _velocity = Godot.Vector2.Zero;
	[Export] CollisionShape2D _collision;

	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	
	public void TakeDamage(int damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			Explode();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Rotation += _angularVelocity * (float)delta;
		_velocity = LinearVelocity;
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
		CollisionShape2D collisionShape = _collision;
		collisionShape.Scale = new Vector2(scaleFactor, scaleFactor);
		velocity *= _rng.RandfRange(_minSpeed, _maxSpeed);
		LinearVelocity = velocity;
		AngularVelocity = _angularVelocity;
		_angularVelocity = _rng.RandfRange(_minSpinSpeed, _maxSpinSpeed);
	}
	
	public void Explode()
	{
		Asteroid asteroid = new Asteroid();
		asteroid.QueueFree();
	}
}
