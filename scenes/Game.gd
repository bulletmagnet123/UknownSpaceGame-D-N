extends Node
var asteroid_scene = preload("res://scenes/asteroid.tscn")

enum AsteroidSize { SMALL, MEDIUM, LARGE }

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	spawn_initial_asteroids()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	
	pass # Replace with function body.

func spawn_asteroid(pos, size):
	var a = asteroid_scene.instantiate()
	a.global_position = pos
	a.size = size
	a.connect("asteroid_collided", Callable(self, "_on_asteroid_collided"))
	add_child(a)

func _on_player_collided():
	
	spawn_asteroid(Vector2(100, 100), AsteroidSize.SMALL)
	pass

func _on_asteroid_collided(pos, size):
	# Spawn new asteroids based on the size of the collided asteroid
	if size == AsteroidSize.LARGE:
		spawn_asteroid(pos, AsteroidSize.MEDIUM)
		spawn_asteroid(pos, AsteroidSize.MEDIUM)
	elif size == AsteroidSize.MEDIUM:
		spawn_asteroid(pos, AsteroidSize.SMALL)
		spawn_asteroid(pos, AsteroidSize.SMALL)

func spawn_initial_asteroids():
	# Example function to spawn initial asteroids
	spawn_asteroid(Vector2(100, 100), AsteroidSize.LARGE)
	spawn_asteroid(Vector2(200, 200), AsteroidSize.MEDIUM)
