using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

public class Monitor
{
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public double Diagonal { get; set; }
    public string Resolution { get; set; }
    public double Cost { get; set; }
    public string MonitorType { get; set; } 

    public virtual double CalculatePurchaseCost()
    {
        return Cost;
    }
}

public class LCDMonitor : Monitor
{
    public bool HasTvTuner { get; set; }
    public string MatrixType { get; set; }

    public override double CalculatePurchaseCost()
    {
        double cost = Cost;
        if (HasTvTuner && cost > 5000)
        {
            cost *= 0.95; // 5% discount
        }
        return cost;
    }
}

public class TouchMonitor : Monitor
{
    public string TouchScreenType { get; set; }
    public int HDDCapacity { get; set; }

    public override double CalculatePurchaseCost()
    {
        double monthlyPayment = Cost / 12 * 1.03; 
        return monthlyPayment;
    }
}

public class Program
{
    static List<Monitor> monitors = new List<Monitor>();

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Load();
        while (true)
        {
            Console.WriteLine("Виберіть дію:");
            Console.WriteLine("1. Додати монітор");
            Console.WriteLine("2. Видалити монітор");
            Console.WriteLine("3. Редагувати монітор");
            Console.WriteLine("4. Вивести всю інформацію про монітори");
            Console.WriteLine("5. Вийти з програми");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddMonitor();
                    break;
                case "2":
                    RemoveMonitor();
                    break;
                case "3":
                    EditMonitor();
                    break;
                case "4":
                    DisplayMonitorsInformation();
                    break;
                case "5":
                    Console.WriteLine("Вихід з програми...");
                    return;
                default:
                    Console.WriteLine("Невідома команда. Будь ласка, спробуйте знову.");
                    break;
            }
        }
    }

    static void Load() 
    {
        using StreamReader Sr = new("Base.txt", Encoding.UTF8);
        var Lines = Sr.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var Line in Lines)
        {
            var Block = Line.Split(';');
            if (Block[5].Equals("LCD"))
                monitors.Add(new LCDMonitor()
                {
                    Model = Block[0],
                    Manufacturer = Block[1],
                    Diagonal = double.Parse(Block[2]),
                    Resolution = Block[3],
                    Cost = double.Parse(Block[4]),
                    MonitorType = Block[5],
                    HasTvTuner = bool.Parse(Block[6]),
                    MatrixType = Block[7],
                });
            else
                monitors.Add(new TouchMonitor()
                {
                    Model = Block[0],
                    Manufacturer = Block[1],
                    Diagonal = double.Parse(Block[2]),
                    Resolution = Block[3],
                    Cost = double.Parse(Block[4]),
                    MonitorType = Block[5],
                    TouchScreenType = Block[8],
                    HDDCapacity = int.Parse(Block[9]),
                });
        }
    }

    static void AddMonitor()
    {
        Console.WriteLine("Додавання нового монітора:");

        Console.Write("Модель: ");
        string model = Console.ReadLine();

        Console.Write("Виробник: ");
        string manufacturer = Console.ReadLine();

        Console.Write("Діагональ: ");
        double diagonal;
        while (!double.TryParse(Console.ReadLine(), out diagonal))
        {
            Console.WriteLine("Некоректне значення. Введіть дійсне число.");
        }

        Console.Write("Роздільність: ");
        string resolution = Console.ReadLine();

        Console.Write("Вартість: ");
        double cost;
        while (!double.TryParse(Console.ReadLine(), out cost))
        {
            Console.WriteLine("Некоректне значення. Введіть дійсне число.");
        }

        Console.Write("Тип монітора (LCD/Touch): ");
        string monitorType = Console.ReadLine();

        Monitor monitor;
        if (monitorType.Equals("LCD", StringComparison.OrdinalIgnoreCase))
        {
            monitor = new LCDMonitor
            {
                Model = model,
                Manufacturer = manufacturer,
                Diagonal = diagonal,
                Resolution = resolution,
                Cost = cost,
                MonitorType = monitorType
            };
            //monitor = lcdMonitor;
        }
        else if (monitorType.Equals("Touch", StringComparison.OrdinalIgnoreCase))
        {
            monitor = new TouchMonitor
            {
                Model = model,
                Manufacturer = manufacturer,
                Diagonal = diagonal,
                Resolution = resolution,
                Cost = cost,
                MonitorType = monitorType
            };
            //monitor = touchMonitor;
        }
        else
        {
            Console.WriteLine("Невідомий тип монітора.");
            return;
        }
        using (StreamWriter sw = new StreamWriter("Base.txt", true, Encoding.UTF8))
        {
            if (monitor is LCDMonitor)
            {
                var m = (LCDMonitor)monitor;
                sw.WriteLine($"\r\n{m.Model};{m.Manufacturer};{m.Diagonal};{m.Resolution};{m.Cost};{m.MonitorType};{m.HasTvTuner};{m.MatrixType};;");
            }
            else if (monitor is TouchMonitor)
            {
                var m = (TouchMonitor)monitor;
                sw.WriteLine($"\r\n{m.Model};{m.Manufacturer};{m.Diagonal};{m.Resolution};{m.Cost};{m.MonitorType};;;{m.TouchScreenType};{m.HDDCapacity}");
            }
            
        }
        monitors.Add(monitor);


        Console.WriteLine($"Монітор {monitor.Model} успішно додано.");
    }

    static void RemoveMonitor()
    {
        Console.WriteLine("Видалення монітора:");
        Console.Write("Введіть модель монітора, який потрібно видалити: ");
        string model = Console.ReadLine();

        Monitor monitorToRemove = monitors.FirstOrDefault(m => m.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
        if (monitorToRemove != null)
        {
            monitors.Remove(monitorToRemove);
            Console.WriteLine($"Монітор {model} було успішно видалено.");
        }
        else
        {
            Console.WriteLine($"Монітор {model} не знайдено.");
        }
    }

    static void EditMonitor()
    {
        Console.WriteLine("Редагування монітора:");
        Console.Write("Введіть модель монітора, який потрібно редагувати: ");
        string model = Console.ReadLine();

        Monitor monitorToEdit = monitors.FirstOrDefault(m => m.Model.Equals(model, StringComparison.OrdinalIgnoreCase));
        if (monitorToEdit != null)
        {
            Console.Write("Нова модель: ");
            monitorToEdit.Model = Console.ReadLine();

            Console.Write("Новий виробник: ");
            monitorToEdit.Manufacturer = Console.ReadLine();

            Console.Write("Нова діагональ: ");
            double diagonal;
            while (!double.TryParse(Console.ReadLine(), out diagonal))
            {
                Console.WriteLine("Некоректне значення. Введіть дійсне число.");
            }
            monitorToEdit.Diagonal = diagonal;

            Console.Write("Нова роздільність: ");
            monitorToEdit.Resolution = Console.ReadLine();

            Console.Write("Нова вартість: ");
            double cost;
            while (!double.TryParse(Console.ReadLine(), out cost))
            {
                Console.WriteLine("Некоректне значення. Введіть дійсне число.");
            }
            monitorToEdit.Cost = cost;

            Console.WriteLine($"Дані про монітор {model} було успішно відредаговано.");
        }
        else
        {
            Console.WriteLine($"Монітор {model} не знайдено.");
        }
    }

    static void DisplayMonitorsInformation()
    {
        Console.WriteLine("Інформація про монітори:");

        foreach (var monitor in monitors)
        {
            Console.WriteLine($"Модель: {monitor.Model}, Виробник: {monitor.Manufacturer}, Діагональ: {monitor.Diagonal}, Роздільність: {monitor.Resolution}, Вартість: {monitor.Cost}, Тип монітора: {monitor.MonitorType}");
        }
    }
}
