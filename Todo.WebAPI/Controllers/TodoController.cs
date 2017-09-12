using NLog;
using System;
using System.Collections.Generic;
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
        #endregion

        #region Log
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public TodoController(TodoContext todoContext)
        {
            _todoContext = todoContext;
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
                //todo: 登录接口使用model.UserId获取OpenId
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == todoUserId && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) == 0
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
                //todo: 登录接口使用model.UserId获取OpenId
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == todoUserId && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 1 && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) <= 3
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
                //todo: 登录接口使用model.UserId获取OpenId
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == todoUserId && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 3 && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) <= 7
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
                //todo: 登录接口使用model.UserId获取OpenId
                List<Models.Todo> todos = (from t in _todoContext.Todos
                                           where t.TodoUser.TodoUserId == todoUserId && DbFunctions.DiffDays(t.AlertTime, DateTime.Now) > 7
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
    }
}
