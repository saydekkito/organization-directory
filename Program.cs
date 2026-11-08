using organization_directory.Data;
using organization_directory.Models;
using organization_directory.Repositories;
using organization_directory.Services;

namespace organization_directory;

internal class Program {
    static void Main(string[] args) {
        var databaseContext = new DatabaseContext();
        databaseContext.InitializeDatabase();

        var repository = new OrganizationRepository(databaseContext);
        var organizationService = new OrganizationService(repository);
        var xmlService = new XmlService(organizationService);

        RunMenu(organizationService, xmlService);
    }

    static void RunMenu(
        OrganizationService organizationService,
        XmlService xmlService) {
        bool isRunning = true;

        while (isRunning) {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine("   СПРАВОЧНИК ТЕЛЕФОНОВ ОРГАНИЗАЦИЙ");
            Console.WriteLine("==================================");
            Console.WriteLine();
            Console.WriteLine("1. Просмотреть все организации");
            Console.WriteLine("2. Найти организацию");
            Console.WriteLine("3. Добавить организацию");
            Console.WriteLine("4. Изменить организацию");
            Console.WriteLine("5. Удалить организацию");
            Console.WriteLine("6. Сортировать организации");
            Console.WriteLine("7. Импортировать данные из XML");
            Console.WriteLine("8. Экспортировать данные в XML");
            Console.WriteLine("0. Выход");
            Console.WriteLine();
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine() ?? string.Empty;

            switch (choice) {
                case "1":
                    ShowAllOrganizations(organizationService);
                    break;

                case "2":
                    SearchOrganizations(organizationService);
                    break;

                case "3":
                    AddOrganization(organizationService);
                    break;

                case "4":
                    EditOrganization(organizationService);
                    break;

                case "5":
                    DeleteOrganization(organizationService);
                    break;

                case "6":
                    SortOrganizations(organizationService);
                    break;

                case "7":
                    ImportOrganizationsFromXml(xmlService);
                    break;

                case "8":
                    ExportOrganizationsToXml(xmlService);
                    break;

                case "0":
                    isRunning = false;
                    break;

                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }

        Console.WriteLine("Работа программы завершена.");
    }

    static void ShowAllOrganizations(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== СПИСОК ОРГАНИЗАЦИЙ ===");
        Console.WriteLine();

        var organizations = organizationService.GetAll();

        DisplayOrganizations(organizations);

        Pause();
    }

    static void SearchOrganizations(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== ПОИСК ОРГАНИЗАЦИИ ===");
        Console.WriteLine();
        Console.WriteLine("1. По названию");
        Console.WriteLine("2. По ИНН");
        Console.WriteLine("3. По E-mail");
        Console.WriteLine("4. По телефону");
        Console.WriteLine("0. Назад");
        Console.WriteLine();
        Console.Write("Выберите поле для поиска: ");

        string choice = Console.ReadLine() ?? string.Empty;

        if (choice == "0")
            return;

        if (choice != "1" && choice != "2" &&
            choice != "3" && choice != "4") {
            Console.WriteLine("\nНеизвестная команда.");
            Pause();
            return;
        }

        Console.Write("\nВведите поисковый запрос: ");
        string query = (Console.ReadLine() ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(query)) {
            Console.WriteLine("\nПоисковый запрос не может быть пустым.");
            Pause();
            return;
        }

        List<Organization> results = choice switch {
            "1" => organizationService.FindByName(query),
            "2" => organizationService.FindByINN(query),
            "3" => organizationService.FindByEmail(query),
            "4" => organizationService.FindByPhone(query),
            _ => new List<Organization>()
        };

        Console.WriteLine();

        DisplayOrganizations(results);
        Pause();
    }

    static void AddOrganization(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== ДОБАВЛЕНИЕ ОРГАНИЗАЦИИ ===");
        Console.WriteLine();
        Console.WriteLine("Правила ввода:");
        Console.WriteLine("- Название: обязательно.");
        Console.WriteLine("- ИНН: 10 или 12 цифр, уникальный.");
        Console.WriteLine("- E-mail: необязателен, но должен иметь корректный формат.");
        Console.WriteLine("- Телефоны: необязательны.");
        Console.WriteLine();

        var organization = new Organization {
            Name = ReadRequiredValue("Название организации: "),
            INN = ReadINN(organizationService),
            Email = ReadEmail(),
            ReceptionPhone = ReadOptionalValue("Телефон приёмной: "),
            HRPhone = ReadOptionalValue("Телефон отдела кадров: "),
            AccountingPhone = ReadOptionalValue("Телефон бухгалтерии: ")
        };

        try {
            organizationService.Add(organization);
            Console.WriteLine("\nОрганизация успешно добавлена.");
        }
        catch (Exception ex) {
            Console.WriteLine($"\nНе удалось добавить организацию: {ex.Message}");
        }

        Pause();
    }

    static void EditOrganization(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== РЕДАКТИРОВАНИЕ ОРГАНИЗАЦИИ ===");
        Console.WriteLine();

        Console.Write("Введите ID организации: ");

        if (!int.TryParse(Console.ReadLine(), out int id)) {
            Console.WriteLine("Некорректный ID.");
            Pause();
            return;
        }

        var organization = organizationService.GetById(id);

        if (organization == null) {
            Console.WriteLine("Организация с таким ID не найдена.");
            Pause();
            return;
        }

        Console.WriteLine("\nТекущие данные организации:");
        Console.WriteLine($"Название: {organization.Name}");
        Console.WriteLine($"ИНН: {organization.INN}");
        Console.WriteLine($"E-mail: {organization.Email}");
        Console.WriteLine($"Телефон приёмной: {organization.ReceptionPhone}");
        Console.WriteLine($"Телефон отдела кадров: {organization.HRPhone}");
        Console.WriteLine($"Телефон бухгалтерии: {organization.AccountingPhone}");

        Console.WriteLine("\nВведите новые данные.");
        Console.WriteLine("Enter — оставить прежнее значение.");
        Console.WriteLine("Для очистки необязательного поля введите '-'.");
        Console.WriteLine();

        organization.Name = ReadUpdatedRequiredValue(
            "Название организации", organization.Name);

        organization.INN = ReadUpdatedINN(
            organizationService, organization.Id, organization.INN);

        organization.Email = ReadUpdatedEmail(organization.Email);

        organization.ReceptionPhone = ReadUpdatedOptionalValue(
            "Телефон приёмной", organization.ReceptionPhone);

        organization.HRPhone = ReadUpdatedOptionalValue(
            "Телефон отдела кадров", organization.HRPhone);

        organization.AccountingPhone = ReadUpdatedOptionalValue(
            "Телефон бухгалтерии", organization.AccountingPhone);

        try {
            bool updated = organizationService.Update(organization);

            Console.WriteLine(updated
                ? "\nДанные организации успешно обновлены."
                : "\nНе удалось обновить данные организации.");
        }
        catch (Exception ex) {
            Console.WriteLine($"\nОшибка при обновлении: {ex.Message}");
        }

        Pause();
    }

    static void DeleteOrganization(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== УДАЛЕНИЕ ОРГАНИЗАЦИИ ===");
        Console.WriteLine();

        Console.Write("Введите ID организации: ");

        if (!int.TryParse(Console.ReadLine(), out int id)) {
            Console.WriteLine("Некорректный ID.");
            Pause();
            return;
        }

        var organization = organizationService.GetById(id);

        if (organization == null) {
            Console.WriteLine("Организация с таким ID не найдена.");
            Pause();
            return;
        }

        Console.WriteLine($"\nНазвание: {organization.Name}");
        Console.WriteLine($"ИНН: {organization.INN}");

        Console.Write("\nВы действительно хотите удалить эту организацию? (y/n): ");

        string confirmation = Console.ReadLine() ?? string.Empty;

        if (!confirmation.Equals("y", StringComparison.OrdinalIgnoreCase)) {
            Console.WriteLine("\nУдаление отменено.");
            Pause();
            return;
        }

        bool deleted = organizationService.Delete(id);

        Console.WriteLine(deleted
            ? "\nОрганизация успешно удалена."
            : "\nНе удалось удалить организацию.");

        Pause();
    }

    static void SortOrganizations(OrganizationService organizationService) {
        Console.Clear();

        Console.WriteLine("=== СОРТИРОВКА ОРГАНИЗАЦИЙ ===");
        Console.WriteLine();
        Console.WriteLine("1. По названию (по возрастанию)");
        Console.WriteLine("2. По ИНН (по возрастанию)");
        Console.WriteLine("3. По E-mail (по возрастанию)");
        Console.WriteLine("4. По E-mail (по убыванию)");
        Console.WriteLine("0. Назад");
        Console.WriteLine();
        Console.Write("Выберите способ сортировки: ");

        string choice = Console.ReadLine() ?? string.Empty;

        if (choice == "0")
            return;

        var organizations = choice switch {
            "1" => organizationService.SortByName(),
            "2" => organizationService.SortByINN(),
            "3" => organizationService.SortByEmail(),
            "4" => organizationService.SortByEmail(true),
            _ => null
        };

        if (organizations == null) {
            Console.WriteLine("\nНеизвестная команда.");
            Pause();
            return;
        }

        DisplayOrganizations(organizations);

        Pause();
    }

    static void ImportOrganizationsFromXml(XmlService xmlService) {
        Console.Clear();

        Console.WriteLine("=== ИМПОРТ ДАННЫХ ИЗ XML ===");
        Console.WriteLine();
        Console.WriteLine("Внимание!");
        Console.WriteLine("Импортированные организации будут добавлены к существующим.");
        Console.WriteLine("Если в файле обнаружатся повторяющиеся ИНН, импорт не выполнится.");
        Console.WriteLine();

        Console.Write("Введите путь к XML-файлу: ");
        string filePath = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(filePath)) {
            Console.WriteLine("\nПуть к файлу не может быть пустым.");
            Pause();
            return;
        }

        try {
            xmlService.ImportFromXml(filePath);

            Console.WriteLine("\nДанные успешно импортированы.");
        }
        catch (Exception ex) {
            Console.WriteLine($"\nОшибка импорта: {ex.Message}");
        }

        Pause();
    }

