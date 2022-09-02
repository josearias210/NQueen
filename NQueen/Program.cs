using GAF;
using GAF.Operators;

int sizeBoard = 8;
int maxEvaluation = 10000;
int populationSize = 1000;
Population population = new();
Random rand = new(Guid.NewGuid().GetHashCode());

for (int p = 0; p < populationSize; p++)
{
    Chromosome chromosome = new();
    for (int g = 0; g < sizeBoard; g++)
    {
        int pos = rand.Next(0, sizeBoard);
        chromosome.Genes.Add(new(pos));
    }

    population.Solutions.Add(chromosome);
}


GeneticAlgorithm ga = new(population, CalculateFitness);

Elite elite = new(5);
Crossover crossover = new(0.85)
{
    CrossoverType = CrossoverType.DoublePoint
};

SwapMutate mutate = new(0.04);

ga.Operators.Add(elite);
ga.Operators.Add(crossover);
ga.Operators.Add(mutate);

ga.OnGenerationComplete += ga_OnGenerationComplete;
ga.OnRunComplete += ga_OnRunComplete;

ga.Run(Terminate);


Console.WriteLine("End program");


double CalculateFitness(Chromosome chromosome)
{
    int errors = 0;

    var board = chromosome.Genes.Select(s => (int)s.ObjectValue).ToArray();
    for (var q = 0; q < board.Length; q++)
    {
        for (var c = q + 1; c < board.Length; c++)
        {
            // same row
            if (board[q] == board[c])
            {
                errors++;
            }
            else
            {
                // diagonal
                if (System.Math.Abs(c - q) == System.Math.Abs(board[q] - board[c]))
                {
                    errors++;
                }
            }
        }
    }

    return 1 - (double)errors / 100f;
}

int[,] ChromosomeToArray(Chromosome chromosome)
{
    int col = 0;
    var board = new int[sizeBoard, sizeBoard];
    foreach (var gene in chromosome.Genes)
    {
        board[(int)gene.ObjectValue, col] = 1;
        col++;
    }

    return board;
}


void PrintBoard(int[,] board)
{
    for (int row = 0; row < sizeBoard; row++)
    {
        for (int col = 0; col < sizeBoard; col++)
        {
            if (board[row, col] == 1)
            {
                Console.Write(" X ");
            }
            else
            {
                Console.Write(" O ");
            }
        }
        Console.WriteLine();
    }
}


bool Terminate(Population population, int currentGeneration, long currentEvaluation) => population.GetTop(1)[0].Fitness == 1 || currentGeneration > maxEvaluation;

void ga_OnRunComplete(object sender, GaEventArgs e)
{
    var fitness = e.Population.GetTop(1)[0];
    var board = ChromosomeToArray(fitness);
    PrintBoard(board);

    Console.WriteLine();
    Console.WriteLine($"fitness: {fitness.Fitness} Generation: {e.Generation}");
}

void ga_OnGenerationComplete(object sender, GaEventArgs e) => Console.Write("");
