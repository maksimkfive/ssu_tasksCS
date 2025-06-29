/*
 * Во входном файле задается: в первой сторке N – количество городов; начиная со
 * второй строки через пробел названия N‑городов и их координаты в декартовой системе;
 * с новой строки матрица смежности графа, описывающая схему дорог (вес ребра
 * рассчитывается по координатам городов).
 *
 * 6. Определите, между какими городами нужно построить дорогу, чтобы расстояние
 * между любыми городами не превышало N км.
 */

using System;

namespace Pr22_III_6
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph("graph.txt");

            Console.Write("Введите максимальное допустимое расстояние между любыми городами (N): ");
            if (!double.TryParse(Console.ReadLine(), out double maxDistance))
            {
                Console.WriteLine("Некорректный ввод числа.");
                return;
            }

            graph.FindRoadsToBuild(maxDistance);
        }
    }
}