    static void ExportOrganizationsToXml(XmlService xmlService) {
        Console.Clear();

        Console.WriteLine("=== ЭКСПОРТ ДАННЫХ В XML ===");
        Console.WriteLine();

        Console.Write("Введите путь к XML-файлу: ");
        string filePath = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(filePath)) {
            Console.WriteLine("\nПуть к файлу не может быть пустым.");
            Pause();
            return;
        }

        try {
            xmlService.ExportToXml(filePath);

            Console.WriteLine("\nДанные успешно экспортированы.");
            Console.WriteLine($"Файл: {Path.GetFullPath(filePath)}");
        }
        catch (Exception ex) {
            Console.WriteLine($"\nОшибка экспорта: {ex.Message}");
        }

        Pause();
    }

    static void DisplayOrganizations(List<Organization> organizations) {
        if (organizations.Count == 0) {
            Console.WriteLine("Организации не найдены.");
            return;
        }

        Console.WriteLine($"Найдено организаций: {organizations.Count}");
        Console.WriteLine();

        foreach (var organization in organizations) {
            Console.WriteLine($"ID: {organization.Id}");
            Console.WriteLine($"Название: {organization.Name}");
            Console.WriteLine($"ИНН: {organization.INN}");
            Console.WriteLine($"E-mail: {organization.Email}");
            Console.WriteLine($"Телефон приёмной: {organization.ReceptionPhone}");
            Console.WriteLine($"Телефон отдела кадров: {organization.HRPhone}");
            Console.WriteLine($"Телефон бухгалтерии: {organization.AccountingPhone}");
            Console.WriteLine("----------------------------------");
        }
    }

    static string ReadRequiredValue(string prompt) {
        while (true) {
            Console.Write(prompt);
            string value = (Console.ReadLine() ?? string.Empty).Trim();

            if (!string.IsNullOrWhiteSpace(value))
                return value;

            Console.WriteLine("Поле обязательно для заполнения. Попробуйте ещё раз.");
        }
    }

    static string ReadINN(
        OrganizationService organizationService,
        int? currentId = null) {
        while (true) {
            Console.Write("ИНН: ");
            string inn = (Console.ReadLine() ?? string.Empty).Trim();

            if (inn.Length != 10 && inn.Length != 12) {
                Console.WriteLine("ИНН должен содержать 10 или 12 цифр.");
                continue;
            }

            if (!inn.All(c => c >= '0' && c <= '9')) {
                Console.WriteLine("ИНН должен содержать только цифры.");
                continue;
            }

            bool alreadyExists = organizationService.GetAll().Any(organization =>
                organization.INN == inn &&
                organization.Id != currentId
            );

            if (alreadyExists) {
                Console.WriteLine("Организация с таким ИНН уже существует.");
                continue;
            }

            return inn;
        }
    }

    static string ReadEmail() {
        while (true) {
            Console.Write("E-mail: ");
            string email = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(email))
                return string.Empty;

            try {
                var mailAddress = new System.Net.Mail.MailAddress(email);

                if (mailAddress.Address == email)
                    return email;
            }
            catch (FormatException) {
                // некорректный формат адреса
            }
            catch (ArgumentException) {
                // некорректный аргумент
            }

            Console.WriteLine("Некорректный e-mail. Попробуйте ещё раз.");
        }
    }

    static string ReadOptionalValue(string prompt) {
        Console.Write(prompt);
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    static string ReadUpdatedRequiredValue(
        string fieldName,
        string currentValue) {
        while (true) {
            Console.Write($"{fieldName} [{currentValue}] (Enter — оставить): ");
            string input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return currentValue;

            return input;
        }
    }

    static string ReadUpdatedINN(
        OrganizationService organizationService,
        int currentId,
        string currentValue) {
        while (true) {
            Console.Write($"ИНН [{currentValue}] (Enter — оставить): ");
            string inn = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(inn))
                return currentValue;

            if (inn.Length != 10 && inn.Length != 12) {
                Console.WriteLine("ИНН должен содержать 10 или 12 цифр.");
                continue;
            }

            if (!inn.All(c => c >= '0' && c <= '9')) {
                Console.WriteLine("ИНН должен содержать только цифры.");
                continue;
            }

            bool alreadyExists = organizationService.GetAll().Any(organization =>
                organization.INN == inn &&
                organization.Id != currentId
            );

            if (alreadyExists) {
                Console.WriteLine("Организация с таким ИНН уже существует.");
                continue;
            }

            return inn;
        }
    }

    static string ReadUpdatedEmail(string currentValue) {
        while (true) {
            Console.Write($"E-mail [{currentValue}] (Enter — оставить, '-' — очистить): ");
            string email = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(email))
                return currentValue;

            if (email == "-")
                return string.Empty;

            try {
                var mailAddress = new System.Net.Mail.MailAddress(email);

                if (mailAddress.Address == email)
                    return email;
            }
            catch (FormatException) {
                // некорректный формат адреса
            }
            catch (ArgumentException) {
                // некорректный аргумент
            }

            Console.WriteLine("Некорректный e-mail. Попробуйте ещё раз.");
        }
    }

    static string ReadUpdatedOptionalValue(
        string fieldName,
        string currentValue) {
        Console.Write($"{fieldName} [{currentValue}] (Enter — оставить, '-' — очистить): ");
        string input = (Console.ReadLine() ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(input))
            return currentValue;

        if (input == "-")
            return string.Empty;

        return input;
    }

    static void Pause() {
        Console.WriteLine("\nНажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
}