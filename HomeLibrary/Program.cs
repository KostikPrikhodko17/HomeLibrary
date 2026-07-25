using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace HomeLibrary
{
    class Program
    {
        delegate ConsoleColor GetStatusColorBook(StatusReading status);

        delegate void ShowColorStatusBookDelegate(ModelBook book);


        static void Main()
        {
            ModelBook book = new ModelBook();
            List<ModelBook> books = new List<ModelBook>();

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "books.txt");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                books = System.Text.Json.JsonSerializer.Deserialize<List<ModelBook>>(json) ?? new List<ModelBook>();
            }

            bool isStarted = true;
            do
            {
                Console.WriteLine("===Домашняя библиотека===");
                Console.WriteLine("\n1 - Добавить книгу\n2 - Показать все книги\n3 - Найти книгу\n4 - Изменить статус чтения\n4 - Уддалить книгу\n5 - Статистика\n6 - Сохранить и выйти");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddBook(books);
                        break;

                    case "2":
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Список книг пуст.");
                            continue;
                        }
                        ShowAllBooks(books);
                        break;

                    case "3":
                        break;

                    case "4":
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Список книг пуст.");
                            continue;
                        }
                        ChangeStatusBook(books);
                        break;

                    case "5":
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Список книг пуст.");
                            continue;
                        }
                        Statistics(books);
                        break;

                    case "6":
                        SaveBooks(books, filePath);
                        isStarted = false;
                        break;

                    default:
                        Console.WriteLine("Неверный ввод. Пожалуйста, выберите правильный вариант.");
                        break;
                }
            }
            while (isStarted);

        }

        static void AddBook(List<ModelBook> books)
        {
            string message = "Введите название книги: ";
            string name = NameBookOrNameAuthor(message);
            if (name == null)
                return;

            message = "Введите имя автора: ";
            string author = NameBookOrNameAuthor(message);
            if (author == null)
                return;

            int status = StatusReading();
            if (status == 0)
                return;

            int grade = 0;
            if (status == 3)
            {
                grade = GradeBook();
                if (grade == 0)
                    return;
            }

            books.Add(new ModelBook
            {
                Id = books.Count + 1,
                Name = name,
                Author = author,
                Status = (StatusReading)status,
                Grade = grade 
            });
            Console.WriteLine();
        }

        static string NameBookOrNameAuthor(string message) // Возвращает нназвание книги и имя автора
        {
            while (true)
            {
                Console.Write($"\nВернуться в меню - x\n{message}");
                string name = Console.ReadLine();

                if (name != "x" && !string.IsNullOrWhiteSpace(name))
                    return name;
                else if (name.ToLower() == "x")
                    return null;
                else
                    Console.WriteLine("\nНекоректные данные.");
            }

        }

        static int StatusReading() // Возвращает статус чтения книги
        {
            while (true)
            {
                Console.Write("\nВернуться в меню - x\n1 - Не начато \n2 - Читаю \n3 - Прочитано\nВведите статус чтения: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int status) && status >= 1 && status <= 3)
                    return status;
                else if (input.ToLower() == "x")
                    return 0;
                else
                    Console.WriteLine("Некоректные данные.");
            }
        }

        static int GradeBook() // Возвращает оценку книги
        {
            while (true)
            {
                Console.Write("\nВернуться в меню - x\nВведите оценку книги от 1 до 10: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int grade) && grade >= 1 && grade <= 10)
                    return grade;
                else if (input.ToLower() == "x")
                    return 0;
                else
                    Console.WriteLine("Некоректные данные.");
            }
        }


        static void ShowAllBooks(List<ModelBook> books) // Показать все книги
        {
            ShowColorStatusBookDelegate showColorStatusBook = ShowColorStatusBook;
            Console.WriteLine("\n===Список книг===\n");
            Console.WriteLine($"{"ID", -5} {"Название", -30} {"Автор", -20} {"Оценка", -10} {"Статус", -5}");
            Console.WriteLine(new string('-', 75));
            foreach (var book in books)
            {
                Console.Write($"{book.Id, -5} {book.Name, -30} {book.Author, -20} {book.Grade, -10}");
                showColorStatusBook(book);
            }
            Console.WriteLine();
        }

        static ConsoleColor GetStatusColor(StatusReading status) // Возвращает цвет статуса чтения
        {
            return status switch
            {
                HomeLibrary.StatusReading.NotStarted => ConsoleColor.DarkCyan,
                HomeLibrary.StatusReading.Reading => ConsoleColor.Yellow,
                HomeLibrary.StatusReading.Finished => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }

        static void ShowColorStatusBook(ModelBook book) // раскрашивает статус чтения книги в зависимости от его значения
        {
            GetStatusColorBook statusColorBook = GetStatusColor;
            Console.ForegroundColor = statusColorBook(book.Status);
            Console.WriteLine(book.Status + "\n");
            Console.ResetColor();
        }

        static void ChangeStatusBook(List<ModelBook> books) // Изменить статус чтения книги
        {
            ShowColorStatusBookDelegate showColorStatusBook = ShowColorStatusBook;
            Console.WriteLine($"{"ID", -5} {"Название", -30} {"Статус"}");
            Console.WriteLine(new string('-', 50));

            foreach (var book in books)
            {
                Console.Write($"{book.Id, -5} {book.Name, -30}");
                showColorStatusBook(book);
            }
            Console.Write("\nЧтобы изменить статус чтения, введите ID книги: ");
            string changestatus = Console.ReadLine();

            if (int.TryParse(changestatus, out int bookId))
            {
                ModelBook modelBook = books.FirstOrDefault(b => b.Id == bookId);

                if (modelBook != null)
                {
                    int newStatus = StatusReading();
                    if (newStatus == 0)
                        return;

                    modelBook.Status = (StatusReading)newStatus;

                    if (newStatus == 3)
                    {
                        int newGrade = GradeBook();
                        if (newGrade == 0)
                            return;
                        modelBook.Grade = newGrade;
                    }
                }
            }
            else
            {
                Console.WriteLine("Некоректные данные.");
                return;
            }
        }

        static void SaveBooks(List<ModelBook> books, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            string json = JsonSerializer.Serialize(books, options);
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        static void Statistics(List<ModelBook> books)
        {
            int finishedBooks = books.Count(b => b.Status == HomeLibrary.StatusReading.Finished);
            int readingBooks = books.Count(b => b.Status == HomeLibrary.StatusReading.Reading);
            int notStarted = books.Count(b => b.Status == HomeLibrary.StatusReading.NotStarted);

            DrawBar("Прочитано", finishedBooks, books.Count, ConsoleColor.Green);
            DrawBar("Читаю", readingBooks, books.Count, ConsoleColor.Yellow);
            DrawBar("Не начато", notStarted, books.Count, ConsoleColor.DarkCyan);
        }

        static void DrawBar(string status, int booksCount, int allBooks, ConsoleColor color)
        {
            const int barWidth = 20;
            int filledWidth = (int)((double)booksCount / allBooks * barWidth);
            int emptyWidth = barWidth - filledWidth;
            string bar = new string('█', filledWidth) + new string('░', emptyWidth);

            Console.ForegroundColor = color;
            Console.Write($"\n{status}: ");
            Console.ResetColor();
            Console.WriteLine($"{bar} {booksCount}\n");
        }
    }
}