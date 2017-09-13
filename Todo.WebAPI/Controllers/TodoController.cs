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
        public TodoController(TodoContext todoContext, RestClient client)
        {
            _todoContext = todoContext;
            _client = client;
            _client.BaseUrl = new Uri(ConfigurationManager.AppSettings["LoginApi"]);
        }
        #endregion

        #region 登录服务
        private ResponseResult<string> getOpenId(RequestBase model)
        {
            IRestRequest request = new RestRequest("/GetOpenId", Method.POST);
            request.AddJsonBody(model);

            IRestResponse response = _client.Execute(request);
            return JsonConvert.DeserializeObject<ResponseResult<string>>(response.Content);
        }
        #endregion

        #region 用户
        /// <summary>
        /// 用户是否已注册
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        private bool isOpenIdRegistered(string openId)
        {
            try
            {
                if(string.IsNullOrEmpty(openId))
                {
                    throw new ArgumentNullException("OpenId为空");
                }

                int count = (from u in _todoContext.TodoUsers
                             where !string.IsNullOrEmpty(u.OpenId) && u.OpenId == openId
                             select u).Count();

                return count != 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }

        ///// <summary>
        ///// 用户是否存在
        ///// </summary>
        ///// <param name="todoUserId"></param>
        ///// <returns></returns>
        //private bool isUserExists(Guid todoUserId)
        //{
        //    try
        //    {
        //        int count = (from u in _todoContext.TodoUsers
        //                     where u.TodoUserId == todoUserId
        //                     select u).Count();

        //        return count != 0;
        //    }
        //    catch(Exception ex)
        //    {
        //        logger.Error(ex);
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public ResponseResult<object> Register(string openId)
        {
            try
            {
                if(string.IsNullOrEmpty(openId))
                {
                    throw new ArgumentNullException("OpenId为空");
                }

                if(isOpenIdRegistered(openId))
                {
                    //该OpenId已经注册，同样返回success
                    return new ResponseResult<object>()
                    {
                        ErrCode = 0,
                        ErrMsg = "success",
                        Data = null
                    };
                }

                _todoContext.TodoUsers.Add(new TodoUser()
                {
                    TodoUserId = Guid.NewGuid(),
                    OpenId = openId,
                });
                _todoContext.SaveChanges();
                return new ResponseResult<object>()
                {
                    ErrCode = 0,
                    ErrMsg = "success",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return new ResponseResult<object>()
                {
                    ErrCode = 1001,
                    ErrMsg = ex.Message,
                    Data = null
                };
            }
        }

        ///// <summary>
        ///// 通过OpenId换取TodoUserId
        ///// </summary>
        ///// <param name="openId"></param>
        ///// <returns></returns>
        //public ResponseResult<Guid?> GetTodoUserId(string openId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(openId))
        //        {
        //            throw new ArgumentNullException("OpenId为空");
        //        }

        //        if (!isOpenIdRegistered(openId))
        //        {
        //            return new ResponseResult<Guid?>()
        //            {
        //                ErrCode = 1003,
        //                ErrMsg = "用户未注册",
        //                Data = null
        //            };
        //        }

        //        var todoUser = _todoContext.TodoUsers.FirstOrDefault(x => x.OpenId == openId);
        //        return new ResponseResult<Guid?>()
        //        {
        //            ErrCode = 0,
        //            ErrMsg = "success",
        //            Data = todoUser.TodoUserId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return new ResponseResult<Guid?>()
        //        {
        //            ErrCode = 1001,
        //            ErrMsg = ex.Message,
        //            Data = null
        //        };
        //    }
        //}
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
                if(result.ErrCode != 0)
                {
                    logger.Error(result.ErrMsg);
                    return new ResponseResult<List<TodoViewModel>>()
                    {
                        ErrCode = 1002,
                        ErrMsg = result.ErrMsg,
                        Data = null
                    };
                }

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.OpenId == result.Data && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) == 0
                                           select t).ToList();
                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach(var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertTime = item.AlertTime,
                        Content = item.Content,
                        CreateTime = item.CreateTime,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        TodoUserId = item.TodoUser.TodoUserId,
                        UpdateTime = item.UpdateTime,
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
            catch(Exception ex)
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

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.OpenId == result.Data && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 1 && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) <= 3
                                           select t).ToList();
                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertTime = item.AlertTime,
                        Content = item.Content,
                        CreateTime = item.CreateTime,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        TodoUserId = item.TodoUser.TodoUserId,
                        UpdateTime = item.UpdateTime,
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

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.OpenId == result.Data && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 3 && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) <= 7
                                           select t).ToList();
                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertTime = item.AlertTime,
                        Content = item.Content,
                        CreateTime = item.CreateTime,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        TodoUserId = item.TodoUser.TodoUserId,
                        UpdateTime = item.UpdateTime,
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

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.OpenId == result.Data && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 7
                                           select t).ToList();
                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertTime = item.AlertTime,
                        Content = item.Content,
                        CreateTime = item.CreateTime,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        TodoUserId = item.TodoUser.TodoUserId,
                        UpdateTime = item.UpdateTime,
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
        /// 获取过期Todo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<List<TodoViewModel>> GetExpiredTodos([FromBody]RequestBase model)
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

                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.OpenId == result.Data && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) < 0
                                           select t).ToList();
                List<TodoViewModel> todoVMs = new List<TodoViewModel>();
                foreach (var item in todos)
                {
                    todoVMs.Add(new TodoViewModel()
                    {
                        AlertBeforeMinutes = item.AlertBeforeMinutes,
                        AlertTime = item.AlertTime,
                        Content = item.Content,
                        CreateTime = item.CreateTime,
                        Title = item.Title,
                        TodoId = item.TodoId,
                        TodoUserId = item.TodoUser.TodoUserId,
                        UpdateTime = item.UpdateTime,
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
        public ResponseResult<Guid?> AddTodo(TodoAddRequest model)
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

                if(string.IsNullOrEmpty(model.Title))
                {
                    throw new ArgumentNullException("标题为空");
                }

                var user = (from u in _todoContext.TodoUsers
                            where u.OpenId == result.Data
                            select u).FirstOrDefault();
                var todo = new Models.Todo()
                {
                    TodoId = Guid.NewGuid(),
                    Title = model.Title,
                    Content = model.Content,
                    AlertTime = model.AlertTime,
                    AlertBeforeMinutes = model.AlertBeforeMinutes,
                    UseAlert = model.UseAlert,
                    CreateTime = DateTime.Now,
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
            catch(Exception ex)
            {
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
