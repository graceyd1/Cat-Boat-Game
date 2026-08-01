using Godot;
using System;

public partial class ControlsTab : Button
{
	private Label text;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		text = GetNode<Label>("../Expanded/MarginContainer/VBoxContainer/Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnButtonPressed() {
		GetNode<MarginContainer>("%MarginContainer").Show();
		GetNode<Node2D>("%QuitOptions").Hide();
		GetNode<Node2D>("%ItemsUI").Hide();
		text.Text = "\nWASD & arrows - movement\nEnter & space - continue dialogue, enter room/house";
		if (GlobalScript.Inventory.Contains("flashlight")) {
			text.Text += "\nF - toggle flashlight";
		}
	}
}
