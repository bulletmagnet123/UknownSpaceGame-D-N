extends CharacterBody2D
 
var movement_vector := Vector2()
func _on_asteroid_collided(position: Vector2, size: int) -> void:
	print("Asteroid collided at position: ", position, " with size: ", size)
signal asteroid_collided




var rotation_speed: = randf_range(-1, 1) * PI / 2
@export var size := AsteroidSize.LARGE


var speed: = randf_range(50, 100)
enum AsteroidSize{LARGE, MEDIUM, SMALL}
@export var asteroidSize := AsteroidSize

func SpawnAsteroids(size: int, position: Vector2) -> void:
	var asteroid = preload("res://scenes/asteroid.tscn").instantiate()
	asteroid.size = size
	asteroid.position = position
	get_parent().add_child(asteroid)


func _on_body_entered(body):
	if body.name == "Player":  # Assuming the player's node is named "Player"
		emit_signal("asteroid_collided", global_position, size)
		queue_free()	
		

func _ready():
	rotation = randf_range(0, 2*PI)
	movement_vector = Vector2(randf_range(-1, 1), randf_range(-1, 1)).normalized()
	connect("asteroid_collided", Callable(self, "_on_asteroid_collided"))
	print(rotation_degrees)
	
	
	match size:
		AsteroidSize.LARGE:
			speed = randf_range(50, 100)
			
		AsteroidSize.MEDIUM:
			speed = randf_range(100, 150)

		AsteroidSize.SMALL:
			speed = randf_range(100, 200)
			
func _physics_process(delta):
	global_position += movement_vector * speed * delta
	rotation += rotation_speed * delta
