using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GenomeComparer : IComparer<Genome>
    {
        public GenomeComparer()
        {

        }

        /// <summary>
        /// 유전체 비교
        /// </summary>
        /// <param name="x">유전체 x</param>
        /// <param name="y">유전체 y</param>
        /// <returns></returns>
        public int Compare(Genome x, Genome y)
        {
            if (x.Fitness > y.Fitness)
                return 1;
            else if (x.Fitness == y.Fitness)
                return 0;
            else
                return -1;
        }
    }
}
