using Godot;
using System;

public partial class CoinDisplayNode : Node2D
{
	private Vector2 pos;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pos = new Vector2(10, -2);
		GetNode<Label>("%CoinCount").Text = GlobalScript.Coins.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("%CoinCount").Text = GlobalScript.Coins.ToString();
		GlobalPosition = GetNode<Camera2D>("..").GetScreenCenterPosition() + pos;
	}
}
