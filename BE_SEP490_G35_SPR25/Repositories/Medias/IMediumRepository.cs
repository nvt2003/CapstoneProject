using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Medias
{
    public interface IMediumRepository
    {
        Task<Medium> CreateMediaItemAsync(Medium mediaItem);
        public Task<List<int>> GetIdDeleteMediaEvent(int eventId, List<EventMedium> list);
        public Task<bool> DeleteMediaEvent(int mediaEventId);
    }
}
