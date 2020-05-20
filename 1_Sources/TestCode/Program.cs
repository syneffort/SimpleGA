using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm;

namespace TestCode
{
    class Program
    {
        static double Function(double[] values)
        {
            double x = values[0];
            double y = values[1];

            double c = 9.0;
            return Math.Pow(15 * x * y * (1 - x) * (1 - y) * Math.Sin(c * Math.PI * x) * Math.Cos(c * Math.PI * y), 2);
        }

        static void Main(string[] args)
        {
            GenAlgorithm algorithm = new GenAlgorithm(0.8, 0.3, 1000, 5000, 2);
            algorithm.FitnessFunction = new ToSolve(Function);

            algorithm.Elitism = true;
            algorithm.Run();

            Console.WriteLine("============= Result =============");
            Console.WriteLine();
            Console.WriteLine(string.Format("Best fitness : {0}\t @ x={1}, y={2}", algorithm.BestGenome.Fitness, algorithm.BestGenome.Genes[0], algorithm.BestGenome.Genes[1]));
            Console.WriteLine(string.Format("Worst fitness : {0}\t @ x={1}, y={2}", algorithm.WorstGenome.Fitness, algorithm.WorstGenome.Genes[0], algorithm.WorstGenome.Genes[1]));

            Console.ReadKey();
        }
    }
}
