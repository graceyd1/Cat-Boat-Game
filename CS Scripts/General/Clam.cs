using Godot;
using System;

public partial class Clam : Sprite2D
{
	private int ClamIdx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var name = Name.ToString();
		ClamIdx = name[^1] - '0'; //get the last char of the name as a number

		if (GlobalScript.ClamsCollected[ClamIdx])
		{
			Frame = 0;
			GetNode<InteractArea>("InteractArea").Interactable(false);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnInteract()
	{
		GetNode<InteractArea>("InteractArea").Interactable(false);
		Frame = 0;
		GetParent().GetNode<Player>("UnderwaterPlayer").GetItem("Pearl");
		GlobalScript.ClamsCollected[ClamIdx] = true;
	}
}
