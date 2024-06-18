using Godot;
using System;

public partial class ItemOption : ColorRect
{
	[Signal]
	public delegate void SelectedUpgradeEventHandler(ItemOption item);

	public bool mouseOver = false;
	public ItemOption item;
	public Player player;

	public float damage = 1.0f;


	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		Connect(SignalName.SelectedUpgrade, new Callable(player, "Upgrade"));
	}

	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsAction("click"))
			if(mouseOver)
				EmitSignal(SignalName.SelectedUpgrade, this);
	}

	private void OnMouseEntered()
	{
		mouseOver = true;
	}


	private void OnMouseExited()
	{
		mouseOver = false;
	}

}


