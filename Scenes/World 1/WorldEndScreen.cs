using Godot;
using System;

public partial class WorldEndScreen : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnKeepPlayingButtonPressed()
	{
		var fader = GetNode<Fader>("/root/Fader");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		await fader.FadeIn(.7f);
		await GlobalSceneChange.ChangeRoom(new Vector2(400, 232), "sea_bunny_room", false);
	}

	private async void OnTitleScreenButtonPressed()
	{
		var fader = GetNode<Fader>("/root/Fader");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		await fader.FadeIn(.7f);
		await GlobalSceneChange.ChangeRoom(Vector2.Zero, "title_screen", false);
	}
}
