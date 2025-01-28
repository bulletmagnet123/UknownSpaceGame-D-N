extends RayCast2D

var tween

var beam_length = 1000
const RAY_LENGTH = 1000

var is_active: bool

@onready var Laser: RayCast2D = $RayCast2D
@onready var cam: Camera2D = $Camera2D

var laser_color = Color(0.0235294, 1, 1, 1)
func _ready() -> void:
	tween = get_tree().create_tween()
	is_active = true
	var mousepos = get_local_mouse_position()
	
	
func _physics_process(delta: float) -> void:
	print(Laser.is_colliding())
	
	

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
