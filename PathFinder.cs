﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
//using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Collections.ObjectModel;

namespace test02
{
    class PathFinder
    {
        public class Cell
        {
            // Координаты точки на карте.
            public Objects.Vector2D position;
            // Длина пути от старта (G).
            public int PathLengthFromStart { get; set; }
            // Точка, из которой пришли в эту точку.
            public Cell cameFrom;
            // Примерное расстояние до цели (H).
            public int HeuristicEstimatePathLength { get; set; }
            // Ожидаемое полное расстояние до цели (F).
            public int EstimateFullPathLength{ get { return this.PathLengthFromStart + this.HeuristicEstimatePathLength; } }
        }

        public static List<Objects.Vector2D> GetPath(int[,] field, Objects.Vector2D start, Objects.Vector2D goal)
        {
            var closedSet = new Collection<Cell>();
            var openSet = new Collection<Cell>();
            Cell startCell = new Cell()
            {
                position = start,
                cameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
            };
            openSet.Add(startCell);
            while (openSet.Count > 0)
            {
                var currentCell = openSet.OrderBy(Cell =>
                  Cell.EstimateFullPathLength).First();
                // Шаг 4.
                if (currentCell.position == goal)
                    return GetPathForCell(currentCell);
                openSet.Remove(currentCell);
                closedSet.Add(currentCell);
                foreach (var neighbourCell in GetNeighbours(currentCell, goal, field))
                {
                    if (closedSet.Count(Cell => Cell.position == neighbourCell.position) > 0)
                        continue;
                    var openCell = openSet.FirstOrDefault(Cell =>
                      Cell.position == neighbourCell.position);
                    if (openCell == null)
                        openSet.Add(neighbourCell);
                    else
                      if (openCell.PathLengthFromStart > neighbourCell.PathLengthFromStart)
                    {
                        openCell.cameFrom = currentCell;
                        openCell.PathLengthFromStart = neighbourCell.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        private static int GetDistanceBetweenNeighbours()
        {
            return 1;
        }

        private static int GetHeuristicPathLength(Objects.Vector2D from, Objects.Vector2D to)
        {
            return Convert.ToInt32(Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y));
        }

        private static Collection<Cell> GetNeighbours(Cell Cell,
          Objects.Vector2D goal, int[,] field)
        {
            var result = new Collection<Cell>();

            // Соседними точками являются соседние по стороне клетки.
            Objects.Vector2D[] Vectors = new Objects.Vector2D[4];
            Vectors[0] = new Objects.Vector2D(Cell.position.x + 1, Cell.position.y);
            Vectors[1] = new Objects.Vector2D(Cell.position.x - 1, Cell.position.y);
            Vectors[2] = new Objects.Vector2D(Cell.position.x, Cell.position.y + 1);
            Vectors[3] = new Objects.Vector2D(Cell.position.x, Cell.position.y - 1);

            foreach (var vector in Vectors)
            {
                // Проверяем, что не вышли за границы карты.
                if (vector.x < 0 || vector.x >= field.GetLength(0))
                    continue;
                if (vector.y < 0 || vector.y >= field.GetLength(1))
                    continue;
                // Проверяем, что по клетке можно ходить.
                if ((field[Convert.ToInt32(vector.x), Convert.ToInt32(vector.y)] != 0) && (field[Convert.ToInt32(vector.x), Convert.ToInt32(vector.y)] != 1))
                    continue;
                // Заполняем данные для точки маршрута.
                var neighbourCell = new Cell()
                {
                    position = vector,
                    cameFrom = Cell,
                    PathLengthFromStart = Cell.PathLengthFromStart +
                    GetDistanceBetweenNeighbours(),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(vector, goal)
                };
                result.Add(neighbourCell);
            }
            return result;
        }

        private static List<Objects.Vector2D> GetPathForCell(Cell Cell)
        {
            var result = new List<Objects.Vector2D>();
            var currentCell = Cell;
            while (currentCell != null)
            {
                result.Add(currentCell.position);
                currentCell = currentCell.cameFrom;
            }
            result.Reverse();
            return result;
        }
    }
}