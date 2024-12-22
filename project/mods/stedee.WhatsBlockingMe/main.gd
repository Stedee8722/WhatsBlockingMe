extends Node

const SkipButton := preload("res://mods/stedee.WhatsBlockingMe/Scenes/skip_button.tscn")

func _ready():
	get_tree().connect("node_added", self, "_add_skip_button")

func _add_skip_button(node: Node) -> void:
	if node.name == "loading_menu":
		var button: MarginContainer = SkipButton.instance()
		node.add_child(button)
