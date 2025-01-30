extends Node2D

var tween

var beam_length = 1000
const RAY_LENGTH = 1000
var is_active: bool
var shoot = false

@onready var line2d: Line2D = $Line2D  # Ensure the path matches the node name
@onready var Laser: RayCast2D = $Laser
@onready var Player: CharacterBody2D = get_parent() as CharacterBody2D  # Get the parent node as Player
@onready var collision = $Line2D/CollisionShape2D

func _ready() -> void:
	tween = get_tree().create_tween()
	var mousepos = get_local_mouse_position()
	is_active = false
	if Player:
		Laser.add_exception(Player)  # Exclude the player from collision detection
	else:
		print("Player node not found")

func _physics_process(delta: float) -> void:
	if Input.is_action_pressed("ui_shoot"):
		if not is_active:
			is_active = true
			visible = true
			tween.kill()
			tween.tween_property(line2d, "points", [Vector2(0, 0), Vector2(beam_length, 0)], 0.5)
			tween.set_trans(Tween.TRANS_LINEAR)
			tween.set_ease(Tween.EASE_IN_OUT)
			
	else:
		if is_active:
			is_active = false
			visible = false
			tween.kill()  # Stop all tweens before starting a new one
			tween.tween_property(line2d, "points", [Vector2(0, 0), Vector2(0, 0)], 0.5)
			tween.set_trans(Tween.TRANS_LINEAR)
			tween.set_ease(Tween.EASE_IN_OUT)
			

	if is_active:
		line2d.visible = true
		if Player:
			Laser.global_position = Player.global_position  # Set laser position to player position
			#Laser.cast_to = (get_global_mouse_position() - Laser.global_position).normalized() * 10000  # Extend the laser beam infinitely towards the target
			Laser.force_raycast_update()
			if Laser.is_colliding():
				var collision_point = Laser.get_collision_point()
				line2d.points = [Vector2(0, 0), Laser.to_local(collision_point)]
				shoot = true
			else:
				line2d.points = [Vector2(0, 0), Vector2(beam_length, 0)]
				shoot = false
		else:
			print("Player node not found in _physics_process")
	else:
		line2d.visible = false
