using Godot;
using System;

public partial class Asteroid : CharacterBody2D
{
	[Export] private float _minSpeed = 150.0f;
	[Export] private float _maxSpeed = 250.0f;
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

		HandleCollisions();
		
		MoveAndSlide();
	}

	private void HandleCollisions()
	{
		// Check for collisions
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			Node collider = collision.GetCollider() as Node;
			
			// Check if we collided with the player
			if (collider != null && collider.IsInGroup("Player"))
			{
				// Apply additional spin based on collisionds
				ApplyCollisionSpin(collision);
				
				// If you want to damage the player, you can do it here
				// For example: collider.Call("TakeDamage", _damage);
			}
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
	
	private void ApplyCollisionSpin(KinematicCollision2D collision)
	{
		// Get collision normal
		Vector2 normal = collision.GetNormal();
		
		// Calculate spin direction based on collision normal
		// This creates a more realistic spin based on where the asteroid was hit
		float spinDirection = normal.Cross(Vector2.Right);
		
		// Apply a random spin impulse in the calculated direction
		_angularVelocity += spinDirection * _rng.RandfRange(0.5f, 1.0f) * _collisionSpinImpulse;
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
