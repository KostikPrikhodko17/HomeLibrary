using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                    case "4":
                        if (books.Count == 0)
                        {
                            Console.WriteLine("Список книг пуст.");
                            continue;
                        }
                        ChangeStatusBook(books);
                        break;

                    case "6":
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

            int grade = GradeBook();
            if (grade == 0)
                return;


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
                Console.Write("\nВернуться в меню - x\nВведите статус чтения. \n1 - Не начато \n2 - Читаю \n3 - Прочитано\n: ");
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
                Console.Write("\nВернуться в меню - x\nВведите оценку книги от 1 до 10\n: ");
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
            Console.WriteLine("\n===Список книг===");
            foreach (var book in books)
            {
                Console.Write($"ID: {book.Id}\nНазвание: {book.Name}\nАвтор: {book.Author}\nСтатус: ");
                showColorStatusBook(book);
                Console.WriteLine($"Оценка: {book.Grade}\n");
            }
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
            Console.WriteLine(book.Status);
            Console.ResetColor();
        }

        static void ChangeStatusBook(List<ModelBook> books) // Изменить статус чтения книги
        {
            ShowColorStatusBookDelegate showColorStatusBook = ShowColorStatusBook;
            Console.WriteLine($"ID\tНазвание\tСтатус");

            foreach (var book in books)
            {
                Console.Write($"{book.Id}\t{book.Name}\t");
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
                }
            }
        }
    }
}