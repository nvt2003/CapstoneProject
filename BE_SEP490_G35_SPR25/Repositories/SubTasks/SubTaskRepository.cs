
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using System.Threading.Tasks;


public class SubTaskRepository : ISubTaskRepository
    {
        private readonly PlanifyContext _context;
    public SubTaskRepository(PlanifyContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Create a new subtask
    /// </summary>
    /// <param name="newSubTask"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async  Task<SubTask> CreateSubTaskAsync(SubTask newSubTask)
    {
        try
        {
            await _context.SubTasks.AddAsync(newSubTask);
            await _context.SaveChangesAsync();
            return newSubTask;
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred.", ex);
        }
    }
    public bool UpdateActualSubTaskAmount(int subTaskId, decimal amount)
    {
        try
        {
            var task = _context.SubTasks.FirstOrDefault(st => st.Id == subTaskId);
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
    /// <summary>
    /// Update a subtask
    /// </summary>
    /// <param name="subTaskId"></param>
    /// <param name="updatedSubTask"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SubTask?> UpdateSubTaskAsync(int subTaskId, SubTask updatedSubTask)
    {
        try
        {
            var existingSubTask = await _context.SubTasks.FindAsync(subTaskId);
            if (existingSubTask == null)
            {
                return null; 
            }
            existingSubTask.SubTaskName = updatedSubTask.SubTaskName;
            existingSubTask.SubTaskDescription = updatedSubTask.SubTaskDescription;
            existingSubTask.Status = updatedSubTask.Status;
            existingSubTask.StartTime = updatedSubTask.StartTime;
            existingSubTask.Deadline = updatedSubTask.Deadline;
            existingSubTask.AmountBudget = updatedSubTask.AmountBudget;
            _context.SubTasks.Update(existingSubTask);
            await _context.SaveChangesAsync();

            return existingSubTask;
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while updating the subtask.", ex);
        }
    }
    /// <summary>
    /// Update subtask status
    /// </summary>
    /// <param name="subTaskId"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> UpdateSubTaskStatusAsync(int subTaskId, int newStatus)
    {
        try
        {
            var existingSubTask = await _context.SubTasks.FindAsync(subTaskId);
            if (existingSubTask == null)
            {
                return false; // Không tìm thấy SubTask
            }

            existingSubTask.Status = newStatus;
            _context.SubTasks.Update(existingSubTask);
            await _context.SaveChangesAsync();

            return true; // Cập nhật thành công
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while updating the subtask status.", ex);
        }
    }

    /// <summary>
    /// Delete a subtask
    /// </summary>
    /// <param name="subTaskId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> DeleteSubTaskAsync(int subTaskId)
    {
        try
        {
            var subTask = await _context.SubTasks.FindAsync(subTaskId);
            if (subTask == null)
            {
                return false;
            }

            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while deleting the subtask.", ex);
        }
    }
    /// <summary>
    /// Get a subtask by ID
    /// </summary>
    /// <param name="subTaskId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SubTask?> GetSubTaskByIdAsync(int subTaskId)
    {
        try
        {
            return await _context.SubTasks
                .Include(sb=>sb.Task).ThenInclude(t=>t.Event)
                .FirstOrDefaultAsync(sb=>sb.Id == subTaskId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error while retrieving sub-task.", ex);
        }
    }
    /// <summary>
    /// Get subtasks by task ID
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<SubTask>> GetSubTasksByTaskIdAsync(int taskId)
    {
        try
        {
            return await _context.SubTasks
                .Where(st => st.TaskId == taskId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving sub-tasks by taskId.", ex);
        }
    }

    public async Task<bool> AssignSubTask(JoinTask joinTask)
    {
        try
        {
            _context.JoinTasks.Add(joinTask);
            await _context.SaveChangesAsync();
            return true;
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<bool> DeleteAssignedUser(Guid userId, int subTaskId)
    {
        try
        {
            var joinTask = await _context.JoinTasks.FirstOrDefaultAsync(jt => jt.UserId == userId && jt.TaskId == subTaskId);
            if (joinTask != null)
            {
                _context.JoinTasks.Remove(joinTask);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<List<Guid>> GetJoinedIdBySubTaskIdAsync(int subtaskId)
    {
        try
        {
            return await _context.JoinTasks
                .Where(jt=>jt.TaskId == subtaskId)
                .Select(jt=>jt.UserId)
                .ToListAsync();
                
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> GetEventIdBySubtaskId(int subtaskId)
    {
        try
        {
            return await _context.Events
                .Include(e => e.Tasks).ThenInclude(t => t.SubTasks)
                .Where(e => e.Tasks.Any(t => t.SubTasks.Any(s => s.Id == subtaskId)))
                .Select(e=>e.Id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PageResultDTO<SubTask>> SearchSubTaskByImplementerId(Guid implementerId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var count = _context.JoinTasks
                   .Where(jt => jt.UserId == implementerId &&
                               jt.Task.StartTime <= endDate &&
                               (jt.Task.Deadline >= startDate || jt.Task.Deadline == null) &&
                               jt.Task.Status != -1)
                   .Select(jt => jt.Task)
                .OrderBy(e => e.StartTime)
                .Count();
            var result = await _context.JoinTasks
                   .Include(jt => jt.Task).ThenInclude(st => st.Task).ThenInclude(t => t.Event)
                   .Where(jt => jt.UserId == implementerId && 
                               jt.Task.StartTime <= endDate &&
                               (jt.Task.Deadline >= startDate || jt.Task.Deadline == null) &&
                               jt.Task.Status != -1)
                .OrderBy(e => e.Task.StartTime)
                .Select(jt => new SubTask
                {
                    Id = jt.Task.Id,
                    SubTaskName = jt.Task.SubTaskName,
                    SubTaskDescription = jt.Task.SubTaskDescription,
                    StartTime = jt.Task.StartTime,
                    Deadline = jt.Task.Deadline,
                    TaskId = jt.Task.TaskId,
                    AmountBudget = jt.Task.AmountBudget,
                    Status = jt.Task.Status,
                    CreateBy = jt.Task.CreateBy,
                    Task = jt.Task.Task,
                })
                .ToListAsync();
            return new PageResultDTO<SubTask>(result, count, 0, 0);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}


