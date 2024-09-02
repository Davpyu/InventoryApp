using System.Data.Entity.Infrastructure;
using DotNetService.Exceptions;
using Models = DotNetService.Models;

namespace DotNetService.Domain.Permission.Repositories
{
    public class PermissionStoreRepository(
        PermissionQueryRepository permissionQueryRepository,
        Models.IamDBContext context
    )
    {
        private readonly PermissionQueryRepository _permissionQueryRepository = permissionQueryRepository;
        private readonly Models.IamDBContext _context = context;

        public Models.Permission Create(Models.Permission permissionRepository)
        {
            Models.Permission newPermission = new()
            {
                Name = permissionRepository.Name
            };

            return this.Save(newPermission);
        }

        public void Update(Guid id, Models.Permission permissionRepository)
        {
            Models.Permission oldPermission = _permissionQueryRepository.Find(id);
            if (oldPermission == null)
            {
                return;
            }

            oldPermission.Name = permissionRepository.Name;
            this.Save(oldPermission, true);
        }

        public void Delete(Guid id)
        {
            try
            {
                Models.Permission data = new Models.Permission { Id = id };
                _context.Permissions.Attach(data);
                _context.Permissions.Remove(data);
                _context.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException("No data was deleted.");
            }
        }

        private Models.Permission Save(Models.Permission data, bool isUpdate = false)
        {
            if (!isUpdate)
            {

                var dataCreated = _context.Permissions.Add(data);
                _context.SaveChanges();
                return dataCreated.Entity;
            }

            try
            {
                var dataUpdated = _context.Permissions.Update(data);
                _context.SaveChanges();

                return dataUpdated.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new UnprocessableEntityException(
                    "No data was updated."
                );
            }
        }

        public void BulkSave(Models.Permission[] data)
        {
            _context.Permissions.AddRange(data);
            _context.SaveChanges();
        }
    }
}