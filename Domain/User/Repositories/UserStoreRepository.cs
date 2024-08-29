using Models = DotNetService.Models;
using System.Linq;
using DotNetService.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DotNetService.Domain.User.Repositories
{
    public class UserStoreRepository(
        Models.IamDBContext context,
        UserQueryRepository userQueryRepository
    )
    {
        private readonly Models.IamDBContext _context = context;
        private readonly UserQueryRepository _userQueryRepository = userQueryRepository;

        public Models.User Create(Models.User data)
        {
            return this.Save(data);
        }

        public Models.User Update(Guid id, Models.User newData)
        {
            newData.Id = id;
            return this.Save(newData, true);
        }

        public void Delete(Guid id)
        {
            Models.User data = _userQueryRepository.FindOneById(id, true);

            _context.Users.Remove(data);
            int affectedRows = _context.SaveChanges();

            if (affectedRows == 0)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private Models.User Save(Models.User data, bool isUpdate = false)
        {

            if (!isUpdate)
            {
                _userQueryRepository.FindOneByEmail(data.Email, true);

                var dataCreated = _context.Users.Add(data);
                _context.SaveChanges();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Users.Update(data);
                _context.SaveChanges();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("User with id " + data.Id + " not found.");
            }
        }
    }
}