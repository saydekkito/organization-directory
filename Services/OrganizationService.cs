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
        return _repository.Add(organization);
    }
    
    public bool Update(Organization organization) {
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
}