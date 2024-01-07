using Godot;
using System;

public partial class GridDrawOverlay : Control
{
	int tileSizeX = 0;
	int tileSizeY = 0;
	int gridSize = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		base._Draw();
		for (int y = 0; y < gridSize + 2; y++)
		{
			DrawLine(new Vector2(0, y * tileSizeY), new Vector2((gridSize + 1) * tileSizeX, y * tileSizeY), new Color("black"));
		}
		for (int x = 0; x < gridSize + 2; x++)
		{
			DrawLine(new Vector2(x * tileSizeX, 0), new Vector2(x * tileSizeX, (gridSize + 1) * tileSizeY), new Color("black"));
		}
	}

	public void setParameter(int tileX, int tileY, int gridSize)
	{
		tileSizeX = tileX;
		tileSizeY = tileY;
		this.gridSize = gridSize;
	}

	public void draw()
	{
		QueueRedraw();
	}
}
