using SafeIt.Entities;
using SafeIt.Models;
using File = SafeIt.Entities.File;

namespace SafeIt.Services
{
    public interface IFileService
    {
        Task<bool> AddFile(AddFile addFile);
        DataTableResponse AllFiles(string? draw, int size, int pagesize);
        bool IsExist(string name, string key);
        bool IsExist(string name);
    }

    public class FileService : IFileService
    {
        private readonly ApplicationDBContext _context;
        public FileService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<bool> AddFile(AddFile addFile)
        {
            var isExist = _context.Files.FirstOrDefault(x => x.Name == addFile.Name);
            if (isExist != null) return true;
            await _context.Files.AddAsync(new File
            {
                Name = addFile.Name,
                Key = addFile.Key,
                Type = addFile.Name.Split('.')[1].Replace(".", "")
            });
            int isSaved = await _context.SaveChangesAsync();
            if(isSaved > 0) return true;
            return false;
        }

        public DataTableResponse AllFiles(string? draw, int skip, int pagesize)
        {
            var data = _context.Files.Skip(skip).Take(pagesize).Select(x => new FileViewModel {Name = x.Name, Key = x.Key, Type = x.Type });

            int recordsTotal = _context.Files.Count();
            DataTableResponse dataTableResponse = new DataTableResponse()
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            };

            return dataTableResponse;
        }

        public bool IsExist(string name, string key)
        {
            var isExist = _context.Files.FirstOrDefault(x => x.Name == name&& x.Key == key);
            if (isExist != null) return true;
            return false;
        }

        public bool IsExist(string name)
        {
            var isExist = _context.Files.FirstOrDefault(x => x.Name == name);
            if (isExist != null) return true;
            return false;
        }
    }
}
