using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public delegate double ToSolve(double[] values);

    public class GenAlgorithm
    {
        /// <summary>
        /// 교차율
        /// </summary>
        public double CrossoverRate { get; private set; }

        /// <summary>
        /// 변이율
        /// </summary>
        public double MutationRate { get; private set; }

        /// <summary>
        /// 개체군 크기
        /// </summary>
        public int PopulationSize { get; private set; }

        /// <summary>
        /// 세대 수
        /// </summary>
        public int GenerationSize { get; private set; }

        /// <summary>
        /// 적합도
        /// </summary>
        public double Fitness { get; private set; }

        /// <summary>
        /// 엘리티즘
        /// </summary>
        public bool Elitism { get; set; }

        /// <summary>
        /// 유전체 내부 유전자 개수
        /// </summary>
        public int GeneCount { get; private set; }

        /// <summary>
        /// 적합도 함수
        /// </summary>
        public ToSolve FitnessFunction { get; set; }

        /// <summary>
        /// 유전체 리스트 (적합도 오름차순 정렬됨)
        /// </summary>
        public List<Genome> Genomes { get; private set; }

        private Random _rand;
        private List<Genome> _nextGeneration;
        private List<double> _selectionPressure;
        private double _totalFitness;

        private double _debug;

        /// <summary>
        /// 생성자
        /// 변이율 5%, 교차율 80%, 개체군 크기 100, 세대수 2000 값 사용
        /// </summary>
        public GenAlgorithm()
        {
            InitInstance();
                        
            CrossoverRate = 0.8;
            MutationRate = 0.05;
            PopulationSize = 100;
            GenerationSize = 2000;
            Fitness = 0;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="crossoverRate">교차율</param>
        /// <param name="mutationRate">변이율</param>
        /// <param name="populationSize">개체군 크기</param>
        /// <param name="generationSize">세대수</param>
        /// <param name="geneCount">유전체 내 유전자 개수</param>
        public GenAlgorithm(double crossoverRate, double mutationRate, int populationSize, int generationSize, int geneCount)
        {
            InitInstance();

            CrossoverRate = crossoverRate;
            MutationRate = mutationRate;
            PopulationSize = populationSize;
            GenerationSize = generationSize;
            GeneCount = geneCount;
            Fitness = 0;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="geneCount">유전체 내 유전자 개수</param>
        public GenAlgorithm(int geneCount)
        {
            InitInstance();

            GeneCount = geneCount;
        }

        public void InitInstance()
        {
            Elitism = false;
            _rand = new Random();
        }

        /// <summary>
        /// 유전 알고리즘 실행
        /// </summary>
        public void Run()
        {
            if (FitnessFunction == null)
                throw new ArgumentException("적합도 함수가 필요합니다.");

            if (GeneCount <= 0)
                throw new ArgumentException("유전체 크기가 설정되지 않았습니다.");

            Genomes = new List<Genome>(PopulationSize);
            _nextGeneration = new List<Genome>(PopulationSize);
            _selectionPressure = new List<double>(PopulationSize);
            Genome.MutationRate = MutationRate;

            CreateGenomes();
            RankPopulation();
            Debug.WriteLine("Initial generation's maximum fitness :\t {0}", Genomes[PopulationSize - 1].Fitness);

            for (int i = 0; i < GenerationSize; i++)
            {
                CreateNextGeneration();
                RankPopulation();

                if (_debug > Genomes[PopulationSize - 1].Fitness)
                    Debug.WriteLine("Fitness down!!!!");

                Debug.WriteLine("{0} generation's maximum fitness :\t {1}", i + 1, Genomes[PopulationSize - 1].Fitness);
                _debug = Genomes[PopulationSize - 1].Fitness;
            }

            Debug.WriteLine("==================================");
            Debug.WriteLine("Complete!");
        }

        private void CreateGenomes()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Genomes.Add(new Genome(GeneCount));
            }
        }

        private void RankPopulation()
        {
            _totalFitness = 0;
            for (int i = 0; i < PopulationSize; i++)
            {
                Genome genome = Genomes[i];
                genome.Fitness = FitnessFunction(genome.Genes);
                _totalFitness += genome.Fitness;
            }

            // 오름차순 정렬
            Genomes.Sort(new GenomeComparer());

            double fitness = 0.0;
            _selectionPressure.Clear();
            for (int i = 0; i < PopulationSize; i++)
            {
                fitness += Genomes[i].Fitness;
                _selectionPressure.Add(fitness);
            }
        }

        private void CreateNextGeneration()
        {
            _nextGeneration.Clear();
            Genome eliteGene = null;
            if (Elitism)
                eliteGene = Genomes[PopulationSize - 1];

            for (int i = 0; i < PopulationSize; i += 2)
            {
                int parentIdx1 = RouletteSelection();
                int parentIdx2 = parentIdx1;
                while (parentIdx1 == parentIdx2)
                {
                    parentIdx2 = RouletteSelection();
                }

                Genome parent1, parent2, child1, child2;
                parent1 = Genomes[parentIdx1];
                parent2 = Genomes[parentIdx2];

                if (_rand.NextDouble() < CrossoverRate)
                    parent1.Crossover(parent2, out child1, out child2);
                else
                {
                    child1 = (Genome)parent1.Clone();
                    child2 = (Genome)parent2.Clone();
                }

                child1.Mutate();
                child2.Mutate();

                _nextGeneration.Add(child1);
                _nextGeneration.Add(child2);
            }

            if (Elitism && eliteGene != null)
                _nextGeneration[PopulationSize - 1] = (Genome)eliteGene.Clone();

            Genomes.Clear();
            for (int i = 0; i < PopulationSize; i++)
                Genomes.Add(_nextGeneration[i]);
        }

        private int RouletteSelection()
        {
            double randomFitness = _rand.NextDouble() * _totalFitness;
            int idx = -1;
            int first = 0;
            int last = PopulationSize - 1;
            int mid = (last - first) / 2;

            while (idx == -1 && first <= last)
            {
                if (randomFitness < _selectionPressure[mid])
                    last = mid;
                else if (randomFitness > _selectionPressure[mid])
                    first = mid;

                mid = (first + last) / 2;

                if (last - first == 1)
                    idx = last;
            }

            return idx;
        }

        /// <summary>
        /// 최우수 유전체
        /// </summary>
        public Genome BestGenome
        {
            get { return Genomes[PopulationSize - 1]; }
        }

        /// <summary>
        /// 최열등 유전체
        /// </summary>
        public Genome WorstGenome
        {
            get { return Genomes[0]; }
        }
    }
}
