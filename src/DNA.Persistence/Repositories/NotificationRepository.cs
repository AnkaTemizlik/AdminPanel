using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Repositories;
using DNA.Domain;
using DNA.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using DNA.Domain.Extentions;

namespace DNA.Persistence.Repositories {

    [Service(typeof(INotificationRepository), Lifetime.Scoped)]
    public class NotificationRepository : BaseRepository, INotificationRepository {

        private readonly IEntityRepository _entityRepository;

        public NotificationRepository(IAppDbContext context, IEntityRepository entityRepository) : base(context) {
            this._entityRepository = entityRepository;
        }

        public async Task<QueryResult<Notification>> ListAsync(Query query) {
            var q = new EntityQuery(query, nameof(Notification));
            var sql = Context.SetTablePrefix(TableScripts.Notification_Select);
            return await _entityRepository.QueryAsync<Notification>(q, sql, null);
        }

        public async Task<bool> InsertAsync(Notification notification) {
            using var connection = Context.Connection;
            var result = await connection.ExecuteAsync(Context.SetTablePrefix(TableScripts.Notification_AddOrUpdate), notification);
            return result > 0;
        }


        public async Task<bool> MarkAsReadOrUnreadAsync(int id) {
            using var connection = Context.Connection;
            var result = await connection.ExecuteAsync(Context.SetTablePrefix(TableScripts.Notification_MarkAsReadOrUnread),
                new { Id = id });
            return result > 0;
        }

        public async Task<bool> DeleteAllReadAsync(int userId) {
            using var connection = Context.Connection;
            var result = await connection.ExecuteAsync(Context.SetTablePrefix(TableScripts.Notification_DeleteAllRead),
                new { CurrentUserId = userId });
            return result > 0;
        }
    }
}
