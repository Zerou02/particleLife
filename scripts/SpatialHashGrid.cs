//Pyanodons

using System;
using System.Collections.Generic;
using Godot;
using Vector3 = Godot.Vector3;

public class SpatialHashGrid
{
    Vector3 cellSize;
    Vector3 dimensions;
    Vector3 pivot;

    double particleRadius = 0;
    Dictionary<string, List<Particle>> cells = new Dictionary<string, List<Particle>>();
    public bool debug = false;
    public SpatialHashGrid(Vector3 cellSize, Vector3 dimensions, Vector3 pivot, double particleRadius)
    {
        this.particleRadius = particleRadius;
        this.cellSize = cellSize;
        this.dimensions = dimensions;
        this.pivot = pivot;
        initCells();
    }

    public void setParticleRadius(double radius)
    {
        this.particleRadius = radius;
    }
    public void initCells()
    {
        cells = new Dictionary<string, List<Particle>>();
        for (int x = (int)pivot.X; x <= dimensions.X / cellSize.X; x++)
        {
            for (int y = (int)pivot.Y; y <= dimensions.Y / cellSize.Y; y++)
            {
                for (int z = (int)pivot.Z; z <= dimensions.Z / cellSize.Z; z++)
                {
                    var key = x + "," + y + "," + z;
                    cells.TryAdd(key, new List<Particle>());
                }
            }
        }
    }
    public void addToGrid(Particle particle)
    {
        var start = Utils.startTimeMeasurement();
        var a = calcGridCells(particle);
        foreach (var x in a)
        {
            cells[x].Add(particle);
        }
        if (debug) { Utils.endTimeMeasurement(start, "AddToGrid"); }
    }

    List<string> calcGridCells(Particle particle)
    {
        var start = Utils.startTimeMeasurement();
        var retList = new List<string>();
        var leftmostX = (int)(particle.position.X - particleRadius);
        var rightmostX = (int)(particle.position.X + particleRadius);
        var leftmostY = (int)(particle.position.Y - particleRadius);
        var rightmostY = (int)(particle.position.Y + particleRadius);
        var leftmostZ = (int)(particle.position.Z - particleRadius);
        var rightmostZ = (int)(particle.position.Z + particleRadius);

        leftmostX = (int)(Math.Max(leftmostX, (int)pivot.X) / cellSize.X);
        leftmostY = (int)(Math.Max(leftmostY, (int)pivot.Y) / cellSize.Y);
        leftmostZ = (int)(Math.Max(leftmostZ, (int)pivot.Z) / cellSize.Z);
        rightmostX = (int)(Math.Min(rightmostX, (int)dimensions.X) / cellSize.X);
        rightmostY = (int)(Math.Min(rightmostY, (int)dimensions.Y) / cellSize.Y);
        rightmostZ = (int)(Math.Min(rightmostZ, (int)dimensions.Z) / cellSize.Z);
        for (int x = leftmostX; x <= rightmostX; x++)
        {
            for (int y = leftmostY; y <= rightmostY; y++)
            {
                for (int z = leftmostZ; z <= rightmostZ; z++)
                {
                    var key = x + "," + y + "," + z;
                    retList.Add(key);
                }
            }
        }
        if (debug) { Utils.endTimeMeasurement(start, "CalcCells"); }
        return retList;
    }
    public void updateParticle(Particle particle)
    {
        var start = Utils.startTimeMeasurement();
        removeFromGrid(particle);
        addToGrid(particle);
        if (debug) Utils.endTimeMeasurement(start, "UpdateParticle");
    }

    public void removeFromGrid(Particle particle)
    {
        var start = Utils.startTimeMeasurement();
        foreach (var x in calcGridCells(particle))
        {

            var idx = cells[x].FindIndex(x => x.id == particle.id);
            if (idx == -1)
            {
                continue;
            }
            var last = cells[x][cells[x].Count - 1];
            cells[x][idx] = last;
            cells[x].RemoveAt(cells[x].Count - 1);
        }
        if (debug) Utils.endTimeMeasurement(start, "RemoveParticle");
    }

    public void clear()
    {
        initCells();
    }

    public List<Particle> findNearbyClients(Particle particle)
    {
        var start = Utils.startTimeMeasurement();
        var dict = new Dictionary<int, Particle>();
        var retList = new List<Particle>();
        var indices = this.calcGridCells(particle);
        foreach (var x in indices)
        {
            foreach (var y in cells[x])
            {
                dict.TryAdd(y.id, y);
            }
        }

        foreach (var x in dict.Values)
        {
            retList.Add(x);
        }
        if (debug) Utils.endTimeMeasurement(start, "FindNearby");
        return retList;
    }
}