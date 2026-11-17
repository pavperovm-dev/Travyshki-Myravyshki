using System;

namespace TravushkiMyravushki
{
    class Program
    {
        // ===== Константы поля и героя =====
        const int FieldWidth = 10;
        const int FieldHeight = 6;
        const int MaxHealth = 20;
        const int HealAmount = 5;          // сколько HP даёт одна травушка
        const int MaxWeaponLevel = 3;      // уровни оружия: 0, 1, 2, 3
        const int PercentFull = 100;
        const int LowHealthPercent = 30;   // ниже - муравейSON сильно ранен
        const int MediumHealthPercent = 60;

        // ===== Враги и очки =====
        const int BeetleHealth = 6;
        const int BeetleDamage = 2;
        const int SpiderHealth = 8;
        const int SpiderDamage = 4;
        const int HerbScore = 10;
        const int KillScore = 15;

        // ===== Символы на поле =====
        const char Grass = '.';
        const char Herb = '*';
        const char Beetle = 'B';
        const char Spider = 'S';
        const char Weapon = '!';
        const char Home = 'H';
        const char AntSymbol = 'M';

        // Названия и урон оружия по уровням
        static readonly string[] WeaponNames = { "рпг", "ножик", "клык", "автомат черный" };
        static readonly int[] WeaponDamages = { 2, 3, 4, 6 };

        // Стартовая карта луга (10 символов в каждой строке)
        static readonly string[] StartMap =
        {
            "H.*....!..",
            "...B...*..",
            ".*....S...",
            "....!...B.",
            ".*...*....",
            "...S...*.*"
        };

        static char[,] field = new char[FieldHeight, FieldWidth];

        // ===== Состояние игры =====
        static int antX = 0;
        static int antY = 0;
        static int antHealth = MaxHealth;
        static int herbsCollected = 0;
        static int weaponLevel = 0;
        static int score = 0;
        static int turn = 0;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            int herbsGoal;
            int maxTurns;

            Console.WriteLine("=== ТРАВУШКИ-МУРАВУШКИ ===");
            Console.WriteLine("Ты муравейSON. Собери травушку (*) и принеси в логово (H) до кровавой зимы!");
            Console.Write("Сложность (легко / средне / сложно): ");

            // switch: одно значение сравниваем с набором констант
            switch ((Console.ReadLine() ?? "").Trim().ToLower())
            {
                case "легко":
                case "лёгко":
                    herbsGoal = 3; maxTurns = 60; break;
                case "сложно":
                    herbsGoal = 6; maxTurns = 50; break;
                default:
                    herbsGoal = 5; maxTurns = 55; break; // средняя сложность
            }

            LoadField();
            bool wantsExit = false;

            // главный цикл: идём, пока нет победы, поражения, кровавойзимы или выхода
            while (!wantsExit && antHealth > 0 && turn < maxTurns && !IsVictory(herbsGoal))
            {
                DrawField();
                PrintStatus(herbsGoal, maxTurns);
                Console.Write("Команда (w/a/s/d - идти, h - покушать, q - выход): ");
                string command = (Console.ReadLine() ?? "").Trim().ToLower();

                switch (command)
                {
                    case "w": MoveAnt(0, -1); break;
                    case "s": MoveAnt(0, 1); break;
                    case "a": MoveAnt(-1, 0); break;
                    case "d": MoveAnt(1, 0); break;
                    case "h": HealAnt(); break;
                    case "q":
                    case "exit": wantsExit = true; break;
                    default: Console.WriteLine("Такой команды нет, муравейSON запутался."); break;
                }
            }

            // итог партии
            Console.WriteLine();
            if (IsVictory(herbsGoal)) Console.WriteLine("ПОБЕДА! Логово сыто, кровавая зима не страшна!");
            else if (antHealth <= 0) Console.WriteLine("ПОРАЖЕНИЕ. МуравейSON погиб в бою...");
            else if (turn >= maxTurns) Console.WriteLine("ПОРАЖЕНИЕ. Пришла кровавая зима, ходы кончились.");
            else Console.WriteLine("МуравейSON ушёл далеко. До встречи!");
            Console.WriteLine($"Итоговый счёт: {score}");
        }

        // Победа: травушек хватает, и муравейSON стоит дома
        static bool IsVictory(int herbsGoal)
        {
            return antHealth > 0 && herbsCollected >= herbsGoal && antX == 0 && antY == 0;
        }

