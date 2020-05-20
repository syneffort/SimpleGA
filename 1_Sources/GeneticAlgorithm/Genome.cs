using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class Genome : ICloneable
    {
        /// <summary>
        /// 유전자 크기
        /// </summary>
        public int Length { get; private set; }
        
        /// <summary>
        /// 유전자
        /// </summary>
        public double[] Genes { get; private set; }
        
        /// <summary>
        /// 변이율
        /// </summary>
        public static double MutationRate { get; set; }
        
        /// <summary>
        /// 적합도
        /// </summary>
        public double Fitness { get; set; }

        private static Random _rand = new Random();

        public Genome(int length)
        {
            Length = length;
            Genes = new double[length];
            CreateGenes();
        }

        public Genome(int length, bool createGene)
        {
            Length = length;
            Genes = new double[length];

            if (createGene)
                CreateGenes();
        }

        public Genome(ref double[] genes)
        {
            Length = genes.Length;
            Genes = new double[Length];

            for (int i = 0; i < Length; i++)
                Genes[i] = genes[i];
        }

        public object Clone()
        {
            Genome cloned = new Genome(Length);
            cloned.Length = this.Length;
            cloned.Genes = this.Genes;
            cloned.Fitness = this.Fitness;

            return cloned;
        }

        private void CreateGenes()
        {
            for (int i = 0; i < Length; i++)
            {
                Genes[i] = _rand.NextDouble();
            }
        }

        /// <summary>
        /// 유전체 교차
        /// </summary>
        /// <param name="genome2">교차 대상 유전체</param>
        /// <param name="child1">자식 유전체1</param>
        /// <param name="child2">자식 유전체2</param>
        public void Crossover(Genome genome2, out Genome child1, out Genome child2)
        {
            child1 = new Genome(Length, false);
            child2 = new Genome(Length, false);

            int pos = (int)(_rand.NextDouble() * (double)Length);
            for (int i = 0; i < Length; i++)
            {
                if (i < pos)
                {
                    child1.Genes[i] = Genes[i];
                    child2.Genes[i] = genome2.Genes[i];
                }
                else
                {
                    child1.Genes[i] = genome2.Genes[i];
                    child2.Genes[i] = Genes[i];
                }
            }
        }

        /// <summary>
        /// 유전체 변이
        /// </summary>
        public void Mutate()
        {
            for (int i = 0; i < Length; i++)
            {
                if (_rand.NextDouble() < MutationRate)
                    Genes[i] = (Genes[i] + _rand.NextDouble()) / 2.0;
            }
        }
    }
}
