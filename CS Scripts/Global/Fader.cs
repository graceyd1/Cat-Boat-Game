using Godot;
using System;
using System.Threading.Tasks;

public partial class Fader : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var rect = GetNode<ColorRect>("ColorRect");
		rect.Color = new Color(rect.Color, 0.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async Task FadeIn(float duration) {
		var rect = GetNode<ColorRect>("ColorRect");
		rect.Color = new Color(rect.Color, 0.0f);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(rect, "color:a", 1.0, duration);
		await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);
	}
	
	public async Task FadeOut(float duration) {
		var rect = GetNode<ColorRect>("ColorRect");
		rect.Color = new Color(rect.Color, 1.0f);
		var tween = GetTree().CreateTween();
		tween.TweenProperty(rect, "color:a", 0.0, duration);
		await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);
	}
}
