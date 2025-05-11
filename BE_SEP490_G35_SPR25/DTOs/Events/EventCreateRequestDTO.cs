using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planify_BackEnd.DTOs.Events
{
    public class EventCreateRequestDTO
    {
        [Required]
        public string EventTitle { get; set; }

        public string EventDescription { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int CategoryEventId { get; set; }

        public string Placed { get; set; }

        public string MeasuringSuccess { get; set; }

        public string Goals { get; set; }

        public string MonitoringProcess { get; set; }

        [Range(0, int.MaxValue)]
        public int SizeParticipants { get; set; }

        public string PromotionalPlan { get; set; }

        public string TargetAudience { get; set; }

        public string SloganEvent { get; set; }

        public List<TaskCreateEventDTO> Tasks { get; set; }

        public List<RiskCreateEventDTO> Risks { get; set; }
        public List<ActivityEventDTO> Activities { get; set; }

        public List<CostBreakdownCreateEventDTO> CostBreakdowns { get; set; }
    }

    public class TaskCreateEventDTO
    {
        [Required]
        public string TaskName { get; set; }

        public string Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? Deadline { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }

        public List<SubTaskCreateEventDTO> SubTasks { get; set; }
    }

    public class SubTaskCreateEventDTO
    {
        [Required]
        public string SubTaskName { get; set; }

        public string Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? Deadline { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Budget { get; set; }
    }

    public class RiskCreateEventDTO
    {
        [Required]
        public string Name { get; set; }

        public string Reason { get; set; }

        public string Solution { get; set; }

        public string Description { get; set; }
    }

    public class CostBreakdownCreateEventDTO
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? PriceByOne { get; set; }
    }
    public class ActivityEventDTO
    {
        [Required]
        public string Name { get; set; }

        public string Content { get; set; }
    }
}