using Godot;
using System;

public partial class HitBox : Area2D
{
	[Export]
	public float damage = 10.0f;

	CollisionShape2D collision;
	Timer disableTimer;
	public override void _Ready()
	{
		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		disableTimer = GetNode<Timer>("DisableHitBoxTimer");
	}

	public void TempDisable()
	{
		collision.Call(CollisionShape2D.MethodName.SetDisabled, true);
		disableTimer.Start();
	}

	private void OnDisableHitBoxTimerTimeout()
	{
		collision.Call(CollisionShape2D.MethodName.SetDisabled, false);
	}
}



