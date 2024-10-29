

using DotNetService.Constants.Storage;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Infrastructure.Shareds
{

    public class StorageService
    {

        private readonly IConfiguration _config;

        public StorageService(
            IConfiguration config,
            IHttpContextAccessor context
        )
        {
            _config = config;

            var storage = _config["Storage"] ?? StorageConstant.LOCAL;
            if (storage == StorageConstant.MINIO && bool.Parse(_config["Minio:IsEnable"] ?? "false"))
            {
                // TODO: need to implement minio integration
            }
        }

        public string GetDownloadFileUrl(string fileUrl)
        {
            var storage = _config["Storage"] ?? StorageConstant.LOCAL;

            switch (storage)
            {
                case StorageConstant.AWS:
                    // TODO Need to be code here if going to use AWS
                    break;
                case StorageConstant.LOCAL:
                    var url = "{0}{1}/{2}";
                    return string.Format(url, _config["App:BaseURL"] ?? "http://localhost:5001", _config["LocalStorage:VirtualPath"] ?? "/temp", fileUrl);
                case StorageConstant.MINIO:
                    // TODO Need to be code here if going to use AWS
                    break;
            }

            throw new FileNotFoundException("Not Found");
        }

        public List<string> GenerateFileNames(IFormFile[] files)
        {
            var fileUrls = new List<string>();
            foreach (var fileObject in files)
            {
                var fileDestinationPath = GenerateFileName(fileObject);
                var storage = _config["Storage"] ?? StorageConstant.LOCAL;

                var fileUrl = "";
                switch (storage)
                {
                    case StorageConstant.AWS:
                        // TODO Need to be code here if going to use AWS
                        break;
                    case StorageConstant.LOCAL:

                        fileUrl = _config["App:BaseURL"] + "/storage/download?fileUrl=" + fileDestinationPath;
                        break;
                    case StorageConstant.MINIO:
                        if (!bool.Parse(_config["Minio:IsEnable"] ?? "false")) throw new BusinessException("Minio disabled");
                        fileUrl = (bool.Parse(_config["Minio:IsUseSSL"] ?? "false") ? "https://" : "http://") + _config["Minio:Endpoint"] + "/" + _config["Minio:Bucket"] + "/" + fileDestinationPath;
                        break;
                }

                fileUrls.Add(fileUrl);
            }

            return fileUrls;
        }

        public string GenerateFileName(IFormFile fileObject)
        {
            var extension = Utils.GetFileExtension(fileObject);
            return "/uploads/file-" + Utils.RandStr(10).ToLower() + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-uploaded." + extension;
        }

        public string MoveFile(IFormFile file, string destPath = null)
        {
            destPath ??= GenerateFileName(file);
            var storage = _config["Storage"] ?? StorageConstant.LOCAL;

            switch (storage)
            {
                case StorageConstant.AWS:
                    // TODO Need to be code here if going to use AWS
                    break;
                case StorageConstant.LOCAL:
                    var localStorageRootPath = _config["LocalStorage:RootPath"] ?? "storage";
                    var filePath = Utils.MoveFileToStorage(file, localStorageRootPath, "uploads");

                    return GetDownloadFileUrl(filePath);
                case StorageConstant.MINIO:
                    // TODO: need to implement minio integration
                    break;
            }

            return null;
        }

        public async Task<List<string>> MoveFiles(IFormFile[] files, List<string> defaultFileUrls = null)
        {

            var storage = _config["Storage"] ?? StorageConstant.LOCAL;
            switch (storage)
            {
                case StorageConstant.AWS:
                    // TODO Need to be code here if going to use AWS
                    break;
                case StorageConstant.LOCAL:
                    var localStorageRootPath = _config["LocalStorage:RootPath"] ?? "storage";
                    var fileUrls = defaultFileUrls ?? new List<string>();
                    foreach (var file in files)
                    {
                        var filePath = await Task.Factory.StartNew(() =>
                        {
                            return Utils.MoveFileToStorage(file, localStorageRootPath, "uploads");
                        });

                        if (defaultFileUrls == null)
                        {
                            var downloadableUrl = GetDownloadFileUrl(filePath);
                            fileUrls.Add(downloadableUrl);
                        }
                    }
                    break;
                case StorageConstant.MINIO:
                    if (!bool.Parse(_config["Minio:IsEnable"] ?? "false")) throw new BusinessException("Minio disabled");
                    // TODO: need to implement minio integration
                    break;
            }

            return null;
        }
    }
}