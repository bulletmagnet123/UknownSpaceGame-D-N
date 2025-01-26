extends CharacterBody2D
 
var movement_vector := Vector2()

var rotation_speed: = randf_range(-1, 1) * PI / 2

var speed: = randf_range(50, 100)
#enum AsteroidSize{LARGE, MEDIUM, SMALL}
#@export var asteroidSize := AsteroidSize

func _ready():
	rotation = randf_range(0, 2*PI)
	print(rotation_degrees)
	movement_vector = Vector2(randf_range(-1, 1), randf_range(-1, 1)).normalized()
	
	#match size:
	#	AsteroidSize.LARGE:
	#		speed = randf_range(50, 100)
	#	AsteroidSize.MEDIUM:
	#		speed = randf_range(100, 150)
	#	AsteroidSize.SMALL:
	#		speed = randf_range(100, 200)
