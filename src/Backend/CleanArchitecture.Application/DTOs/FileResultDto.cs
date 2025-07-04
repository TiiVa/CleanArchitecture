namespace CleanArchitecture.Application.DTOs;

public class FileResultDto
{
    public MemoryStream FileStream { get; set; }
    public string ContentType { get; set; }
}