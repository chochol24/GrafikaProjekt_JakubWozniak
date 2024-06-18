using Godot;
using System;
using System.Collections.Generic;

public enum HurtBoxEnum
{
	Cooldown,
	HitOnce,
	DisableHitBox
}

public partial class HurtBox : Area2D
{
	[Signal]
	public delegate void HurtEventHandler(float damage, Vector2 angle, int knockback);
	

	[Export]
	HurtBoxEnum hurtBoxType = 0;
	CollisionShape2D collision;
	Timer disableTimer;

	public List<Node2D> hitOnce = new List<Node2D>();
	public override void _Ready()
	{
		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		disableTimer = GetNode<Timer>("DisableTimer");
	}

	public override void _Process(double delta)
	{
	}


	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup("attack"))
		{
			if ((Object)area.Get("damage") != null)
			{
				switch (hurtBoxType)
				{
					case HurtBoxEnum.Cooldown:
						collision.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
						disableTimer.Start();
						break;
					case HurtBoxEnum.HitOnce:
						break;
					case HurtBoxEnum.DisableHitBox:
						if (area.HasMethod("TempDisable"))
						{
							HitBox x = area as HitBox;
							x.TempDisable();
						}
						break;
				}
				var dmg = 0f;	
				var angle = Vector2.Zero;
				var knockback = 1;
				
				if (area is HitBox h)
				{
					dmg = h.damage;
				}
				if (area is Bone b)
				{
					angle = b.angle;
					knockback = b.knockback;
					dmg = b.damage;
					b.EnemyHit(1);
				}
				EmitSignal(SignalName.Hurt, dmg, angle, knockback);
			}
		}

	}

	public void RemoveFromList(Object o)
	{
		if(hitOnce.Contains((Node2D)o))
			hitOnce.Remove((Node2D)o);
	}


	private void OnDisableTimerTimeout()
	{
		collision.CallDeferred(CollisionShape2D.MethodName.SetDisabled, false);
	}

}




