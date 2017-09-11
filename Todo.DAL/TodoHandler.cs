using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Model;

namespace Todo.DAL
{
    public class TodoHandler
    {
        #region field
        private readonly TodoContext _todoContext;
        #endregion

        #region Log
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region .ctor
        public TodoHandler(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }
        #endregion

        /// <summary>
        /// 添加TodoUser
        /// </summary>
        /// <param name="model"></param>
        public void AddTodoUser(TodoUser model)
        {
            _todoContext.TodoUsers.Add(model);
        }

        /// <summary>
        /// 通过UserId查询TodoUser
        /// </summary>
        /// <param name="userId">保存在Redis中的主键</param>
        /// <returns></returns>
        public TodoUser GetTodoUserByUserId(Guid userId)
        {
            var todoUser = (from tdu in _todoContext.TodoUsers
                            where tdu.UserId == userId
                            select tdu).FirstOrDefault();
            return todoUser;
        }
    }
}