        // Копируем карту из строк в двумерный массив
        static void LoadField()
        {
            for (int y = 0; y < FieldHeight; y++)
                for (int x = 0; x < FieldWidth; x++)
                    field[y, x] = StartMap[y][x];
        }

        // Рисуем поле вложенными циклами
        static void DrawField()
        {
            Console.WriteLine();
            for (int y = 0; y < FieldHeight; y++)
            {
                string row = "";
                for (int x = 0; x < FieldWidth; x++)
                {
                    char cell = (x == antX && y == antY) ? AntSymbol : field[y, x];
                    row += cell + " ";
                }
                Console.WriteLine(row);
            }
        }

        // Строка состояния + настроение по проценту здоровья
        static void PrintStatus(int herbsGoal, int maxTurns)
        {
            int percent = antHealth * PercentFull / MaxHealth;
            string mood;
            if (percent < LowHealthPercent) mood = "СИЛЬНО РАНЕН!";
            else if (percent < MediumHealthPercent) mood = "потрёпан";
            else mood = "живчик";

            Console.WriteLine($"HP {antHealth}/{MaxHealth} ({mood}) | Оружие: {WeaponNames[weaponLevel]} | Травинки: {herbsCollected}/{herbsGoal} | Ход: {turn}/{maxTurns} | Счёт: {score}");
        }

        // Шаг муравьяSON; границы проверяются отдельными условиями
        static void MoveAnt(int dx, int dy)
        {
            int newX = antX + dx;
            int newY = antY + dy;

            if (newX < 0) { Console.WriteLine("Левый край МИРА!"); return; }
            if (newX >= FieldWidth) { Console.WriteLine("Правый край МИРА!"); return; }
            if (newY < 0) { Console.WriteLine("Верхний край МИРА!"); return; }
            if (newY >= FieldHeight) { Console.WriteLine("Нижний край МИРА!"); return; }

            antX = newX;
            antY = newY;
            turn++;

            // что лежит в клетке?
            switch (field[antY, antX])
            {
                case Herb:
                    herbsCollected++;
                    score += HerbScore;
                    field[antY, antX] = Grass;
                    Console.WriteLine("Нашёл травушку!");
                    break;
                case Weapon:
                    if (weaponLevel < MaxWeaponLevel) weaponLevel++;
                    field[antY, antX] = Grass;
                    Console.WriteLine($"Новое оружие: {WeaponNames[weaponLevel]}!");
                    break;
                case Beetle:
                    Fight(Beetle);
                    break;
                case Spider:
                    Fight(Spider);
                    break;
                default:
                    break;
            }
        }

        // Бой по раундам: муравейSON бьёт первым
        static void Fight(char enemy)
        {
            string enemyName = enemy == Beetle ? "жукSON" : "паукSON";
            int enemyHealth = enemy == Beetle ? BeetleHealth : SpiderHealth;
            int enemyDamage = enemy == Beetle ? BeetleDamage : SpiderDamage;
            int antDamage = WeaponDamages[weaponLevel];

            Console.WriteLine($"БОЙ с ОППОНЕНТОМ: {enemyName}! Оружие: {WeaponNames[weaponLevel]} (урон {antDamage}).");
            while (enemyHealth > 0 && antHealth > 0)
            {
                enemyHealth -= antDamage;                   // удар муравья
                if (enemyHealth > 0) antHealth -= enemyDamage; // ответ врага
            }

            if (antHealth < 0) antHealth = 0; // здоровье не уходит в минус

            if (antHealth > 0)
            {
                score += KillScore;
                field[antY, antX] = Grass;
                Console.WriteLine($"Ты победил: {enemyName}! Осталось HP: {antHealth}");
            }
            else Console.WriteLine($"{enemyName} оказался сильнее...");
        }

        // Лечение: тратим травушку, получаем здоровье
        static void HealAnt()
        {
            if (herbsCollected <= 0) Console.WriteLine("Нет травушек - нечем лечиться.");
            else if (antHealth >= MaxHealth) Console.WriteLine("Ты и так здоров.");
            else
            {
                herbsCollected--;
                antHealth += HealAmount;
                if (antHealth > MaxHealth) antHealth = MaxHealth; // верхняя граница
                turn++;
                Console.WriteLine($"КУСЬ-КУСЬ! HP теперь {antHealth}.");
            }
        }
    }
}
