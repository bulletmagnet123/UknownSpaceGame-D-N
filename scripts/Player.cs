using Godot;

public partial class Player : CharacterBody2D
{
	const float Speed = 100.0f;
	Vector2 velocity;


	public override void _Ready()
	{



	}


	public void moveRight()
	{

		velocity.X += Speed;
		GD.Print("Moving RIGHT");
	}

	public void moveLeft()
	{
		velocity.X -= Speed;
		GD.Print("Moving LEFT");
	}




	public void moveUp()
	{
		velocity.Y -= Speed;
		GD.Print("Moving UP");
	}

	public void moveDown()
	{
		velocity.Y += Speed;
		GD.Print("Moving DOWN");
	}


	public override void _PhysicsProcess(double delta)
	{



		Vector2 velocity = Velocity;

		// Gradually stop the player when no input is detected
		velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed * (float)delta);
		velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed * (float)delta);
		Velocity = velocity;
		MoveAndSlide();

		if (Input.IsActionPressed("Up"))
		{
			moveUp();
		}
		if (Input.IsActionPressed("Down"))
		{
			moveDown();
		}


		if (Input.IsActionPressed("Left"))
		{
			moveLeft();
		}
		if (Input.IsActionPressed("Right"))
		{
			moveRight();
		}

	}
	[Export] private Node Type;
}