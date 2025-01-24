extends CharacterBody2D

@export var speed: float = 500

func _physics_process(delta):
	var direction := Vector2(
		Input.get_action_strength("RIGHT") - Input.get_action_strength("LEFT"),
		Input.get_action_strength("DOWN") - Input.get_action_strength("UP")
	) 
	if direction.length() > 1.0:
		direction = direction.normalized()
		
	velocity = direction * speed
	move_and_slide()
