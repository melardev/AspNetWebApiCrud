using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Mvc;
using WebApiPaginatedCrud.Dtos.Responses.Shared;
using WebApiPaginatedCrud.Dtos.Responses.Todos;
using WebApiPaginatedCrud.Entities;
using WebApiPaginatedCrud.Enums;
using WebApiPaginatedCrud.Infrastructure.Services;
using WebApiPaginatedCrud.Models;
using System.Net.Http;
using System.Net;

namespace WebApiPaginatedCrud.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.RoutePrefix("api/todos")]
    public class TodosController : ApiController
    {
        private readonly TodoService _todosService;


        public TodosController()
        {
            _todosService = new TodoService();
        }

        [System.Web.Http.HttpGet]
        public async Task<JsonResult<List<TodoDto>>> GetTodos()
        {
            var todos = await _todosService.FetchMany(TodoShow.All);
            return Json(TodoListResponse.Build(todos),
                GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("pending")]
        public async Task<ResponseMessageResult> GetPending()
        {
            List<Todo> todos = await _todosService.FetchMany(TodoShow.Pending);
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, TodoListResponse.Build(todos),
                GlobalConfiguration.Configuration.Formatters.JsonFormatter));
            // return Ok(TodoListResponse.Build(todos)); <-- also works
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("completed")]
        public async Task<JsonResult<List<TodoDto>>> GetCompleted()
        {
            var todos = await _todosService.FetchMany(TodoShow.Completed);
            return Json(TodoListResponse.Build(todos), GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public async Task<HttpResponseMessage> GetTodoDetails(int id)
        {
            var todo = await _todosService.Get(id);
            if (todo != null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ObjectContent(typeof(Todo), todo,
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter);
                return response;
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new ObjectContent(typeof(ErrorDtoResponse), new ErrorDtoResponse("Not found"),
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter);
                return response;
            }
        }


        [System.Web.Http.HttpPost]
        public async Task<CreatedNegotiatedContentResult<Todo>> CreateTodo([FromBody] Todo todo)
        {
            await _todosService.CreateTodo(todo);
            return Created((string) "/api/todos/" + todo.Id, todo);
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("{id}")]
        public async Task<IHttpActionResult> UpdateTodo(int id, [FromBody] Todo todoFromUser)
        {
            Todo todoFromDb = await _todosService.Get(id);
            if (todoFromDb != null)
            {
                return Ok(await _todosService.Update(todoFromDb, todoFromUser));
            }
            else
            {
                // return NotFound(); <-- returns empty response
                return new ResponseMessageResult(new HttpResponseMessage
                {
                    Content = new ObjectContent(typeof(object), new
                    {
                        Success = false,
                        FullMessages = new string[] {"Not Found"}
                    }, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
                    StatusCode = HttpStatusCode.NotFound
                });
            }
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id:int}")]
        public async Task<HttpResponseMessage> DeleteTodo(int id)
        {
            var todo = await _todosService.Get(id);
            if (todo != null)
            {
                await _todosService.Delete(id);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ErrorDtoResponse("Todo not Found"),
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter);
            }
        }


        [System.Web.Http.HttpDelete]
        public async Task<ResponseMessageResult> DeleteAll()
        {
            await _todosService.DeleteAll();
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}