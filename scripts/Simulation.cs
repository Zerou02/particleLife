using System;
using System.Collections.Generic;
using System.Threading;
using Godot;
public class Simulation
{
    public List<Particle> particles = new List<Particle>();
    public List<Color> colourVals = new List<Color>() { new Color("red"), new Color("blue"), new Color("green"), new Color("purple"), new Color("orange"), new Color("black") };
    public Config config;
    public SpatialHashGrid spatialHashGrid;

    Vector3 pivot = new Vector3(0, 0, 0);
    Vector3 dim = new Vector3(2, 2, 2);

    List<Thread> threads = new List<Thread>();
    List<bool> threadSynchronizer = new List<bool>();
    List<int> ids = new List<int>();
    public Simulation()
    {
        config = new Config();
        spatialHashGrid = new SpatialHashGrid(new Vector3(1, 1, 1), dim, pivot, config.rMax);
        spatialHashGrid.debug = false;
        initSim();
    }

    void createThreads(int chunkSize)
    {
        var lastI = 0;
        int id = 0;

        for (int i = 0; i < config.n; i += chunkSize)
        {
            ids.Add(i / chunkSize);
            threadSynchronizer.Add(false);
            threads.Add(new Thread(() => updateParticleVelocity(i, chunkSize, ids[ids.Count - 1])));
            lastI = i;
        }
        ids.Add(lastI / chunkSize);
        threadSynchronizer.Add(false);
        threads.Add(new Thread(() => updateParticleVelocity(lastI, chunkSize, ids[ids.Count - 1])));
        foreach (var x in threads)
        {
            x.Start();
        }
    }

    void initSim()
    {
        clearSim();
        createRandomColours(config.amountColours);
        spatialHashGrid.clear();
        addParticle(config.n);
        createThreads(500);
    }

    void createRandomColours(int amount)
    {
        colourVals = new List<Color>();
        var rnd = new Random();
        for (int i = 0; i < amount; i++)
        {
            colourVals.Add(Color.FromHsv((float)rnd.NextDouble(), 1, 1));
        }
        config.matrix = config.makeRandomMatrix(amount, amount);
    }

    void rebuildHashGrid()
    {
        spatialHashGrid.initCells();
        spatialHashGrid.setParticleRadius(config.rMax);
        foreach (var x in particles) { spatialHashGrid.addToGrid(x); }
    }
    void addParticle(int amount)
    {
        var random = new Random();
        for (int i = 0; i < amount; i++)
        {
            var colour = random.Next(config.amountColours);
            Vector3 position = Vector3.Zero;
            position.X = (float)random.NextDouble() * dim.X; // * 2 - 1;
            position.Y = (float)random.NextDouble() * dim.X; // * 2 - 1;
            position.Z = (float)random.NextDouble() * dim.X; // * 2 - 1;
            Vector3 velocity = Vector3.Zero;
            velocity.X = 0;
            velocity.Y = 0;
            velocity.Z = 0;
            var newParticle = new Particle(position, velocity, colour);
            particles.Add(newParticle);
            spatialHashGrid.addToGrid(newParticle);
        }
    }

    void removeParticles(int amount)
    {
        for (int i = particles.Count - amount; i < particles.Count; i++)
        {
            spatialHashGrid.removeFromGrid(particles[i]);
        }
        particles.RemoveRange(particles.Count - amount, amount);
    }

    void clearSim()
    {
        particles = new List<Particle>();
    }

    public void printMatrix()
    {
        foreach (var y in config.matrix)
        {
            var str = "";
            foreach (var x in y)
            {
                str += x + ", ";
            }
            GD.Print(str);
        }
    }

    double force(double r, double a)
    {
        double beta = 0.3;
        if (r < beta)
        {
            return r / beta - 1;
        }
        else if (beta < r && r < 1)
        {
            return a * (1 - Math.Abs(2 * r - 1 - beta) / (1 - beta));
        }
        else
        {
            return 0;
        }
    }

    void updateParticleVelocity(int rangeBegin, int amount, int id)
    {
        while (true)
        {

            GD.Print(id);
            if (threadSynchronizer[id])
            {
                return;
            }
            for (int i = rangeBegin; i < Math.Min(amount + rangeBegin, config.n); i++)
            {
                double totalForceX = 0;
                double totalForceY = 0;
                double totalForceZ = 0;

                foreach (var j in spatialHashGrid.findNearbyClients(particles[i]))
                {
                    if (j.id == particles[i].id) { continue; }

                    double rx = j.position.X - particles[i].position.X;
                    double ry = j.position.Y - particles[i].position.Y;
                    double rz = j.position.Z - particles[i].position.Z;
                    double r = Math.Sqrt(rx * rx + ry * ry + rz * rz);
                    if (r > 0 && r < config.rMax)
                    {
                        double f = force(r / config.rMax, config.matrix[particles[i].colour][j.colour]);
                        totalForceX += rx / r * f;
                        totalForceY += ry / r * f;
                        totalForceZ += rz / r * f;

                    }
                }
                totalForceX *= config.rMax;
                totalForceY *= config.rMax;
                totalForceZ *= config.rMax;

                var newX = particles[i].velocity.X * (float)config.frictionFactor;
                var newY = particles[i].velocity.Y * (float)config.frictionFactor;
                var newZ = particles[i].velocity.Z * (float)config.frictionFactor;

                newX += (float)(totalForceX * config.dt);
                newY += (float)(totalForceY * config.dt);
                newZ += (float)(totalForceZ * config.dt);
                particles[i].velocity = new Vector3(newX, newY, newZ);
            }
            threadSynchronizer[id] = true;
            GD.Print("ENDINNER");
        }
    }

    void updatePos()
    {
        for (int i = 0; i < config.n; i++)
        {
            var newX = particles[i].position.X + (float)(particles[i].velocity.X * config.dt);
            var newY = particles[i].position.Y + (float)(particles[i].velocity.Y * config.dt);
            var newZ = particles[i].position.Z + (float)(particles[i].velocity.Z * config.dt);

            if (newX > dim.X) newX = pivot.X + 0.1f;
            if (newY > dim.Y) newY = pivot.Y + 0.1f;
            if (newZ > dim.Z) newZ = pivot.Z + 0.1f;

            if (newX < pivot.X) newX = dim.X - 0.1f;
            if (newY < pivot.Y) newY = dim.Y - 0.1f;
            if (newZ < pivot.Z) newZ = dim.Z - 0.1f;

            particles[i].position = new Vector3(newX, newY, newZ);
            spatialHashGrid.updateParticle(particles[i]);
        }
    }
    public void step()
    {
        var amountFinished = 0;
        foreach (var x in threadSynchronizer)
        {
            if (x) amountFinished++;
        }
        GD.Print(amountFinished);
        if (amountFinished != threadSynchronizer.Count)
        {
            updatePos();
            for (int i = 0; i < threadSynchronizer.Count; i++)
            {
                threadSynchronizer[i] = false;
            }
        }

    }

    public void changeRadiusTo(double val)
    {
        this.config.rMax = val;
        rebuildHashGrid();
    }

    public void changeSpeedTo(double val)
    {
        this.config.dt = val;
    }

    public void changeParticleTypesTo(int amount)
    {
        this.config.amountColours = amount;
        initSim();
    }

    public void changeAmountParticlesTo(int amount)
    {
        if (amount > config.n)
        {
            addParticle(amount - config.n);
        }
        else
        {
            removeParticles(config.n - amount);
        }
        config.n = amount;
    }

}