using System.Collections.Generic;
using Godot;

public partial class Matrix : Control
{
	List<List<double>> matrix = new List<List<double>>();
	Color curr = new Color("red");
	Color dest = new Color("green");
	PackedScene backgroundTile = GD.Load<PackedScene>("res://scenes/BackgroundTile.tscn");
	PackedScene backgroundTileOverlay = GD.Load<PackedScene>("res://scenes/BackgroundTileOverlay.tscn");

	GridDrawOverlay gridDrawOverlay;
	Control grid;
	Control tileOverlayAnchor;
	List<Color> colours = new List<Color>();

	bool isVisible = true;
	bool tileIsCurrentlyPressed = false;
	int tileWidth = 0;
	int tileHeight = 0;
	public override void _Ready()
	{
		gridDrawOverlay = GetNode<GridDrawOverlay>("GridDrawOverlay");
		grid = GetNode<Control>("Grid");
		tileOverlayAnchor = GetNode<Control>("TileOverlayAnchor");
		GetTree().Root.SizeChanged += () => setMatrix(matrix, colours);
	}

	public void setVisible(bool val)
	{
		this.Visible = val;

	}
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		base._Draw();
	}

	public void handleOverlayClose(double val, BackgroundTile backgroundTile)
	{
		backgroundTile.setValue(val);
		var idxX = (int)backgroundTile.Position.X / tileWidth;
		var idxY = (int)backgroundTile.Position.Y / tileHeight;
		this.matrix[idxY - 1][idxX - 1] = val;
		tileIsCurrentlyPressed = false;
	}
	public void handleTilePress(BackgroundTile backgroundTile)
	{
		if (this.tileIsCurrentlyPressed || backgroundTile.Position.X == 0 || backgroundTile.Position.Y == 0)
		{
			return;
		}
		this.tileIsCurrentlyPressed = true;
		var x = backgroundTileOverlay.Instantiate<BackgroundTileOverlay>();
		tileOverlayAnchor.AddChild(x);
		x.init(backgroundTile.getVal());
		x.onClose += (x) => handleOverlayClose(x, backgroundTile);
		tileOverlayAnchor.Position = backgroundTile.Position;
	}

	public BackgroundTile createTile(int posX, int posY, int sizeX, int sizeY)
	{
		var tile = backgroundTile.Instantiate<BackgroundTile>();
		tile.Size = Size with { X = sizeX, Y = sizeY };
		tile.Position = Position with { X = posX, Y = posY };
		return tile;
	}

	public void createTileColour(int idxX, int idxY, int sizeX, int sizeY, Color color)
	{
		var tile = createTile(idxX * sizeX, idxY * sizeY, sizeX, sizeY);
		grid.AddChild(tile);
		tile.btnPressed += () => handleTilePress(tile);
		tile.setColour(color);
	}

	public void createTileValue(int idxX, int idxY, int sizeX, int sizeY, double value)
	{
		var tile = createTile(idxX * sizeX, idxY * sizeY, sizeX, sizeY);
		grid.AddChild(tile);
		tile.btnPressed += () => handleTilePress(tile);
		tile.setValue(value);
	}
	public void setMatrix(List<List<double>> matrix, List<Color> colours)
	{
		foreach (var x in grid.GetChildren())
		{
			x.QueueFree();
		}
		this.matrix = matrix;
		this.colours = colours;
		var size = matrix.Count + 1;
		tileWidth = (int)(Size.X / size);
		tileHeight = (int)(Size.Y / size);
		gridDrawOverlay.setParameter(tileWidth, tileHeight, matrix.Count);
		createTileColour(0, 0, tileWidth, tileHeight, new Color("gray"));
		for (int x = 1; x < size; x++)
		{
			createTileColour(x, 0, tileWidth, tileHeight, colours[x - 1]);
		}
		for (int y = 1; y < size; y++)
		{
			createTileColour(0, y, tileWidth, tileHeight, colours[y - 1]);
			for (int x = 1; x < size; x++)
			{
				createTileValue(x, y, tileWidth, tileHeight, matrix[y - 1][x - 1]);
			}
		}
		//gridDrawOverlay.draw();
	}
}
