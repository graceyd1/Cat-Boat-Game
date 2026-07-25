using Godot;
using System;

public partial class Coin : Node2D
{

	/// <summary>
	/// The index of this coin in GlobalScript
	/// </summary>
	private int CoinIdx;

	private bool Disabled;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Disabled = false;
		var name = Name.ToString();
		CoinIdx = name[^1] - '0'; //get the last char of the name as a number

		if (GlobalScript.CoinsCollected[CoinIdx])
		{
			Disabled = true;
			Hide();
		}
		else
		{
			Disabled = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Node2D player)
	{
		if (!Disabled)
		{
			var anim = GetNode<AnimatedSprite2D>("AnimatedCoin/AnimatedSprite2D");
			anim.Animation = "pickup";
			anim.Play();
			GlobalScript.Coins ++;
			GlobalScript.CoinsCollected[CoinIdx] = true;
			Disabled = true;
		}
	}

	// private void Disable()
	// {
	// 	// var sh	ape = GetNode<CollisionShape2D>("AnimatedCoin/Area2D/CollisionShape2D");
	// 	// shape.SetDeferred("Disabled", true);
	// 	Disabled = true;
	// }
}
