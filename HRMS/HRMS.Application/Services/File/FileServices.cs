using HRMS.Application.Common.Interface;
using HRMS.Domain.Common;
using HRMS.Domain.Constants;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace HRMS.Application.Services.File
{
    public class FileServices:IFileServices
    {
        private readonly IFileStorageFactory _fileStorageFactory;
        private readonly ILocalStorageProvider _localStorageProvider;
        private readonly IUnitOfWork _unitOfWork;
        public FileServices(IFileStorageFactory fileStorageFactory, IUnitOfWork unitOfWork)
        {
            _fileStorageFactory = fileStorageFactory;
            _unitOfWork = unitOfWork;
            _localStorageProvider = (ILocalStorageProvider)_fileStorageFactory.CreateProvider(new() { ConfigJson = FileConstants.LOCAL_STORAGE_CONFIG });
        }

        public async Task<ApiResponseDto<bool>> UploadImageAsync(string filename, byte[]? filebytes, FileLocationConfigDto configDto)
        {
            if (filebytes is null || filebytes.Length == 0)
                return ApiResponseDto<bool>.FailureStatus(FileConstants.CONTENT_EMPTY_MSG);

            using var memoryStream = new MemoryStream(filebytes);

            var isUploaded = await _fileStorageFactory
                        .CreateProvider(configDto)
                        .UploadAsync(filename, memoryStream);

            return ApiResponseDto<bool>.FlagStatus(isUploaded, isUploaded ? FileConstants.DOWLOAD_SUCCESS_MSG : FileConstants.DOWLOAD_FAIL_MSG);
        }
        public async Task<ApiResponseDto<Guid?>> SaveFileAsync(FileRequestDto request)
        {
            var setting = await _unitOfWork.SystemSettingsRepo.TableNoTracking.FirstOrDefaultAsync(x => x.SettingName == "FileStorageLocation");

            if (!int.TryParse(setting?.SettingValue, out int locationId))
                return ApiResponseDto<Guid?>.FailureStatus(FileConstants.NO_STORAGE_CONFIG_MSG);

            var location = await _unitOfWork.FileLocationConfigurationsRepo.TableNoTracking
                                .Where(x => x.Id == locationId)
                                .Select(l => new FileLocationConfigDto
                                {
                                    ConfigName = l.ConfigName,
                                    ConfigJson = l.ConfigJson,
                                })
                                .FirstOrDefaultAsync();

            if (location is null)
                return ApiResponseDto<Guid?>.FailureStatus(FileConstants.NO_STORAGE_CONFIG_MSG);

            var file = await StoreFileInDbAsync(request, locationId);
            if (file is null) return ApiResponseDto<Guid?>.FailureStatus(FileConstants.UPLOAD_FAILED_MSG);

            var uploadRes = await UploadImageAsync(request.FileName, request.FileContent, location);
            if (!uploadRes.Success) return ApiResponseDto<Guid?>.FailureStatus(uploadRes.Message);

            return ApiResponseDto<Guid?>.SuccessStatus(file.GuidId, FileConstants.DOWLOAD_SUCCESS_MSG);
        }

        private async Task<StoredFiles?> StoreFileInDbAsync(FileRequestDto request, int locationId)
        {
            var file = new StoredFiles();
            file.Add(request.FileName, request.FileContentType, request.FileExtension, locationId);

            _unitOfWork.StoredFilesRepo.Add(file);

            if (await _unitOfWork.SaveAsync())
                return null;

            return file;
        }

        public async Task<ApiResponseDto<bool>> MarkFileAsProcessByIdAsync(Guid Id)
        {
            var file = await _unitOfWork.StoredFilesRepo.Table
                        .FirstOrDefaultAsync(s => s.GuidId == Id && !s.IsProcessed && !s.IsDeleted);

            if (file is null)
                return ApiResponseDto<bool>.FailureStatus(FileConstants.PROCESSING_FAILED_MSG);

            file.MarkAsProcessed();
            _unitOfWork.StoredFilesRepo.Update(file);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto<bool>.FlagStatus(saved, FileConstants.PROCESSING_SUCCESS_MSG);
        }

        public async Task<ApiResponseDto<bool>> ProcessFileMaintenanceAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _unitOfWork.SystemSettingsRepo.TableNoTracking.Where(x => x.SettingName.StartsWith("FileProcess")).ToListAsync(cancellationToken);

            var batchSize = settings.GetSystemSetting("FileBatchSize", 50);
            var retentionDays = settings.GetSystemSetting("FileRetentionDays", r => DateTime.Now.Date.AddDays(-r), 10);

            var files = await _unitOfWork.StoredFilesRepo.Table
                        .Where(s => !s.IsProcessed && !s.IsDeleted && s.CreatedDate < retentionDays)
                        .OrderBy(s => s.CreatedDate)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);

            foreach(var file in files)
            {
                var location = await _unitOfWork.FileLocationConfigurationsRepo.TableNoTracking
                                .Where(x => x.Id == file.FileLocationId)
                                .Select(l => new FileLocationConfigDto
                                {
                                    ConfigName = l.ConfigName,
                                    ConfigJson = l.ConfigJson,
                                })
                                .FirstOrDefaultAsync(cancellationToken);

                var isUploaded = await _fileStorageFactory
                        .CreateProvider(location)
                        .DeleteAsync(file.FileName, cancellationToken);

                file.DeleteFile();
                _unitOfWork.StoredFilesRepo.Update(file);
                await _unitOfWork.SaveAsync();
            }

            return ApiResponseDto<bool>.FlagStatus(true, FileConstants.MAINTENANCE_PROCESS_SUCCESS_MSG);
        }
    }
}
