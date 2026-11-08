using organization_directory.Models;
using organization_directory.Repositories;

namespace organization_directory.Services;

public class OrganizationService {
    private readonly OrganizationRepository _repository;

    public OrganizationService(OrganizationRepository repository) {
        _repository = repository;
    }

    public List<Organization> GetAll() {
        return _repository.GetAll();
    }

    public Organization? GetById(int id) {
        return _repository.GetById(id);
    }

    public int Add(Organization organization) {
        ValidateOrganization(organization);

        bool duplicateInn = _repository.GetAll()
            .Any(o => o.INN == organization.INN);

        if (duplicateInn) {
            throw new ArgumentException("Организация с таким ИНН уже существует.");
        }

        return _repository.Add(organization);
    }

    public void AddRange(List<Organization> organizations) {
        if (organizations == null || organizations.Count == 0) {
            throw new ArgumentException("Список организаций пуст.");
        }

        foreach (var organization in organizations) {
            ValidateOrganization(organization);
        }

        bool duplicateInFile = organizations
            .GroupBy(o => o.INN)
            .Any(group => group.Count() > 1);

        if (duplicateInFile) {
            throw new ArgumentException(
                "XML-файл содержит организации с повторяющимися ИНН."
            );
        }

        var existingInns = _repository.GetAll()
            .Select(o => o.INN)
            .ToHashSet();

        bool duplicateInDatabase = organizations
            .Any(o => existingInns.Contains(o.INN));

        if (duplicateInDatabase) {
            throw new ArgumentException(
                "Некоторые организации из XML-файла уже существуют в базе данных."
            );
        }

        _repository.AddRange(organizations);
    }

    public bool Update(Organization organization) {
        ValidateOrganization(organization);

        bool duplicateInn = _repository.GetAll()
            .Any(o => o.INN == organization.INN && o.Id != organization.Id);

        if (duplicateInn) {
            throw new ArgumentException("Организация с таким ИНН уже существует.");
        }

        return _repository.Update(organization);
    }

    public bool Delete(int id) {
        return _repository.Delete(id);
    }

    public List<Organization> FindByName(string query) {
        return _repository.GetAll()
            .Where(o => o.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Organization> FindByINN(string query) {
        return _repository.GetAll()
            .Where(o => o.INN.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Organization> FindByEmail(string query) {
        return _repository.GetAll()
            .Where(o => o.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Organization> FindByPhone(string query) {
        return _repository.GetAll()
            .Where(o =>
                o.ReceptionPhone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                o.HRPhone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                o.AccountingPhone.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Organization> SortByName(bool descending = false) {
        var organizations = _repository.GetAll();

        return descending
            ? organizations.OrderByDescending(o => o.Name).ToList()
            : organizations.OrderBy(o => o.Name).ToList();
    }

    public List<Organization> SortByINN(bool descending = false) {
        var organizations = _repository.GetAll();

        return descending
            ? organizations.OrderByDescending(o => o.INN).ToList()
            : organizations.OrderBy(o => o.INN).ToList();
    }

    public List<Organization> SortByEmail(bool descending = false) {
        var organizations = _repository.GetAll();

        return descending
            ? organizations.OrderByDescending(o => o.Email).ToList()
            : organizations.OrderBy(o => o.Email).ToList();
    }

    private void ValidateOrganization(Organization organization) {
        if (organization == null) {
            throw new ArgumentNullException(nameof(organization));
        }

        if (string.IsNullOrWhiteSpace(organization.Name)) {
            throw new ArgumentException("Название организации не может быть пустым.");
        }

        if (string.IsNullOrWhiteSpace(organization.INN)) {
            throw new ArgumentException("ИНН не может быть пустым.");
        }

        if (!organization.INN.All(c => c >= '0' && c <= '9')) {
            throw new ArgumentException("ИНН должен содержать только цифры.");
        }

        if (organization.INN.Length != 10 && organization.INN.Length != 12) {
            throw new ArgumentException("ИНН должен содержать 10 или 12 цифр.");
        }

        if (!string.IsNullOrWhiteSpace(organization.Email)) {
            try {
                var email = new System.Net.Mail.MailAddress(organization.Email);

                if (email.Address != organization.Email) {
                    throw new ArgumentException("Некорректный формат электронной почты.");
                }
            }
            catch (FormatException) {
                throw new ArgumentException("Некорректный формат электронной почты.");
            }
        }
    }
}