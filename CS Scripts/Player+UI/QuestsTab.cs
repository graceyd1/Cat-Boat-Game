using Godot;
using System;

public partial class QuestsTab : Button
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
		text.Text = "\n";
		string curQuest = GlobalScript.CQ("long");
		text.Text += "Main Quest: " + curQuest;
	}
}
