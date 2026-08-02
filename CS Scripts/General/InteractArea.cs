using Godot;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class InteractArea : Area2D
{
	[Export]
	public int LabelHeight {get; set;} = 20;
	
	[Export]
	public int DoorType {get; set;} //1 = enter, 0 = exit

	[Signal]
	public delegate void InteractEventHandler();

	[Signal]
	public delegate void InteractReturnAreaNameEventHandler(String areaName);
	//if door - name used to decide which room to enter

	public bool playerNear;
	
	public bool allowInteraction;

	private Control interactLabel;
	
	private List<string> ExitRooms = new List<string> {
		"BobaShop",
		"PlantShop",
		"SubmarineShop",
		"ParvaHouse"
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		allowInteraction = true;

		//looks up to 3 parent levels up to find interact label
		//there's probably a better way to do this
		interactLabel = GetParent().GetNodeOrNull<Control>("InteractLabel");
		try
		{
			if (interactLabel == null)
			{
				interactLabel = GetParent().GetParent().GetNodeOrNull<Control>("InteractLabel");
			}
			if (interactLabel == null)
			{
				interactLabel = GetParent().GetParent().GetParent().GetNodeOrNull<Control>("InteractLabel");
			}
		}
		catch {}

		if (interactLabel != null)
		{
			interactLabel.Hide();
			// if (ExitRooms.Contains(Owner.Name)) {
			// 	interactLabel.Scale = new Vector2(0.5f, 0.5f);
			// }
			// else {
			// 	interactLabel.Scale = new Vector2(0.8f, 0.8f);
			// }
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (playerNear && allowInteraction)
		{
			if (Input.IsActionJustPressed("enter"))
			{
				//interact signals
				EmitSignal(SignalName.Interact);
				EmitSignal(SignalName.InteractReturnAreaName, Name);
			}
		}
	}

	/// <summary>
	/// Enables/disables the interact area and hides the interact label if set to false
	/// </summary>
	/// <param name="allowInteract"></param>
	public void Interactable(Boolean allowInteract)
	{
		allowInteraction = allowInteract;
		if (!allowInteract && interactLabel != null)
		{
			interactLabel.Hide();
		}
	}

	/// <summary>
	/// Enables the interact area after 0.1 seconds to avoid infinite looping
	/// </summary>
	public async void DelayEnable()
	{
		var timer = GetNode<Godot.Timer>("Timer");
		timer.Start();
		await ToSignal(timer, Godot.Timer.SignalName.Timeout);

		allowInteraction = true;
		if (interactLabel != null && GetOverlappingBodies().Count() > 0)
		{
			interactLabel.Show();
		}
	}

	public void ResetLabelPos()
	{
		if (interactLabel != null)
		{
			interactLabel.Position = new Vector2(GlobalPosition.X, GlobalPosition.Y - LabelHeight);
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (allowInteraction)
		{
			if (interactLabel != null)
			{
				interactLabel.Position = new Vector2(GlobalPosition.X, GlobalPosition.Y - LabelHeight);
				interactLabel.Show();
			}
		}
		playerNear = true;
	}

	private void OnBodyExited(Node2D body)
	{
		if (allowInteraction)
		{
			interactLabel?.Hide(); //if interact label not null
		}
		playerNear = false;
	}	
}
