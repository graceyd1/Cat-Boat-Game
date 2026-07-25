using Godot;
using System;

public partial class ItemIcons : Button
{
	public string ItemName{get; set;}
	
	[Signal]
	public delegate void ThisIconPressedEventHandler(ItemIcons obj);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Hide();
		Pressed += OnButtonPressed;
		
	}
	
	public void OnButtonPressed() {
		EmitSignal(SignalName.ThisIconPressed, this);
	}

	public void SetAnimation(string name) {
		var ani = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (ani.SpriteFrames.HasAnimation(name)) {
			ani.Animation = name;
		}
		else {
			GD.Print("Missing item icon: " + name);
		}
	}
	
	
}
