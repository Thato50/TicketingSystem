using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxFusion.TicketingSystem.Common.Tests.Tickets
{
    internal static class ApplicationServiceTestHelpers
    {
        public static IUnitOfWorkManager CreateUnitOfWorkManager()
        {
            var unitOfWork = new Mock<IActiveUnitOfWork>();
            unitOfWork
                .Setup(uow => uow.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var unitOfWorkManager = new Mock<IUnitOfWorkManager>();
            unitOfWorkManager
                .SetupGet(manager => manager.Current)
                .Returns(unitOfWork.Object);

            return unitOfWorkManager.Object;
        }

        public static Mock<IRepository<TEntity, Guid>> CreateRepository<TEntity>(List<TEntity> items)
            where TEntity : class, IEntity<Guid>
        {
            var repository = new Mock<IRepository<TEntity, Guid>>();

            repository
                .Setup(repo => repo.GetAll())
                .Returns(() => items.AsQueryable());

            repository
                .Setup(repo => repo.GetAllListAsync())
                .ReturnsAsync(() => items.ToList());

            repository
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => items.FirstOrDefault(item => item.Id == id));

            repository
                .Setup(repo => repo.InsertAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) =>
                {
                    items.Add(entity);
                    return entity;
                });

            repository
                .Setup(repo => repo.UpdateAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) => entity);

            repository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
                .Returns((Guid id) =>
                {
                    var entity = items.FirstOrDefault(item => item.Id == id);
                    if (entity != null)
                        items.Remove(entity);

                    return Task.CompletedTask;
                });

            return repository;
        }
    }
}
