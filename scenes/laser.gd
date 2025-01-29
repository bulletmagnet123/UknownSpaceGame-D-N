extends Node2D

var tween

var beam_length = 1000
const RAY_LENGTH = 1000
var is_active: bool
var shoot = false

@onready var line2d: Line2D = $Line2D  # Ensure the path matches the node name
@onready var Laser: RayCast2D = $Laser
@onready var Player: CharacterBody2D = get_parent() as CharacterBody2D # Adjust the path to the player node
@onready var collision = $Line2D/CollisionShape2D
@onready var LaserNode = $Laser


func _ready() -> void:
	var player = get_node("Player")
	tween = get_tree().create_tween()
	var mousepos = get_local_mouse_position()
	is_active = false
	Player = get_tree().root.get_node("Player")  # Adjust the path to the player node
	if player:
		Laser.add_exception(Player)  # Exclude the player from collision detection
	else:
		print("Player node not found")

func _physics_process(delta: float) -> void:
	if Input.is_action_pressed("ui_shoot"):
		is_active = true
	else:
		is_active = false
	if is_active:
		line2d.visible = true
		line2d.points = [Vector2(0, 0), Vector2(beam_length, 0)]
		if Player:
			Laser.global_position = Player.global_position  # Set laser position to player position
			Laser.cast_to = (get_global_mouse_position() - Laser.global_position).normalized() * 10000  # Extend the laser beam infinitely towards the target
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
