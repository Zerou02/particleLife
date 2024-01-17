using System;
using System.Collections.Generic;

public class Config
{
    public int n = 500;
    public double dt = 0.02;
    public double frictionHalfLife = 0.04;
    public double rMax = 0.4;
    public int amountColours = 6;
    public double frictionFactor;
    public List<List<double>> matrix = new List<List<double>>();
    public Config()
    {
        frictionFactor = Math.Pow(0.5, dt / frictionHalfLife);
        matrix = makeRandomMatrix(amountColours, amountColours);

    }
    public List<List<double>> makeRandomMatrix(int height, int width)
    {
        var rows = new List<List<double>>();
        for (int i = 0; i < height; i++)
        {
            var row = new List<double>();
            for (int j = 0; j < width; j++)
            {
                row.Add(new Random().NextDouble() * 2 - 1);
            }
            rows.Add(row);
        }
        return rows;
    }
}