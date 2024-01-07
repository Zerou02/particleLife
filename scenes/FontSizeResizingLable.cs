using Godot;
using System;

public partial class FontSizeResizingLable : Label
{
	[Export]
	int baseFontSize = 16;
	public override void _Ready()
	{
		GetTree().Root.SizeChanged += () => resize();
		resize();
	}

	public void resize()
	{
		var factor = this.Text.Length > 5 ? (this.Size.X / CustomMinimumSize.X) : (this.Size.Y / CustomMinimumSize.Y);
		this.LabelSettings.FontSize = (int)(factor * baseFontSize);
	}

	public override void _Process(double delta)
	{
	}
}
