using Models = DotNetService.Models;
using System.Linq;
using DotNetService.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository
    {
        private readonly Models.IamDBContext _context;
        private readonly UserQueryRepository _userQueryRepository;

        public UserStoreRepository(
            Models.IamDBContext context,
            UserQueryRepository userQueryRepository
        )
        {
            _context = context;
            _userQueryRepository = userQueryRepository;
        }

        public void Create(Models.User data)
        {
            this.Save(data);
        }

        public void Update(Guid id, Models.User newData)
        {
            Models.User oldData = _userQueryRepository.Find(id) ??
            throw new DataNotFoundException("User id " + id + " not found");

            oldData.Name = newData.Name;

            this.Save(oldData, true);
        }

        public void Delete(Guid id)
        {
            Models.User oldData = _userQueryRepository.Find(id) ??
            throw new DataNotFoundException("User id " + id + " not found");

            Models.User data = _context.Users.Where(item => item.Id == id).FirstOrDefault();
            _context.Users.Remove(data);
            _context.SaveChanges();
        }

        private void Save(Models.User data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.Users.Add(data);
            }

            int affectedRows = _context.SaveChanges();

            if (affectedRows == 0)
            {
                throw new UnprocessableEntityException("No affected rows");
            }
        }
    }
}