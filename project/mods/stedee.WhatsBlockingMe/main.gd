extends Node

const SkipButton := preload("res://mods/stedee.WhatsBlockingMe/Scenes/skip_button.tscn")

# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	get_tree().connect("node_added", self, "_add_skip_button") # Replace with function body.

func _add_skip_button(node: Node) -> void:
	if node.name == "loading_menu":
		var button: MarginContainer = SkipButton.instance()
		node.add_child(button)
			
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
