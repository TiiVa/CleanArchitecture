namespace CleanArchitecture.Application.DTOs;

public class ExcelFileDto
{
    public Guid ExcelFileId { get; set; } = Guid.NewGuid();

    public string Path { get; set; } = string.Empty;
}