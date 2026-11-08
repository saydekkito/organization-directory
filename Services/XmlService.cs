using System.Xml.Serialization;
using organization_directory.Models;

namespace organization_directory.Services;

public class XmlService {
    private readonly OrganizationService _organizationService;

    public XmlService(OrganizationService organizationService) {
        _organizationService = organizationService;
    }

    public void ExportToXml(string filePath) {
        var organizations = _organizationService.GetAll();

        var serializer = new XmlSerializer(typeof(List<Organization>));

        using var writer = new StreamWriter(filePath);

        serializer.Serialize(writer, organizations);
    }

    public void ImportFromXml(string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException(
                "XML-файл не найден.",
                filePath
            );
        }

        var serializer = new XmlSerializer(typeof(List<Organization>));

        using var reader = new StreamReader(filePath);

        var organizations = (List<Organization>?)serializer.Deserialize(reader);

        if (organizations == null) {
            throw new InvalidDataException(
                "Не удалось прочитать данные из XML-файла."
            );
        }

        _organizationService.AddRange(organizations);
    }
}