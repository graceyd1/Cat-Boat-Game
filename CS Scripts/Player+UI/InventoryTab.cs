using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryTab : Button
{
	private Label text;
	
	private int IconNum = 1;
	
	public static Dictionary<string, string> Descriptions = new Dictionary<string, string> {
		{"Flashlight", "Useful for seeing in the dark and growing vines! Press F to toggle, Use mouse to point"},
		{"Town Pass", "Lets you exit the town"},
		{"Pearl", "Very shiny and round. Maybe some of the townspeople would be interested in something like this..."}
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		text = GetNode<Label>("../Expanded/MarginContainer/VBoxContainer/Label");
	}
	
	public void OnButtonPressed() {
		GetNode<MarginContainer>("%MarginContainer").Hide();
		GetNode<Node2D>("%ItemsUI").Show();
		if (GlobalScript.Inventory.Count > 0) {
			GetNode<Label>("%ItemName").Text = "Select an Item";
			GetNode<Label>("%Description").Text = "Description will appear here";
		}
		else {
			GetNode<Label>("%ItemName").Text = "No Items";
			GetNode<Label>("%Description").Text = "You don't own any items. Try exploring the town.";
		}
		foreach (string item in GlobalScript.Inventory) {
			var icon = GetNode<ItemIcons>("%ItemsUI/ItemIcons" + IconNum);
			if (icon == null) {
				GD.Print("Missing Icon Node: " + item);
			}
			else {
				icon.SetAnimation(item);
				icon.ItemName = item;
				icon.ThisIconPressed += OnIconPressed;
				if (GlobalScript.NumItems.TryGetValue(item, out var count)) {
					icon.SetItemCount(count());
				}
				else {
					icon.HideCount();
				}
				icon.Show();
				IconNum++;
			}
			
		}
		IconNum = 1;
	}
	
	public void OnIconPressed(ItemIcons icon) {
		GetNode<Label>("%ItemName").Text = icon.ItemName;
		if (Descriptions.TryGetValue(icon.ItemName, out string desc)) {
			GetNode<Label>("%Description").Text = desc;
		}
		else {
			GD.Print("Missing item description in dictionary: " + icon.ItemName);
		}
	}
	
}
