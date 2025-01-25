extends CharacterBody2D

@export var speed: float = 500

func _physics_process(delta):
	var MousePos = get_viewport().get_mouse_position()
	
	var direction := Vector2(
		Input.get_action_strength("RIGHT") - Input.get_action_strength("LEFT"),
		Input.get_action_strength("DOWN") - Input.get_action_strength("UP")
	) 
	if direction.length() > 1.0:
		direction = direction.normalized()
		

	var _mouse_direction = MousePos - global_position
	var angle = _mouse_direction.angle()	
	rotation = angle
	velocity = direction * speed
	move_and_slide()
