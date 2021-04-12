using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(INotificationService), Lifetime.Scoped)]
    public class NotificationService : INotificationService {

        readonly INotificationRepository _notificationRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly IProcessRepository _processRepository;
        readonly IConfiguration _config;

        public NotificationService(IConfiguration config, INotificationRepository notificationRepository, IEntityRepository entityRepository, IProcessRepository processRepository) {
            _notificationRepository = notificationRepository;
            _entityRepository = entityRepository;
            _processRepository = processRepository;
            _config = config;
        }

        public async Task<QueryResult<Notification>> ListAsync(Query query) {
            return await _notificationRepository.ListAsync(query);
        }

        public async Task<bool> AddAsync(Notification notification) {
            var ok = await _notificationRepository.InsertAsync(notification);
            return ok;
        }

        public async Task<bool> MarkAsReadOrUnreadAsync(int id) {
            return await _notificationRepository.MarkAsReadOrUnreadAsync(id);
        }
        public async Task<bool> DeleteAllReadAsync(int userId) {
            return await _notificationRepository.DeleteAllReadAsync(userId);
        }
    }
}
