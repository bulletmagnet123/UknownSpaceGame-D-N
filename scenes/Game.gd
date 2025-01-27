extends Node2D

var asteroid_scene = preload("res://scenes/asteroid.tscn")
var Health = 100

enum AsteroidSize { SMALL, MEDIUM, LARGE }

# Called when the node enters the scene tree for the first time.
func _ready() -> void:	
	pass

func _process(delta: float) -> void:
	if Health <= 0:
		spawn_asteroid(global_position, AsteroidSize.SMALL)
	if Input.is_action_just_pressed("ui_select"):  # Assuming "ui_select" is mapped to a key
		Health = 0
		return
	pass
	# Called every frame. Delta is the elapsed time since the previous frame.

func spawn_asteroid(pos: Vector2, size: int):
	var a = asteroid_scene.instantiate()
	a.global_position = pos
	a.size = size
	add_child(a)

func _on_asteroid_collided(pos: Vector2, size: int):
	# Spawn new asteroids based on the size of the collided asteroid
	if size == AsteroidSize.LARGE:
		spawn_asteroid(pos, AsteroidSize.MEDIUM)
		spawn_asteroid(pos, AsteroidSize.MEDIUM)
	elif size == AsteroidSize.MEDIUM:
		spawn_asteroid(pos, AsteroidSize.SMALL)
		spawn_asteroid(pos, AsteroidSize.SMALL)
