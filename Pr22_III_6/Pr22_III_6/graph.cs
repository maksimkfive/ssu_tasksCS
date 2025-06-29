using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Pr22_III_6
{
    // Класс, представляющий город с координатами и именем.
    public class City
    {
        public string Name; // Имя города
        public double X, Y; // Координаты города

        public City(string name, double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        // Вычисляет евклидово расстояние до другого города.
        public double DistanceTo(City other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class Graph
    {
        public int N { get; private set; } // Количество городов
        public City[] Cities { get; private set; } // Массив городов
        public int[,] Adjacency { get; private set; } // Матрица смежности
        public double[,] Weights { get; private set; } // Матрица расстояний (весов)

        // Чтение графа из файла по новому формату
        public Graph(string filename)
        {
            var lines = File.ReadAllLines(filename);
            N = int.Parse(lines[0]);
            Cities = new City[N];

            // Чтение городов и их координат
            for (int i = 0; i < N; i++)
            {
                var parts = lines[1 + i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string name = parts[0];
                double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                Cities[i] = new City(name, x, y);
            }

            // Чтение матрицы смежности
            Adjacency = new int[N, N];
            int startIndex = 1 + N;
            for (int i = 0; i < N; i++)
            {
                var row = lines[startIndex + i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < N; j++)
                {
                    Adjacency[i, j] = int.Parse(row[j]);
                }
            }

            // Формирование матрицы весов (расстояний)
            Weights = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (Adjacency[i, j] == 1)
                        Weights[i, j] = Cities[i].DistanceTo(Cities[j]);
                    else if (i == j)
                        Weights[i, j] = 0;
                    else
                        Weights[i, j] = double.PositiveInfinity;
                }
            }
        }

        // Алгоритм Флойда — вычисляет кратчайшие расстояния между всеми парами городов
        public double[,] FloydWarshall()
        {
            double[,] dist = new double[N, N];
            Array.Copy(Weights, dist, Weights.Length);

            for (int k = 0; k < N; k++)
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];

            return dist;
        }

        // Печать матрицы расстояний
        public void ShowDistances(double[,] dist)
        {
            Console.WriteLine("Матрица кратчайших расстояний:");
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (double.IsPositiveInfinity(dist[i, j]))
                        Console.Write("  INF ");
                    else
                        Console.Write($"{dist[i, j],7:F2} ");
                }
                Console.WriteLine();
            }
        }

        // Определить, между какими городами нужно построить дорогу,
        // чтобы расстояние между любыми городами не превышало maxDistance
        public void FindRoadsToBuild(double maxDistance)
        {
            var dist = FloydWarshall();
            bool found = false;
            var teempdist = new double[N, N];

            // Перебираем все пары городов, между которыми нет дороги
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (Adjacency[i, j] == 0)
                    {
                        // Пробуем добавить дорогу между i и j
                        double[,] tempWeights = (double[,])Weights.Clone();
                        double d = Cities[i].DistanceTo(Cities[j]);
                        tempWeights[i, j] = d;
                        tempWeights[j, i] = d;

                        // Пересчитываем кратчайшие расстояния
                        double[,] tempDist = new double[N, N];
                        Array.Copy(tempWeights, tempDist, tempWeights.Length);

                        for (int k = 0; k < N; k++)
                            for (int a = 0; a < N; a++)
                                for (int b = 0; b < N; b++)
                                    if (tempDist[a, k] + tempDist[k, b] < tempDist[a, b])
                                        tempDist[a, b] = tempDist[a, k] + tempDist[k, b];
                        teempdist = tempDist;
                        // Проверяем, не превышает ли расстояние между любыми городами maxDistance
                        bool allWithinLimit = true;
                        for (int a = 0; a < N && allWithinLimit; a++)
                            for (int b = 0; b < N; b++)
                                if (tempDist[a, b] > maxDistance)
                                {
                                    allWithinLimit = false;
                                    break;
                                }

                        if (allWithinLimit)
                        {
                            Console.WriteLine($"Если построить дорогу между {Cities[i].Name} и {Cities[j].Name},");
                            Console.WriteLine($"то расстояние между любыми городами не превысит {maxDistance} км.");
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Нет такой пары городов, между которыми можно построить одну дорогу, чтобы условие выполнялось.");
            }
            Console.WriteLine("Матрица расстояний после добавления дороги:");
            for (int i = 0; i < teempdist.GetLength(0); i++)
            {
                for (int j = 0; j < teempdist.GetLength(0); j++)
                {
                    if (double.IsPositiveInfinity(teempdist[i, j]))
                        Console.Write("  INF ");
                    else
                        Console.Write($"{teempdist[i, j],7:F2} ");
                }
                Console.WriteLine();
            }
        }

        // Печать исходной матрицы смежности
        public void ShowAdjacency()
        {
            Console.WriteLine("Матрица смежности:");
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                    Console.Write($"{Adjacency[i, j],2}");
                Console.WriteLine();
            }
        }
    }
}
