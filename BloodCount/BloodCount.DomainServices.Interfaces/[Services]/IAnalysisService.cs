using Microsoft.AspNetCore.Http;


namespace BloodCount.DomainServices.Interfaces;

public interface IAnalysisService
{
    Task<string> UploadFileAsync(IFormFile file);
}