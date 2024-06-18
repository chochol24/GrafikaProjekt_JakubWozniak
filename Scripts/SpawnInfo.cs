using Godot;
using System;
[GlobalClass]
public partial class SpawnInfo : Resource
{
	[Export]
	public int timeStart;
	[Export]
	public int timeEnd;
	[Export]
	public Resource enemy;
	[Export]
	public int enemyNumber;
	[Export]
	public int enemySpawnDelay;

	public int spawnDelayCounter = 0;
}
