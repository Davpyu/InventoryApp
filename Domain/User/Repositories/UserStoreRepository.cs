using Models = DotNetService.Models;
using System.Linq;
using DotNetService.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
            newData.Id = id;
            this.Save(newData, true);
        }

        public void Delete(Guid id)
        {
            Models.User data = _context.Users.FirstOrDefault(item => item.Id == id) ?? 
            throw new DataNotFoundException("User with id " + id + " not found.");

            _context.Users.Remove(data);
            int affectedRows = _context.SaveChanges();

            if (affectedRows == 0)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private void Save(Models.User data, bool isUpdate = false)
        {
            if (!isUpdate)
            {
                _context.Users.Add(data);
            }

            try
            {
                _context.Users.Update(data);
                int affectedRows = _context.SaveChanges();

                if (affectedRows == 0)
                {
                    throw new UnprocessableEntityException("No data was updated.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DataNotFoundException("User with id " + data.Id + " not found.");
            }
        }
    }
}