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
            try
            {
                Models.User data = new Models.User { Id = id };
                _context.Users.Attach(data);
                _context.Users.Remove(data);
                _context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
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
                throw new UnprocessableEntityException("No data was updated.");
            }
        }
    }
}