using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleAppV3.Interface;
using ConsoleAppV3.Models;
using ConsoleAppV3.Repositories;

namespace ConsoleAppV3
{
    public class Program
    {
        private static IUserManager _userManager;
        private static INoteArchive _noteArchive;
        private static bool _isRunning = true;
        static void Main(string[] args)
        {
            ServiceInitialize();
            ConsoleMessage();
            while (_isRunning)
            {
                MainMenu();
                MenuCommands(Console.ReadLine());
            }
        }
        private static void ServiceInitialize()
        {
            _userManager = new UserManager();
            _noteArchive = new NoteRepository();
        }
        private static void ConsoleMessage()
        {
            System.Console.WriteLine("обсидиан для бедных");
            if (_userManager.GetUser() != null)
            {
                System.Console.WriteLine($"добро пожаловать, снова {_userManager.GetUser().Name}");
            }
        }
        private static void MainMenu()
        {
            DisplayMenu();
        }
        private static readonly Dictionary<string, Action> Commands = new Dictionary<string, Action>
        {
            { "1", RegisterUser },
            { "2", ShowNote },
            { "3", AddNote },
            { "4", ToggleNoteComplete },
            { "5", ChangeNote },
            { "6", RemoveNote },
            { "7", ShowHelp },
            { "8", Exit }
        };
        private static void DisplayMenu()
        {
            Console.WriteLine("\nменю");
            Console.WriteLine("1) Регистрация");
            Console.WriteLine("2) Просмотр заметок");
            Console.WriteLine("3) Создать заметку");
            Console.WriteLine("4) Изменить статус заметки");
            Console.WriteLine("5) Редактировать заметку");
            Console.WriteLine("6) Удалить заметку");
            Console.WriteLine("7) Помощь");
            Console.WriteLine("8) Выход");
        }
        private static void MenuCommands(string command)
        {
            try
            {
                if (Commands.TryGetValue(command, out var action))
                {
                    action();
                }
                else
                {
                    Console.WriteLine("Неизвестная команда");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        private static void RegisterUser()
        {
            if (_userManager.GetUser() != null)
                throw new InvalidOperationException("такой юзер уже есть");
            System.Console.WriteLine("введите свое имя: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("имя не может быть пустым");
            _userManager.RegisterUser(new User { Name = name });
            System.Console.WriteLine($"Юзер {name} зареган успешно");
        }
        private static void AddNote()
        {
            var user = _userManager.GetUser()
            ?? throw new UnauthorizedAccessException("сначала зарегайся");
            Console.WriteLine("введи названия заметки");
            var title = Console.ReadLine();
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("название не может быть пустым");
            Console.WriteLine("введи описание заметки");
            var description = Console.ReadLine();
            _noteArchive.AddNote(new Note
            {
                Title = title,
                Description = description,
                User = user
            });
            System.Console.WriteLine("заметка создана");
            ShowNote();
        }
        private static void ShowNote()
        {
            var user = _userManager.GetUser();
            if (user == null)
            {
                Console.WriteLine("Сначала войдите в систему");
                return;
            }
            var notes = _noteArchive.GetNotes()
                .Where(n => n.User?.Name == user.Name)
                .ToList();
            if (!notes.Any())
            {
                System.Console.WriteLine("заметок нет");
                return;
            }
            System.Console.WriteLine("Id заметки" + "Sozdana " + "Status ");
            foreach (var note in notes)
            {
                Console.WriteLine($"{note.Id} | {note.Title.Trim()} | {note.NoteCreationTime} | {(note.IsCompleted ? "✓" : "✗")}");
            }
        }
        private static void ToggleNoteComplete()
        {
            var user = _userManager.GetUser();
            if (user == null)
            {
                System.Console.WriteLine("сначала зарегайся");
                return;
            }
            Console.WriteLine("введи id заметки ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                System.Console.WriteLine("айди некорректный");
                return;
            }
            try
            {
                var note = _noteArchive.SearchNoteById(id);
                if (note.User?.Name != user.Name)
                {
                    System.Console.WriteLine("это не ваша заметка");
                    return;
                }
                note.IsCompleted = !note.IsCompleted;
                _noteArchive.ChangeNote(note);
                System.Console.WriteLine($"Статус замкетки {id} изменен на {(note.IsCompleted ? "done" : "in work")}");
                ShowNote();
            }
            catch (Exception)
            {
                System.Console.WriteLine($"Заметка с {id} не найдена");
            }
        }
        private static void ChangeNote()
        {
            var currentUser = _userManager.GetUser();
            if (currentUser == null)
            {
                System.Console.WriteLine("зарегайся уже");
                return;
            }
            try
            {
                System.Console.WriteLine("введи id заметки для изменения ");
                if (!int.TryParse(Console.ReadLine(), out int Id))
                {
                    System.Console.WriteLine("некорректно введен id");
                    return;
                }
                var note = _noteArchive.SearchNoteById(Id);
                if (note.User?.Name != currentUser.Name)
                {
                    System.Console.WriteLine("не ваша заметка");
                    return;
                }
                System.Console.WriteLine($"текущее название заметки {note.Title}");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrEmpty(newTitle))
                {
                    note.Title = newTitle;
                }
                System.Console.WriteLine($"текущее описание заметки {note.Description}");
                var newDescription = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDescription)) 
                {
                    note.Description = newDescription;
                }       
                System.Console.WriteLine($"изменить статус заметки? {(note.IsCompleted ? "completed" : "in work")}) ? [y/n]");
                var changeStatus = Console.ReadLine().Trim().ToLower() == "y";
                if (changeStatus)
                {
                    note.IsCompleted = !note.IsCompleted;
                }
                _noteArchive.ChangeNote(note);
                System.Console.WriteLine("Заметка обновлена ");

                System.Console.WriteLine($"обновленные данные: id {note.Id} title {note.Title} description {note.Description} status {(note.IsCompleted ? "completed" : "in work")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        private static void RemoveNote()
        {
            var user = _userManager.GetUser();
            if (user == null)
            {
                System.Console.WriteLine("Сначала зарегайся");
                return;
            }
            System.Console.WriteLine("Введи айди заметки которую нужно удалить");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                System.Console.WriteLine("invalid id");
                return;
            }
            try
            {
                var note = _noteArchive.SearchNoteById(id);
                if (note.User?.Name != user?.Name)
                {
                    System.Console.WriteLine("Нельзя удалить чужую заметку");
                    return;
                }
                _noteArchive.RemoveNote(id);
                System.Console.WriteLine($"заметка {id} удалена");
                ShowNote();
            }
            catch (Exception)
            {
                Console.WriteLine("invalid id");
            }
        }
        private static void ShowHelp()
        {
            DisplayMenu();
        }
        private static void Exit()
        {
            Console.WriteLine("Данные сохранены");
            _userManager.SaveUser();
            _noteArchive.SaveNote();
            System.Console.WriteLine("до следующей встречи");
            _isRunning = false;
        }
    }
}
