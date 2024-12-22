extends MarginContainer


# Declare member variables here. Examples:
# var a = 2


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if get_parent().b >= 5:
		$VBoxContainer/HBoxContainer/Button.show()


func _on_Button_pressed():
	get_parent()._join_world() # Replace with function body.
