extends MarginContainer

func _ready():
	pass

func _process(delta):
	# show after t in loading menu resets 5 times
	if get_parent().b >= 5:
		$VBoxContainer/HBoxContainer/Button.show()

func _on_Button_pressed():
	get_parent()._join_world()
