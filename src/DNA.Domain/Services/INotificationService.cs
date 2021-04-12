using DNA.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DNA.Domain.Services {
    public interface INotificationService {
        Task<QueryResult<Notification>> ListAsync(Query query);
        Task<bool> AddAsync(Notification notification);
        Task<bool> MarkAsReadOrUnreadAsync(int id);
        Task<bool> DeleteAllReadAsync(int userId);
    }
}
