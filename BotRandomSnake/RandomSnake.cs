using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Xml.Linq;
using PluginInterface;

namespace BotRandomSnake
{
    public class RandomSnake : ISmartSnake
    {
        public Move Direction { get; set; }
        public bool Reverse { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int stat = 0;

        public bool add = true;
        int MapWidht = 0;
        int MapHeight = 0;
        int step = 0;
        List<Point> stonesl;


        int[,] cMap = new int[100, 100];


        private DateTime dt = DateTime.Now;
        private static Random rnd = new Random();

        public void Startup(Size size, List<Point> stones)
        {
            Name = "BotRandomSnake";
            Color = Color.Brown;

            int MapWidht = size.Width;
            int MapHeight = size.Height;
            List<Point> stonesl = stones;

            //Console.WriteLine($"{size.Height}, {size.Height}");

         int k = 0;
            //Алгоритм создания карты для Волнового алгоритма

            for (k=0; k<stones.Count; k++) { 
                for (int i = 0; i < size.Width; i++) {
                    for (int j = 0; j < size.Height; j++)
                    {
                    
                        if (stones[k].X == j && stones[k].Y == i)
                        {
                            cMap[i,j] = -2;
                        }

                    }
                }
            }


            for (int i = 0; i < size.Width; i++)
            {
              for (int j = 0; j < size.Height; j++)
                {
                    if (cMap[i,j] == 0)
                    {
                        cMap[i, j] = -1;
                    }
                    //Console.Write("{0}\t", cMap[i, j]);
                }
                //Console.WriteLine();
            }

            this.MapWidht = size.Width;
            this.MapHeight = size.Height;
            this.cMap = cMap;
            this.stonesl = stones;
        }

        

        public void Update(Snake snake, List<Snake> enemies, List<Point> food, List<Point> dead)
        {

            // Находим ближайшую еду
            Point closestFood = GetClosestFood(snake.Position, food);

            //Console.WriteLine(closestFood);


            //var prevPos = snake.Position;

            cMap[closestFood.Y, closestFood.X] = 0;
            while (add == true)
            {
                add = false;
                for (int y = 0; y < MapWidht; y++)
                    for (int x = 0; x < MapHeight; x++)
                    {
                        if (cMap[x, y] == step)
                        {
                            //Ставим значение шага+1 в соседние ячейки (если они проходимы)
                            if (y - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
                                cMap[x - 1, y] = step + 1;
                            if (x - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
                                cMap[x, y - 1] = step + 1;
                            if (y + 1 < MapWidht && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;
                            if (x + 1 < MapHeight && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
                                cMap[x, y + 1] = step + 1;
                        }
                    }
                step++;

                Console.WriteLine(step);
                add = true;
                
                if (cMap[snake.Position.Y, snake.Position.X] != -1)
                {
                    add = false;               
                }

                if (step > MapWidht * MapHeight) //решение не найдено
                    add = false;
            }

            if (closestFood != snake.Position)
            {
                if (cMap[snake.Position.Y, snake.Position.X + 1] != -2 && cMap[snake.Position.Y, snake.Position.X + 1] != -1 && cMap[snake.Position.Y, snake.Position.X + 1] < cMap[snake.Position.Y, snake.Position.X])
                {
                    if (snake.Position.X + 1 == snake.Tail[0].X)
                    {
                        Reverse = true;
                    }
                    Direction = Move.Right;
                    Console.WriteLine("Режим ПРАВО");

                }

                else if (cMap[snake.Position.Y, snake.Position.X - 1] != -2 && cMap[snake.Position.Y, snake.Position.X - 1] != -1 && cMap[snake.Position.Y, snake.Position.X - 1] < cMap[snake.Position.Y, snake.Position.X])
                {
                    if (snake.Position.X - 1 == snake.Tail[0].X)
                    {
                        Reverse = true;
                    }
                    Direction = Move.Left;
                    Console.WriteLine("Режим ЛЕВО");

                }

                else if (cMap[snake.Position.Y + 1, snake.Position.X] != -2 && cMap[snake.Position.Y + 1, snake.Position.X] != -1 && cMap[snake.Position.Y + 1, snake.Position.X] < cMap[snake.Position.Y, snake.Position.X])
                {
                    if (snake.Position.Y + 1 == snake.Tail[0].Y)
                    {
                        Reverse = true;
                    }
                    Direction = Move.Down;
                    Console.WriteLine("Режим НИЗ");

                }

                else if (cMap[snake.Position.Y - 1, snake.Position.X] != -2 && cMap[snake.Position.Y - 1, snake.Position.X] != -1 && cMap[snake.Position.Y - 1, snake.Position.X] < cMap[snake.Position.Y, snake.Position.X])
                {
                    if (snake.Position.Y - 1 == snake.Tail[0].Y)
                    {
                        Reverse = true;
                    }
                    Direction = Move.Up;
                    Console.WriteLine("Режим ВЕРХ");
                }

                else if (closestFood.X == snake.Position.X)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 1");
                }

                else if (closestFood.Y == snake.Position.Y)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 2");
                }
                

                //Проверка лево право верх

                else if (cMap[snake.Position.X - 1, snake.Position.Y] < 0 && cMap[snake.Position.X + 1, snake.Position.Y] < 0 && cMap[snake.Position.X, snake.Position.Y - 1] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                /*//Проверка лево право низ

                else if (cMap[snake.Position.X - 1, snake.Position.Y] < 0 && cMap[snake.Position.X + 1, snake.Position.Y] < 0 && cMap[snake.Position.X, snake.Position.Y + 1] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                //Проверка лево низ верх

                else if (cMap[snake.Position.X - 1, snake.Position.Y] < 0 && cMap[snake.Position.X, snake.Position.Y + 1] < 0 && cMap[snake.Position.X, snake.Position.Y - 1] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                //Проверка право них верх

                else if (cMap[snake.Position.X + 1, snake.Position.Y] < 0 && cMap[snake.Position.X, snake.Position.Y + 1] < 0 && cMap[snake.Position.X, snake.Position.Y - 1] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                //Проверка лево право верх

                else if (cMap[snake.Position.Y, snake.Position.X - 1] < 0 && cMap[snake.Position.Y, snake.Position.X + 1] < 0 && cMap[snake.Position.Y - 1, snake.Position.X] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }*/

                //Проверка лево право низ

                else if (cMap[snake.Position.Y, snake.Position.X - 1] < 0 && cMap[snake.Position.Y, snake.Position.X + 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                //Проверка лево низ верх

                else if (cMap[snake.Position.Y, snake.Position.X - 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0 && cMap[snake.Position.Y - 1, snake.Position.X] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

                //Проверка право них верх

                else if (cMap[snake.Position.Y, snake.Position.X + 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0 && cMap[snake.Position.Y - 1, snake.Position.X] < 0)
                {
                    Reverse = true;
                    Console.WriteLine("Режим 3");
                }

            }

            /*if (cMap[snake.Position.Y, snake.Position.X] == 0)
            {
                Console.WriteLine("Ура");
            }*/


            if (cMap[snake.Position.Y, snake.Position.X] == 0)
                {
                Console.WriteLine("Ура");
                step = 0;
                add = true; 
                int k = 0;
                //Алгоритм создания карты для Волнового алгоритма



                for (k = 0; k < stonesl.Count; k++)
                {
                    for (int i = 0; i < MapWidht; i++)
                    {
                        for (int j = 0; j < MapHeight; j++)
                        {

                            if (stonesl[k].X == j && stonesl[k].Y == i)
                            {
                                cMap[i, j] = -2;
                            }

                        }
                    }
                }


                for (int i = 0; i < MapWidht; i++)
                {
                    for (int j = 0; j < MapHeight; j++)
                    {
                        if (cMap[i, j] != -2)
                        {
                            cMap[i, j] = -1;
                        }
                        //Console.Write("{0}\t", cMap[i, j]);
                    }
                    //Console.WriteLine();
                }

                cMap[closestFood.Y, closestFood.X] = 0;
                while (add == true)
                {
                    add = false;
                    for (int y = 0; y < MapWidht; y++)
                        for (int x = 0; x < MapHeight; x++)
                        {
                            if (cMap[x, y] == step)
                            {
                                //Ставим значение шага+1 в соседние ячейки (если они проходимы)
                                if (y - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
                                    cMap[x - 1, y] = step + 1;
                                if (x - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
                                    cMap[x, y - 1] = step + 1;
                                if (y + 1 < MapWidht && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
                                    cMap[x + 1, y] = step + 1;
                                if (x + 1 < MapHeight && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
                                    cMap[x, y + 1] = step + 1;

                            }
                            
                        }
                    step++;
                    add = true;
                    
                    if (cMap[snake.Position.Y, snake.Position.X] != -1)
                    {

                        add = false;
                    }
                    
                        
                    if (step > MapWidht * MapHeight) //решение не найдено
                        add = false;
                }

                if (closestFood != snake.Position)
                {
                    if (cMap[snake.Position.Y, snake.Position.X + 1] != -2 && cMap[snake.Position.Y, snake.Position.X + 1] != -1 && cMap[snake.Position.Y, snake.Position.X + 1] < cMap[snake.Position.Y, snake.Position.X])
                    {
                        if (snake.Position.X + 1 == snake.Tail[0].X)
                        {
                            Reverse = true;
                        }
                        Direction = Move.Right;
                        Console.WriteLine("Режим ПРАВО");

                    }

                    else if (cMap[snake.Position.Y, snake.Position.X - 1] != -2 && cMap[snake.Position.Y, snake.Position.X - 1] != -1 && cMap[snake.Position.Y, snake.Position.X - 1] < cMap[snake.Position.Y, snake.Position.X])
                    {
                        if (snake.Position.X - 1 == snake.Tail[0].X)
                        {
                            Reverse = true;
                        }
                        Direction = Move.Left;
                        Console.WriteLine("Режим ЛЕВО");

                    }

                    else if (cMap[snake.Position.Y + 1, snake.Position.X] != -2 && cMap[snake.Position.Y + 1, snake.Position.X] != -1 && cMap[snake.Position.Y + 1, snake.Position.X] < cMap[snake.Position.Y, snake.Position.X])
                    {
                        if (snake.Position.Y + 1 == snake.Tail[0].Y)
                        {
                            Reverse = true;
                        }
                        Direction = Move.Down;
                        Console.WriteLine("Режим НИЗ");

                    }

                    else if (cMap[snake.Position.Y - 1, snake.Position.X] != -2 && cMap[snake.Position.Y - 1, snake.Position.X] != -1 && cMap[snake.Position.Y - 1, snake.Position.X] < cMap[snake.Position.Y, snake.Position.X])
                    {
                        if (snake.Position.Y - 1 == snake.Tail[0].Y)
                        {
                            Reverse = true;
                        }
                        Direction = Move.Up;
                        Console.WriteLine("Режим ВЕРХ");
                    }

                    else if (closestFood.X == snake.Position.X)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 1");
                    }

                    else if (closestFood.Y == snake.Position.Y)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 2");
                    }

                    //Проверка лево право верх

                    else if (cMap[snake.Position.Y , snake.Position.X- 1] < 0 && cMap[snake.Position.Y , snake.Position.X+ 1] < 0 && cMap[snake.Position.Y- 1, snake.Position.X ] < 0)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 3");
                    }

                    //Проверка лево право низ

                    else if (cMap[snake.Position.Y, snake.Position.X-1] < 0 && cMap[snake.Position.Y, snake.Position.X + 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 3");
                    }

                    //Проверка лево низ верх

                    else if (cMap[snake.Position.Y, snake.Position.X - 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0 && cMap[snake.Position.Y - 1, snake.Position.X] < 0)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 3");
                    }

                    //Проверка право них верх

                    else if (cMap[snake.Position.Y, snake.Position.X + 1] < 0 && cMap[snake.Position.Y + 1, snake.Position.X] < 0 && cMap[snake.Position.Y - 1, snake.Position.X] < 0)
                    {
                        Reverse = true;
                        Console.WriteLine("Режим 3");
                    }
                }

                for (int i = 0; i < MapWidht; i++)
                {
                    for (int j = 0; j < MapHeight; j++)
                    {
                        Console.Write("{0}\t", cMap[i, j]);
                    }
                    Console.WriteLine();
                }
            }


            
                
            
            //Вывод карты

            /*for (int i = 0; i < MapWidht; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    Console.Write("{0}\t", cMap[i, j]);
                }
                Console.WriteLine();
            } */


        } 

        // Метод для нахождения ближайшей еды
        private Point GetClosestFood(Point position, List<Point> food)
        {
            if (food.Count == 0)
            {
                return Point.Empty;
            }

            Point closestFood = food[0];
            double closestDistance = GetDistance(position, closestFood);

            foreach (Point f in food)
            {
                double distance = GetDistance(position, f);
                if (distance < closestDistance)
                {
                    closestFood = f;
                    closestDistance = distance;
                }
            }
            return closestFood;
        }

        //вычисление расстояния между двумя точками
        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}