using Newtonsoft.Json;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Todo.WebAPI.Models;

namespace Todo.WebAPI.Controllers
{
    public class TodoController : ApiController
    {
        #region field
        private readonly TodoContext _todoContext;
        private readonly IRestClient _client;
        #endregion

        #region Log
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public TodoController(TodoContext todoContext, IRestClient client)
        {
            _todoContext = todoContext;
            _client = client;
            _client.BaseUrl = new Uri(ConfigurationManager.AppSettings["LoginApi"]);
        }
        #endregion

        #region 登录服务
        /// <summary>
        /// Token换取OpenId接口调用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ResponseResult<string> getOpenId(RequestBase model)
        {
#if DEBUG
            return new ResponseResult<string>()
            {
                ErrCode = 0,
                ErrMsg = "success",
                Data = model.Token
            };
#endif
            IRestRequest request = new RestRequest("/GetOpenId", Method.POST);
            request.AddJsonBody(model);

            IRestResponse response = _client.Execute(request);
            return JsonConvert.DeserializeObject<ResponseResult<string>>(response.Content);
        }
        #endregion

        #region 用户
        /// <summary>
        /// 通过OpenId获取UserId
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        private Guid getUserId(string openId)
        {
            try
            {
                if (string.IsNullOrEmpty(openId))
                {
                    throw new ArgumentNullException("OpenId为空");
                }

                var user = (from u in _todoContext.TodoUsers
                            where !string.IsNullOrEmpty(u.OpenId) && u.OpenId == openId
                            select u).FirstOrDefault();

                if (user == null)
                {
                    //未注册，进行注册
                    user = new TodoUser()
                    {
                        TodoUserId = Guid.NewGuid(),
                        OpenId = openId
                    };

                    _todoContext.TodoUsers.Add(user);
                    _todoContext.SaveChanges();

                    return user.TodoUserId;
                }
                else
                {
                    //已注册，返回UserId
                    return user.TodoUserId;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Error(ex.InnerException);
                throw;
            }
        }

        /// <summary>
        /// 检查UserId是否有权限操作该Todo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="todoId"></param>
        /// <returns></returns>
        private bool hasAuthority(Guid userId, Guid todoId)
        {
            try
            {
                var todo = (from t in _todoContext.Todos
                            where t.TodoId == todoId
                            select t).FirstOrDefault();
                if (todo != null && todo.TodoUser.TodoUserId == userId)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        #endregion

        #region Todo
        #region 查询Todo
        /// <summary>
        /// 获取今日Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetTodayTodos([FromBody]RequestBase model)
        {
            try
            {
                //登录接口使用model.UserId获取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                DateTime begin = DateTime.Today;
                DateTime end = DateTime.Today.AddDays(1);
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == userId && !t.IsDone && t.AlertTime != null  && t.AlertTime >= begin && t.AlertTime < end
                                           orderby t.AlertTime
                                           select t).ToList();
                if (todos == null)
                {
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = new List<TodoViewModel>()
                    };
                }

                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertDate = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = item.Content == null ? string.Empty : item.Content,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        UseAlert = item.UseAlert
                    });
                }
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = todoVMs
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 获取3日Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetThreeDayTodos([FromBody]RequestBase model)
        {
            try
            {
                //登录接口使用model.UserId获取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                DateTime begin = DateTime.Today.AddDays(1);
                DateTime end = DateTime.Today.AddDays(4);
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == userId && !t.IsDone && t.AlertTime != null && (t.AlertTime >= begin && t.AlertTime < end)
                                           orderby t.AlertTime
                                           select t).ToList();
                if (todos == null)
                {
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = null
                    };
                }

                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertDate = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = item.Content == null ? string.Empty : item.Content,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        UseAlert = item.UseAlert
                    });
                }
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = todoVMs
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 获取7日Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetSevenDayTodos([FromBody]RequestBase model)
        {
            try
            {
                //登录接口使用model.UserId获取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                DateTime begin = DateTime.Today.AddDays(4);
                DateTime end = DateTime.Today.AddDays(8);
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == userId && !t.IsDone && t.AlertTime != null && (t.AlertTime >= begin && t.AlertTime < end)
                                           orderby t.AlertTime
                                           select t).ToList();
                if (todos == null)
                {
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = null
                    };
                }

                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertDate = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = item.Content == null ? string.Empty : item.Content,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        UseAlert = item.UseAlert
                    });
                }
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = todoVMs
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 获取更多Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetAfterSevenDayTodos([FromBody]RequestBase model)
        {
            try
            {
                //登录接口使用model.UserId获取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                DateTime begin = DateTime.Today.AddDays(8);
                DateTime begin2 = DateTime.Today;
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == userId && !t.IsDone && (t.AlertTime == null || t.AlertTime.Value >= begin || t.AlertTime.Value < begin2)
                                           orderby t.AlertTime
                                           select t).ToList();
                if (todos == null)
                {
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = null
                    };
                }

                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertDate = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = item.Content == null ? string.Empty : item.Content,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        UseAlert = item.UseAlert
                    });
                }
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = todoVMs
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 获取已完成Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetDoneTodos([FromBody]RequestBase model)
        {
            try
            {
                //登录接口使用model.UserId获取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == userId && t.IsDone
                                           orderby t.AlertTime
                                           select t).ToList();
                if (todos == null)
                {
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = null
                    };
                }

                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertDate = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = item.AlertTime == null ? string.Empty : item.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = item.Content == null ? string.Empty : item.Content,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        UseAlert = item.UseAlert
                    });
                }
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = todoVMs
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<List<TodoViewModel>>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }
        #endregion

        #region 添加/编辑/删除
        /// <summary>
        /// 通过ID获取Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<TodoViewModel> GetTodo([FromBody]TodoGetRequest model)
        {
            try
            {
                //换取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<TodoViewModel>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                //权限判定
                if (!hasAuthority(userId, model.TodoId))
                {
                    return new ResponseResult<TodoViewModel>()
                    {
                        ErrCode = 1004,
                        ErrMsg = "没有权限获取该Todo",
                        Data = null
                    };
                }

                //查询Todo
                var todo = (from t in _todoContext.Todos
                            where t.TodoId == model.TodoId
                            select t).FirstOrDefault();
                if (todo == null)
                {
                    return new ResponseResult<TodoViewModel>()
                    {
                        ErrCode = 1005,
                        ErrMsg = "Todo不存在",
                        Data = null
                    };
                }

                return new ResponseResult<TodoViewModel>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = new TodoViewModel()
                    {
                        TodoId = todo.TodoId,
                        AlertBeforeMinutes = todo.AlertBeforeMinutes,
                        AlertDate = todo.AlertTime == null ? string.Empty : todo.AlertTime.Value.ToString("yyyy-MM-dd"),
                        AlertTime = todo.AlertTime == null ? string.Empty : todo.AlertTime.Value.ToString("HH:mm:ss"),
                        Content = todo.Content == null ? string.Empty : todo.Content,
                        Title = todo.Title,
                        UseAlert = todo.UseAlert
                    }
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<TodoViewModel>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 保存Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseResult<Guid?> SaveTodo(TodoSaveRequest model)
        {
            try
            {
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                if (string.IsNullOrEmpty(model.Title))
                {
                    throw new ArgumentNullException("标题为空");
                }

                if (model.TodoId == null)
                {
                    //添加
                    model.TodoId = Guid.NewGuid();
                    //获取用户
                    var user = (from u in _todoContext.TodoUsers
                                where u.TodoUserId == userId
                                select u).FirstOrDefault();
                    //添加Todo
                    var todo = new Models.Todo()
                    {
                        TodoId = model.TodoId.Value,
                        Title = model.Title,
                        Content = model.Content,
                        AlertTime = model.AlertTime,
                        AlertBeforeMinutes = model.AlertBeforeMinutes,
                        UseAlert = model.UseAlert,
                        CreateTime = DateTime.Now,
                        FormId = model.FormId,
                        TodoUser = user
                    };
                    _todoContext.Todos.Add(todo);
                    _todoContext.SaveChanges();

                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = todo.TodoId
                    };
                }
                else
                {
                    //编辑
                    //权限判定
                    if (!hasAuthority(userId, model.TodoId.Value))
                    {
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 1004,
                            ErrMsg = "没有权限编辑该Todo",
                            Data = null
                        };
                    }

                    //查询Todo
                    var todo = (from t in _todoContext.Todos
                                where t.TodoId == model.TodoId
                                select t).FirstOrDefault();
                    if (todo == null)
                    {
                        return new ResponseResult<Guid?>()
                        {
                            ErrCode = 1005,
                            ErrMsg = "Todo不存在",
                            Data = null
                        };
                    }

                    //编辑Todo
                    todo.Title = model.Title;
                    todo.Content = model.Content;
                    todo.AlertBeforeMinutes = model.AlertBeforeMinutes;
                    todo.AlertTime = model.AlertTime;
                    todo.UpdateTime = DateTime.Now;
                    todo.FormId = model.FormId;
                    todo.UseAlert = model.UseAlert;
                    _todoContext.SaveChanges();

                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = todo.TodoId
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseResult<Guid?>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 删除Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<Guid?> DeleteTodo([FromBody]TodoDeleteRequest model)
        {
            try
            {
                //换取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                //权限判定
                if (!hasAuthority(userId, model.TodoId))
                {
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1004,
                        ErrMsg = "没有权限删除该Todo",
                        Data = null
                    };
                }

                //查询Todo
                var todo = (from t in _todoContext.Todos
                            where t.TodoId == model.TodoId
                            select t).FirstOrDefault();
                if (todo == null)
                {
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1005,
                        ErrMsg = "Todo不存在",
                        Data = null
                    };
                }

                //删除Todo
                _todoContext.Todos.Remove(todo);
                _todoContext.SaveChanges();

                return new ResponseResult<Guid?>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = model.TodoId
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<Guid?>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// 设置已完成
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<Guid?> SetDone([FromBody]TodoSetDoneRequest model)
        {
            try
            {
                //换取OpenId
                var result = getOpenId(model);
                if (result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }
                //通过OpenId获取UserId
                Guid userId = getUserId(result.Data);

                //权限判定
                if (!hasAuthority(userId, model.TodoId))
                {
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1004,
                        ErrMsg = "没有权限删除该Todo",
                        Data = null
                    };
                }

                //查询Todo
                var todo = (from t in _todoContext.Todos
                            where t.TodoId == model.TodoId
                            select t).FirstOrDefault();
                if (todo == null)
                {
                    return new ResponseResult<Guid?>()
                    {
                        ErrCode = 1005,
                        ErrMsg = "Todo不存在",
                        Data = null
                    };
                }

                //设置已完成
                todo.IsDone = true;
                _todoContext.SaveChanges();

                return new ResponseResult<Guid?>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = model.TodoId
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<Guid?>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }
        #endregion
        #endregion
    }
}
