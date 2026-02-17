using AutoMapper;
using SmartTaskManagement.Application.DTOs.Comment;
using SmartTaskManagement.Application.DTOs.Task;
using SmartTaskManagement.Domain.Entities;

namespace SmartTaskManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>();

        CreateMap<CreateTaskDto, TaskItem>();

        CreateMap<UpdateTaskDto, TaskItem>();

        CreateMap<TaskComment, TaskCommentDto>();
    }
}
