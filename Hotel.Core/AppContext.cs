using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel.Core
{
    public class AppContext
    {
		///// <summary>
		///// 获取用户身份
		///// </summary>
		//public IPrincipal UserPrincipal { get; private set; }

		///// <summary>
		///// 获取用户对象
		///// </summary>
		//public IUser User
		//{
		//	get
		//	{
		//		IUserContextService userContextService = DependencyContext.Current.Resolve<IUserContextService>();
		//		if (userContextService == null)
		//		{
		//			throw new Exception("IUserProvider impl is required");
		//		}
		//		_user = userContextService.GetCurrentUser();
		//		return _user;
		//	}
		//}

		//[ThreadStatic]
		//private static AppContext _threadCtx;
		///// <summary>
		///// 获取当前模型操作上下文
		///// </summary>
		//public static AppContext Current
		//{
		//	get
		//	{
		//		var current = HttpContext;
		//		if (current == null)
		//		{
		//			if (_threadCtx == null)
		//			{
		//				_threadCtx = new AppContext();
		//				_threadCtx.UserPrincipal = Thread.CurrentPrincipal;
		//			}
		//			return _threadCtx;
		//		}
		//		ModelContext modelContext = current.Items["ModelContext"] as ModelContext;
		//		if (modelContext == null)
		//		{
		//			modelContext = new ModelContext
		//			{
		//				IsWebContext = true
		//			};
		//			current.Items["ModelContext"] = modelContext;
		//			modelContext.UserPrincipal = current.User;
		//			modelContext.IsWebContext = current.CurrentHandler != null;
		//			if (modelContext.IsWebContext)
		//			{
		//				ProcessUserLang(current, modelContext);
		//				modelContext.ViewFilter = current.Request.QueryString["filter"];
		//			}
		//		}
		//		return modelContext;
		//	}
		//}

		//private User _user;
  //      public User User
  //      {
  //          get
  //          {
  //              if (_user == null)
  //              {
  //                  var ctxUser = ModelContext.Current.UserPrincipal;
  //                  if (ctxUser.Identity.IsAuthenticated)
  //                  {
  //                      var acSvr = DependencyContext.Current.Resolve<IAcService>();
  //                      _user = (AcUser)acSvr.GetUser(ctxUser.Identity.Name, true);
  //                      if (_user == null)
  //                          throw new Exception("can't find user:" + ctxUser.Identity.Name);
  //                  }

  //              }
  //              return _user;
  //          }
  //      }
    }
}
