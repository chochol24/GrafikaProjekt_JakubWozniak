using Godot;
using System;

public partial class BasicButton : Button
{
	[Signal]
	public delegate void ClickEndEventHandler();

	private void _on_pressed()
	{
		EmitSignal(SignalName.ClickEnd);
	}

}


