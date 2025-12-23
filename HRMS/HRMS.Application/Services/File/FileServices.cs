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
using System.Threading;

namespace HRMS.Application.Services.File
{
    public class FileServices:IFileServices
    {
        private readonly IFileStorageFactory _fileStorageFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageCompressor _imageCompressor;
        public FileServices(IFileStorageFactory fileStorageFactory, IUnitOfWork unitOfWork, IImageCompressor imageCompressor)
        {
            _fileStorageFactory = fileStorageFactory;
            _unitOfWork = unitOfWork;
            _imageCompressor = imageCompressor;
        }
        private async Task<FileFetchConfigDto> GetFileLocationConfigAsync(int fileId)
        {
            var file = await _unitOfWork.StoredFilesRepo.Table
                        .FirstOrDefaultAsync(s => s.Id == fileId && !s.IsDeleted);

            var location = file is null
                ? throw new NullReferenceException(FileConstants.NO_STORAGE_CONFIG_MSG)
                : await GetStorageLocationConfigAsync(file.FileLocationId);

            return new FileFetchConfigDto{ FileName = file.FileName, FileContentType = file.FileContentType, FileExtension = file.FileExtension, locationConfig = location };
        }
        private async Task<FileLocationConfigDto> GetFileLocationConfigAsync()
        {
            var setting = await _unitOfWork.SystemSettingsRepo.TableNoTracking.FirstOrDefaultAsync(x => x.SettingKey == "FileStorageLocation");

            if (!int.TryParse(setting?.SettingValue, out int locationId))
                throw new NullReferenceException(FileConstants.NO_STORAGE_CONFIG_MSG);

            return await GetStorageLocationConfigAsync(locationId);
        }

        #region File Retrieval
        private async Task<FileLocationConfigDto> GetStorageLocationConfigAsync(int locationId)
        {
            var location = await _unitOfWork.FileLocationConfigurationsRepo.TableNoTracking
                                .Where(x => x.Id == locationId)
                                .Select(l => new FileLocationConfigDto
                                {
                                    ConfigName = l.ConfigName,
                                    ConfigJson = l.ConfigJson,
                                    ProviderType = l.ProviderType,
                                    Id = l.Id
                                })
                                .FirstOrDefaultAsync();

            return location is null
                ? throw new NullReferenceException(FileConstants.NO_STORAGE_CONFIG_MSG)
                : location;
        }
        public async Task<string> GetFileByStoredFileIdAsync(int storedFileId)
        {
            var file = await _unitOfWork.StoredFilesRepo.Table
                        .FirstOrDefaultAsync(s => s.Id == storedFileId && !s.IsDeleted);

            var location = file is null
                ? throw new NullReferenceException(FileConstants.NO_STORAGE_CONFIG_MSG)
                : await GetStorageLocationConfigAsync(file.FileLocationId);

            return await RetrieveFileFromStorageAsync(file.FileName, location);
        }
        public async Task<FileResponseDto> GetFileBytesByStoredFileIdAsync(int storedFileId, bool thumb = false)
        {
            var file = await _unitOfWork.StoredFilesRepo.Table
                        .FirstOrDefaultAsync(s => s.Id == storedFileId && !s.IsDeleted);

            var location = file is null
                ? throw new NullReferenceException(FileConstants.NO_STORAGE_CONFIG_MSG)
                : await GetStorageLocationConfigAsync(file.FileLocationId);

            var filename = (thumb ? "C_" : string.Empty) + file.GuidId.ToString();

            var bytes = await RetrieveFileBytesFromStorageAsync(filename, location);

            if(bytes.Length == 0)
                bytes = await RetrieveFileBytesFromStorageAsync(file.GuidId.ToString(), location);

            return new FileResponseDto
            {
                FileName = file.FileName,
                FileContentType = file.FileContentType,
                FileExtension = file.FileExtension,
                FileContent = bytes
            };
        }

