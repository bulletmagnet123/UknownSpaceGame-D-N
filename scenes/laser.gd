extends Node2D

var tween

var beam_length = 1000
const RAY_LENGTH = 1000
var is_active: bool

@onready var Laser: RayCast2D = $Laser
@onready var Player: CharacterBody2D = get_tree().root.get_node("Player")  # Adjust the path to the player node

func _ready() -> void:
	tween = get_tree().create_tween()
	var mousepos = get_local_mouse_position()
	

func _physics_process(delta: float) -> void:
		Laser.global_position = get_global_mouse_position()
		if Laser.is_colliding():
			var collider = Laser.get_collider()
			if collider.name != "Player":  # Ensure it's not colliding with the player
				print("Laser collided with something")

func activate() -> void:
	tween.stop_all()
	tween.tween_property(self, "modulate:a", 0, 1, 0.5, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.start()
	visible = true

func deactivate() -> void:
	tween.stop_all()
	tween.tween_property(self, "modulate:a", 1, 0, 0.5, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	tween.start()
	visible = false
