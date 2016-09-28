using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;

namespace FT.IRepository
{
    public interface IUserAccountRepository : IBaseRepository<UserAccount>
    {
        UserResponse UserLogin(string username, string password, bool remember = false, int plat = 0);
        RegisterEnum UserRegister(string username, string password, PlatEnum plat = PlatEnum.Mobile, bool passmd5 = true);
        EasyDataGrid<UserViewModel> GetUserGrid(Model.QueryModel.UserQueryModel query);
        EditEnum ModifyPass(long userid, string password, string newpassword,int passType=0);
        TransAccountsEnum UserTrad(TradEnum type,long formuser, string touser="", decimal amount=0, string card = "", string pass = "",string code="");
        List<UserAccountHistoryViewModel> GetUserTradHistory(UserBetQueryModel query);
        FootGrid<UserAccountHistoryViewModel, TotalTradViewModel> GetTradHistory(UserBetQueryModel query);
        /// <summary>
        /// 帐号添加(关联代理商)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        RegisterEnum AddUser(UserAccount model);

        RegisterEnum EditUser(UserAccount model);
        bool FrozenOrDeleteUser(long userid, LockEnum state);
        void UserLogout(string token);
    }
}