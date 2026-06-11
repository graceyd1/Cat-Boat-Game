using Godot;
using System;


public partial class Camera2d : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GetParent()?.GetParent().Name == "FirstRoom") {
			SetLimit(Side.Left, 0);
			SetLimit(Side.Right, 500);
			SetLimit(Side.Top, 0);
			SetLimit(Side.Bottom, 180);
		}

		//temporary
		if (GetParent()?.GetParent().Name == "TestingRoom") {
			SetLimit(Side.Left, 0);
			SetLimit(Side.Right, 500);
			SetLimit(Side.Top, 0);
			SetLimit(Side.Bottom, 180);
		}

		if (GetParent()?.GetParent().Name == "EnterCaveRoom")
		{
			SetLimit(Side.Left, 0);
			SetLimit(Side.Right, 320);
			SetLimit(Side.Top, 0);
			SetLimit(Side.Bottom, 180);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
