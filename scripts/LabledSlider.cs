using System;
using Godot;
public partial class LabledSlider : Control
{
	[Export]
	double step = 1;
	[Export]
	double min = 0;
	[Export]
	double max = 100;
	[Export]
	double startVal = 50;
	FontSizeResizingLable label;
	HSlider hSlider;
	double val;

	[Signal]
	public delegate void valueChangedEventHandler(double val);
	public override void _Ready()
	{
		label = GetNode<FontSizeResizingLable>("Label");
		hSlider = GetNode<HSlider>("HSlider");
		hSlider.MinValue = min;
		hSlider.Step = step;
		hSlider.MaxValue = max;
		hSlider.Value = startVal;
		label.resize();
		hSlider.ValueChanged += (val) => { setVal(val); };
		setVal(hSlider.Value);
		hSlider.DragEnded += (changed) => EmitSignal(SignalName.valueChanged, val);

	}


	void setVal(double val)
	{
		this.val = val;
		setText();
	}

	void setText()
	{
		label.Text = this.Name + ": " + Utils.trimAfterDecimalPoint(val.ToString(), 2);
	}

	public double getVal()
	{
		return val;
	}
}
