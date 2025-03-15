extends Node2D

var tween

var beam_length = 1000
var is_active: bool
var shoot = false

@onready var line2d: Line2D = $Line2D  # Ensure the path matches the node name
@onready var Laser: RayCast2D = $Laser
@onready var Player: CharacterBody2D = get_parent() as CharacterBody2D  # Get the parent node as Player

func _ready() -> void:
	tween = get_tree().create_tween()
	var mousepos = get_local_mouse_position()
	is_active = false
	if Player:
		Laser.add_exception(Player)  # Exclude the player from collision detection
	else:
		print("Player node not found")

func update_laser(value: float) -> void:
	line2d.points = [Vector2(0, 0), Vector2(beam_length * value, 0)]

func _physics_process(delta: float) -> void:
	if Input.is_action_just_pressed("ui_shoot"):
		if not is_active:  # Only activate if not already active
			is_active = true
			line2d.visible = true
			
			# Update beam length before animation
			if Laser.is_colliding():
				var collision_point = Laser.get_collision_point()
				beam_length = Laser.global_position.distance_to(collision_point)
			else:
				beam_length = 1000
				
			tween = get_tree().create_tween()
			tween.tween_method(Callable(self, "update_laser"), 0.0, 1.0, 0.5).set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_LINEAR)
			
	elif Input.is_action_just_released("ui_shoot"):
		if is_active:  # Only deactivate if currently active
			is_active = false
			tween = get_tree().create_tween()
			tween.tween_method(Callable(self, "update_laser"), 1.0, 0.0, 0.5).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_LINEAR)
			
	# Continuous updates while laser is active
	if is_active and Player:
		Laser.global_position = Player.global_position
		Laser.force_raycast_update()
		
		if Laser.is_colliding():
			var collision_point = Laser.get_collision_point()
			beam_length = Laser.global_position.distance_to(collision_point)
			line2d.points[1] = Vector2(beam_length, 0)  # Update end point in real-time
		else:
			beam_length = 1000
			line2d.points[1] = Vector2(beam_length, 0)
