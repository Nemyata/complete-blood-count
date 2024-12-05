using BloodCount.Domain;
using BloodCount.Domain.Configuration;
using BloodCount.DomainServices.Interfaces;

using BloodCount.DataAccess.Interfaces;
using BloodCount.DataAccess.Interfaces.Main;
using BloodCount.DataAccess.Interfaces.Python;

using System.Text.Json;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using AutoMapper;


namespace BloodCount.DomainServices.Implementation;

public class AnalysisService(IOptions<FileConfigurations> fileOptions,
                             IFilesRepository filesRepository,
                             ILLMRepository LLMRepository,
                             IPythonCallback pythonCallback,
                             IConnectionService connectionService,
                             IMapper mapper,
                             ILogger<AnalysisService> logger) : IAnalysisService
{
    private readonly FileConfigurations _fileOptions = fileOptions.Value;
    private readonly IFilesRepository _filesRepository = filesRepository;
    private readonly ILLMRepository _LLMRepository = LLMRepository;
    private readonly IPythonCallback _pythonCallback = pythonCallback;
    private readonly IConnectionService _connectionService = connectionService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AnalysisService> _logger = logger;

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        using var connection = _connectionService.OpenMainConnection();
        var transaction = connection.BeginTransaction();
        _logger.LogInformation("Обработка загруженной картинки");

        try
        {
            _logger.LogInformation("Сохранение картинки на диск");
            if (!Directory.Exists(_fileOptions.FileStorage))
                Directory.CreateDirectory(_fileOptions.FileStorage);

            var Id = Guid.NewGuid();

            var fileName = Path.GetFileName(file.FileName);
            fileName = Id.ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine(_fileOptions.FileStorage, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            {
                await file.CopyToAsync(fileStream);
                fileStream.Close();
            }

            _logger.LogInformation("Картинка сохранена на диск");

            _logger.LogInformation("Работа Python скриптов");
            var resultOCR = await _pythonCallback.CallPythonAsync(PythonType.OCR, filePath);
            var resultJsonLLM = await _pythonCallback.CallPythonAsync(PythonType.LLM, resultOCR);
            resultJsonLLM = resultJsonLLM.Replace("'", "\"");
            var LLMResult = JsonSerializer.Deserialize<LLMResultVM>(resultJsonLLM);
            var LLM2ML = _mapper.Map<LLM2ML>(LLMResult);
            JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
            var json = $"\"{JsonSerializer.Serialize(LLM2ML, jsonSerializerOptions).Replace("\"", "\\\"")}\"";
            var resultML = await _pythonCallback.CallPythonAsync(PythonType.ML, json);
            _logger.LogInformation("Работа Python скриптов завершена");

            _logger.LogInformation("Запись информации о картинке в БД");
            var files = new Files()
            {
                Id = Id,
                FileName = fileName,
                OCR = resultOCR,
            };

            await _filesRepository.AddAsync(files, connection, transaction);
            transaction.Commit();
            _logger.LogInformation("Информация о картинке сохранена в БД");

            _logger.LogInformation("Запись результата работы LLM в БД");

            var LLM = _mapper.Map<LLM>(LLMResult);
            LLM.Id = Guid.NewGuid();
            LLM.FileId = Id;
            LLM.ResultML = resultML;

            transaction = connection.BeginTransaction();
            await _LLMRepository.AddAsync(LLM, connection, transaction);
            transaction.Commit();
            _logger.LogInformation("Результат работы LLM сохранен в БД");

            _logger.LogInformation("Обработка загруженной картинки завершена");

            return resultML;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Не удалось обработать загруженную картинку");
            throw;
        }
    }
}