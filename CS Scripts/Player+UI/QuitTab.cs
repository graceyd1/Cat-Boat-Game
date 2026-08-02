using Godot;
using System;

public partial class QuitTab : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Node2D>("%QuitOptions").Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnButtonPressed() {
		GetNode<MarginContainer>("%MarginContainer").Show();
		GetNode<Node2D>("%QuitOptions").Show();
		GetNode<Label>("../Expanded/MarginContainer/VBoxContainer/Label").Text = "";
	}

	private async void OnMainMenuButtonPressed()
	{
		GlobalScript.SaveGame();
		var fader = GetNode<Fader>("/root/Fader");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		await fader.FadeIn(.7f);
		await GlobalSceneChange.ChangeRoom(Vector2.Zero, "title_screen", false);
	}

	private void OnCloseGameButtonPressed()
	{
		GlobalScript.SaveGame();
		GetTree().Quit();
	}
}
