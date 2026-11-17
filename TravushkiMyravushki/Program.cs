using System;

namespace TravushkiMyravushki
{
    class Program
    {
        // ===== Размер поля (луг) =====
        const int FieldWidth = 10;
        const int FieldHeight = 6;

        // ===== Стартовые значения муравья =====
        const int StartHealth = 20;       // максимум здоровья
        const int StartArmor = 1;         // броня съедает 1 урона
        const int DefaultHerbsGoal = 5;   // цель если игрок ввёл не число

        // ===== Жук и травинка стоят на фиксированных клетках =====
        const int BeetleDamage = 5;       // сколько здоровья отнимает укус жука
        const int BeetleX = 1;
        const int BeetleY = 0;
        const int HerbX = 5;
        const int HerbY = 2;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // состояние муравья
            int antX = 0;
            int antY = 0;
            int antHealth = StartHealth;
            int herbsCollected = 0;
            int herbsGoal;

            Console.WriteLine("=== ТРАВУШКИ-МУРАВУШКИ ===");
            Console.Write("сколько травинок надо собрать? (число): ");

            // TryParse не даёт программе упасть
            if (!int.TryParse(Console.ReadLine(), out herbsGoal) || herbsGoal <= 0)
            {
                Console.WriteLine("это не число блин! беру стандартную цель");
                herbsGoal = DefaultHerbsGoal;
            }

            DrawField(antX, antY);
            PrintStatus(antHealth, herbsCollected, herbsGoal, antX, antY);

            Console.Write("твой ход (w/a/s/d): ");
            string command = (Console.ReadLine() ?? "").Trim().ToLower();

            if (command == "w") antY--;
            if (command == "s") antY++;
            if (command == "a") antX--;
            if (command == "d") antX++;

            // встретили жука считаем урон
            if (antX == BeetleX && antY == BeetleY)
            {
                int damage = BeetleDamage - StartArmor; // формула: урон жука минус броня
                antHealth = antHealth - damage;
                Console.WriteLine($"жук укусил! урон: {damage}");
            }

            // нашли травинку
            if (antX == HerbX && antY == HerbY)
            {
                herbsCollected++;
                Console.WriteLine("нашёл травинку!");
            }

            DrawField(antX, antY);
            PrintStatus(antHealth, herbsCollected, herbsGoal, antX, antY);
            Console.WriteLine("конец. нажми enter.");
            Console.ReadLine();
        }

        // M - муравей B - жук * - травинка . - трава
        static void DrawField(int antX, int antY)
        {
            Console.WriteLine();
            for (int y = 0; y < FieldHeight; y++)
            {
                string row = "";
                for (int x = 0; x < FieldWidth; x++)
                {
                    if (x == antX && y == antY) row += "M ";
                    else if (x == BeetleX && y == BeetleY) row += "B ";
                    else if (x == HerbX && y == HerbY) row += "* ";
                    else row += ". ";
                }
                Console.WriteLine(row);
            }
        }

        // состояния героя
        static void PrintStatus(int health, int herbs, int goal, int x, int y)
        {
            Console.WriteLine($"муравей: HP {health}/{StartHealth} | травинки: {herbs}/{goal} | клетка: ({x},{y})");
        }
    }
}