        public async Task<string> RetrieveFileFromStorageAsync(string filename, FileLocationConfigDto configDto, CancellationToken cancellationToken = default)
        {
            var fileBytes = await _fileStorageFactory
                        .CreateProvider(configDto)
                        .FetchAsync(filename, cancellationToken);

            if (fileBytes is null || fileBytes.Length == 0)
                return string.Empty;

            return Convert.ToBase64String(fileBytes);
        }
        public async Task<byte[]> RetrieveFileBytesFromStorageAsync(string filename, FileLocationConfigDto configDto, CancellationToken cancellationToken = default)
        {
            var fileBytes = await _fileStorageFactory
                        .CreateProvider(configDto)
                        .FetchAsync(filename, cancellationToken);

            if (fileBytes is null || fileBytes.Length == 0)
                return [];

            return fileBytes;
        }

        #endregion File Retrieval

        #region File Uploads
        public async Task<ApiResponseDto> UploadFileToStorageAsync(string filename, byte[]? filebytes, FileLocationConfigDto configDto)
        {
            if (filebytes is null || filebytes.Length == 0)
                return ApiResponseDto.FailureStatus(FileConstants.CONTENT_EMPTY_MSG);

            using var memoryStream = new MemoryStream(filebytes);

            var isUploaded = await _fileStorageFactory
                        .CreateProvider(configDto)
                        .UploadAsync(filename, memoryStream);

            var thumb = await UploadThumbFileToStorageAsync(filename, filebytes, configDto);

            return ApiResponseDto.FlagStatus(isUploaded, isUploaded ? FileConstants.UPLOAD_SUCCESS_MSG : FileConstants.UPLOAD_FAILED_MSG);
        }
        private async Task<ApiResponseDto> UploadThumbFileToStorageAsync(string filename, byte[] filebytes, FileLocationConfigDto configDto, CancellationToken cancellationToken = default)
        {
            var compressedBytes = _imageCompressor.Compress(filebytes, 500000, true);
            using var memoryStream = new MemoryStream(compressedBytes);
            var isUploaded = await _fileStorageFactory
                        .CreateProvider(configDto)
                        .UploadAsync("C_" + filename, memoryStream, cancellationToken);

            return ApiResponseDto.FlagStatus(isUploaded, isUploaded ? FileConstants.UPLOAD_SUCCESS_MSG : FileConstants.UPLOAD_FAILED_MSG);
        }
        public async Task<ApiResponseDto> UploadFileAsync(FileRequestDto request)
        {
            var location = await GetFileLocationConfigAsync();

            var file = await StoreFileInfoInDbAsync(request, location.Id);
            if (file is null) return ApiResponseDto.FailureStatus(FileConstants.UPLOAD_FAILED_MSG);

            var uploadRes = await UploadFileToStorageAsync(file.GuidId.ToString(), request.FileContent, location);
            if (!uploadRes.Success) return ApiResponseDto.FailureStatus(uploadRes.Message);

            return ApiResponseDto.SuccessStatus(file.GuidId, FileConstants.UPLOAD_SUCCESS_MSG);
        }

        private async Task<StoredFiles?> StoreFileInfoInDbAsync(FileRequestDto request, int locationId)
        {
            var file = new StoredFiles();
            file.Add(request.FileName, request.FileContentType, request.FileExtension, locationId);

            _unitOfWork.StoredFilesRepo.Add(file);

            if (await _unitOfWork.SaveAsync())
                return file;

            return null;
        }
        #endregion File Uploads

        #region File Processing

        public async Task<ApiResponseDto> MarkFileAsProcessByIdAsync(Guid Id)
        {
            var file = await _unitOfWork.StoredFilesRepo.Table
                        .FirstOrDefaultAsync(s => s.GuidId == Id && !s.IsProcessed && !s.IsDeleted);

            if (file is null)
                return ApiResponseDto.FailureStatus(FileConstants.PROCESSING_FAILED_MSG);

            file.MarkAsProcessed();
            _unitOfWork.StoredFilesRepo.Update(file);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto.FlagStatus(saved, FileConstants.PROCESSING_SUCCESS_MSG);
        }

        public async Task<ApiResponseDto> ProcessFileMaintenanceAsync(CancellationToken cancellationToken = default)
        {
            var settings = await _unitOfWork.SystemSettingsRepo.TableNoTracking.Where(x => x.SettingKey.StartsWith("FileProcess")).ToListAsync(cancellationToken);

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

            return ApiResponseDto.FlagStatus(true, FileConstants.MAINTENANCE_PROCESS_SUCCESS_MSG);
        }
        #endregion File Processing
    }
}
