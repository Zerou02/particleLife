using Godot;
using System;

public partial class BackgroundTile : TextureRect
{
	Button button;
	double val;
	[Signal]
	public delegate void btnPressedEventHandler();
	public override void _Ready()
	{
		button = GetNode<Button>("Button");
		button.Pressed += () => { EmitSignal(SignalName.btnPressed); };
	}

	/// <summary>
	/// Range zwischen 0 und 1
	/// </summary>
	public void setValue(double val)
	{
		this.val = val;
		var newVal = lerpRange(val, 0, 1, -1, 1);
		var red = new Color("red");
		var green = new Color("green");
		this.Modulate = red.Lerp(green, (float)newVal);
	}

	public double getVal()
	{
		return val;
	}

	public double lerpRange(double val, double outputStart, double outputEnd, double inputStart, double inputEnd)
	{
		double slope = (outputEnd - outputStart) / (inputEnd - inputStart);
		return outputStart + slope * (val - inputStart);
	}

	public void setSize(int x, int y)
	{
		this.Size = Size with { X = x, Y = y };
	}

	public void setColour(Color colour)
	{
		this.Modulate = colour;
	}
}
