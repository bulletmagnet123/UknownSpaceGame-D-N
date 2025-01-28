extends CharacterBody2D

@export var speed: float = 500
@export var Health: int = 100

func _physics_process(delta):
	var mouse_pos = get_global_mouse_position()
	var direction := Vector2(
		Input.get_action_strength("RIGHT") - Input.get_action_strength("LEFT"),
		Input.get_action_strength("DOWN") - Input.get_action_strength("UP")
	) 
	if direction.length() > 1.0:
		direction = direction.normalized()
		
	var mouse_direction = mouse_pos - global_position
	var angle = mouse_direction.angle()
	rotation = angle
	velocity = direction * speed
	move_and_slide()
	


func _on_collision(body):
	if body.name == "Asteroid":
		Health -= 10
		body.queue_free()
		if Health <= 0:
			queue_free()
		pass
		
