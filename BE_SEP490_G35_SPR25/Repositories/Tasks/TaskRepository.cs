
using Google.Apis.Drive.v3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Tasks
{
    public class TaskRepository : ITaskRepository
    {
        private readonly PlanifyContext _context;
        public TaskRepository(PlanifyContext context)
        {
            _context = context;
        }

        public async Task<Models.Task> CreateTaskAsync(Models.Task newTask)
        {
            try
            {
                await _context.Tasks.AddAsync(newTask);
                await _context.SaveChangesAsync();
                return newTask;
            }
            catch (Exception ex)
            {
                
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<Models.Task>> SearchTaskOrderByStartDateAsync(int page, int pageSize, string? name, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) name = "";

                return await _context.Tasks
                    .Include(t => t.Event)
                    .Where(e => e.TaskName.Contains(name.Trim()) &&
                                e.StartTime >= startDate &&
                                e.Deadline <= endDate &&
                                e.Status == 1)
                    .OrderBy(e => e.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(); 
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while searching tasks.", ex);
            }
        }

        public  PageResultDTO<Models.Task> GetAllTasks(int eventId, int page, int pageSize)
        {
            try
            {
                var count = _context.Tasks.Where(t => t.EventId == eventId && t.Status==1).Count();
                if (count == 0)
                    return new PageResultDTO<Models.Task>(new List<Models.Task>(), count, page, pageSize);
                var tasks = _context.Tasks
                    //.Include(t => t.CreateByNavigation)
                    //.Include(t => t.Event)
                    //.Include(t => t.SubTasks).ThenInclude(st => st.CreateByNavigation)
                    .Where(t => t.EventId == eventId && t.Status==1)
                    //.OrderBy(t => t.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                PageResultDTO<Models.Task> result = new PageResultDTO<Models.Task>(tasks, count, page, pageSize);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving tasks.", ex);
            }
        }


        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(taskId);
                var subTasks = await _context.SubTasks.Where(e => e.TaskId == taskId).ToListAsync();
                if (task == null)
                {
                    return false;
                }
                _context.SubTasks.RemoveRange(subTasks);
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
        public async Task<Models.Task?> UpdateTaskAsync(int taskId, Models.Task updatedTask)
        {
            try
            {
                var existingTask = await _context.Tasks.FindAsync(taskId);
                if (existingTask == null)
                {
                    return null;
                }

                existingTask.TaskName = updatedTask.TaskName;
                existingTask.TaskDescription = updatedTask.TaskDescription;
                existingTask.StartTime = updatedTask.StartTime;
                existingTask.Deadline = updatedTask.Deadline;
                existingTask.AmountBudget = updatedTask.AmountBudget;
                existingTask.EventId = updatedTask.EventId;
                _context.Tasks.Update(existingTask);
                await _context.SaveChangesAsync();
                return existingTask;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
        public async Task<Models.Task?> GetTaskByIdAsync(int taskId)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return null;
                }
                return task; 
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving the task.", ex);
            }
        }

        public bool IsTaskExists(int taskId)
        {
            return _context.Tasks.Any(g => g.Id == taskId);
        }

        public bool UpdateActualTaskAmount(int taskId, decimal amount)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
                task.AmountBudget = amount;
                _context.Update(task);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Models.Task GetTaskById(int taskId)
        {
            try
            {
                var task = _context.Tasks
                    .Include(t => t.CreateByNavigation)
                    .Include(t => t.Event)
                    .Include(t => t.SubTasks).ThenInclude(st => st.CreateByNavigation)
                    .FirstOrDefault(t => t.Id == taskId);
                return task;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> changeStatus(int taskId, int status)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
                task.Status = status;
                _context.Update(task);
                await _context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<PageResultDTO<SubTask>> SearchSubTaskByImplementerId(int page, int pageSize, Guid implementerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var count = _context.JoinTasks
                       .Where(jt => jt.UserId == implementerId &&
                                   jt.Task.StartTime <= endDate &&
                                   (jt.Task.Deadline >= startDate || jt.Task.Deadline == null) &&
                                   jt.Task.Status == 1)
                       .Select(jt => jt.Task)
                    .OrderBy(e => e.StartTime)
                    .Skip((page - 1) * pageSize).Take(pageSize)
                    .Count();
                var result = await _context.JoinTasks
                       .Where(jt => jt.UserId == implementerId &&
                                   jt.Task.StartTime <= endDate &&
                                   (jt.Task.Deadline >= startDate || jt.Task.Deadline == null) &&
                                   jt.Task.Status == 1)
                       .Select(jt => jt.Task)
                    .OrderBy(e => e.StartTime)
                    .Skip((page - 1) * pageSize).Take(pageSize)
                    .ToListAsync();
                return new PageResultDTO<SubTask>(result, count, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteTaskV2(int taskId)
        {
            try
            {
                var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
                task.Status = -1;
                _context.Update(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Guid>> GetJoinedIdByTaskId(int taskId)
        {
            try
            {
                var listSubtaskIds = _context.SubTasks
                                    .Where(s => s.TaskId == taskId)
                                    .Select(s => s.Id)
                                    .ToList();
                var userIds = await _context.JoinTasks
                                    .Where(jt => listSubtaskIds.Contains(jt.TaskId))
                                    .Select(jt => jt.UserId)
                                    .Distinct()
                                    .ToListAsync();
                return userIds;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
