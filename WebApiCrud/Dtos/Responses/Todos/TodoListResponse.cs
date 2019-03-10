using System.Collections.Generic;
using WebApiPaginatedCrud.Dtos.Responses.Shared;
using WebApiPaginatedCrud.Entities;
using WebApiPaginatedCrud.Models;

namespace WebApiPaginatedCrud.Dtos.Responses.Todos
{
    public class TodoListResponse : PagedDto
    {
        public IEnumerable<TodoDto> Todos { get; set; }

        public static List<TodoDto> Build(List<Todo> todos)
        {
            List<TodoDto> todoDtos = new List<TodoDto>(todos.Count);

            foreach (var todo in todos)
                todoDtos.Add(TodoDto.Build(todo));


            return todoDtos;
        }
    }
}