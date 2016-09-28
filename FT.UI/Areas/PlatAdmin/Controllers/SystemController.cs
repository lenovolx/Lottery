using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FT.IRepository;
using FT.Model;
using FT.Model.QueryModel;
using FT.Model.ViewModel;
using FT.Repository;
using FT.UI.Areas.PlatAdmin.Filters;
using FT.Utility.ApiHelper;
using FT.Utility.Helper;
using Newtonsoft.Json.Linq;

namespace FT.UI.Areas.PlatAdmin.Controllers
{
    [AdminAuthorize]
    public class SystemController : BaseController
    {
        #region View

        [PublicViewBagFilterAttribute()]
        public ActionResult AdminInfo()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult MenuInfo()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult RoleInfo()
        {
            return View();
        }
        [PublicViewBagFilterAttribute()]
        public ActionResult LogInfo() { return View(); }
        [PublicViewBagFilterAttribute()]
        public ActionResult Dictionary() { return View(); }
        [PublicViewBagFilterAttribute()]
        public ActionResult Message() { return View(); }
        [PublicViewBagFilterAttribute()]
        public ActionResult Advertise() { return View(); }
        [PublicViewBagFilterAttribute()]
        public ActionResult Card() { return View(); }
        [PublicViewBagFilterAttribute()]
        public ActionResult Task() { return View(); }
        #endregion

        #region AjaxMethod

        #region Admin

        [HttpPost]
        public JsonResult AdminGrid(string sort, string order, int page = 1, int rows = 50, string username = "", int menuid = 0)
        {
            var query = new AdminQueryModel()
            {
                Page = page,
                PageSize = rows,
                UserName = username,
                IsDesc = order == "desc" ? true : false,
                SortField = sort,
                MenuId = menuid,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin
            };
            var datagrid = new AdminRepository().GetAdminGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 账户新增
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        [HttpPost]
        [LogInfo(type: "管理员新增")]
        public ActionResult CreateAdmin(Admin admin)//账户新增
        {
            var salt = Guid.NewGuid().ToString().Substring(0, 20);
            var rrt = new ApiReturn();
            var info = new Admin()
            {
                LoginName = admin.LoginName,
                Password = SecureHelper.Md5(SecureHelper.Md5(admin.Password) + salt),
                RoleId = admin.RoleId,
                PasswordSalt = salt,
                CreateDate = DateTime.Now,
                Status = LockEnum.Nnormal
            };
            if (new AdminRepository().Insert(info))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }        
        [LogInfo(type: "管理员编辑")]
        public ActionResult EditAdmin(Admin admin)//编辑
        {
            var rrt = new ApiReturn();
            if (new AdminRepository().ModifyAdmin(admin))
                rrt.code = 0;
            else
                rrt.errors = "操作失败[已存在的账户名]";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);

        }
        [LogInfo(type: "管理员删除")]
        public JsonResult DelAdmin(int id)//删除
        {
            var rrt = new ApiReturn();
            if (new AdminRepository().FrozenOrDeleteAdmin(id, LockEnum.Delete))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "管理员锁定")]
        public JsonResult SetAdminLock(int id, int Lock)
        {
            var rrt = new ApiReturn();
            if (new AdminRepository().FrozenOrDeleteAdmin(id, (LockEnum)Lock))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "管理员修改密码")]
        public ActionResult ModifyPassword(ChangePassViewModel model)
        {
            var rrt = new ApiReturn();
            var flag = new AdminRepository().ModifyPass(CurrentAdmin.Id.Value, model.OldPassword, model.NewPassword);
            if (flag == EditEnum.Success)
                rrt.code = 0;
            else
                rrt.errors = flag.ToDescription();
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "管理员重置密码")]
        public ActionResult SetDefaultPass(int adminid)
        {
            var rrt = new ApiReturn();
            var flag = new AdminRepository().ModifyPass(adminid, "", "123456", false);
            if (flag == EditEnum.Success)
                rrt.code = 0;
            else
                rrt.errors = flag.ToDescription();
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Setting
        [HttpPost]
        [LogInfo(type: "系统设置")]
        public ActionResult Setting(SystemSetting set)//系统设置
        {
            var rrt = new ApiReturn();
            if (new SystemSettingRepository().EditSetting(set))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSetting()
        {
            return Json(new SystemSettingRepository().GetSetting(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Dictionary
        public JsonResult DictionaryGrid(int id = 0)
        {
            var tree = new DictionaryRepository().DictionaryGrid(id, LanguageAdmin);
            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "字典新增")]
        public ActionResult CreateDictionary(SystemDictionary menu)
        {
            var rrt = new ApiReturn();
            var info = new SystemDictionary()
            {
                Sort = menu.Sort,
                DictionaryName = menu.DictionaryName,
                DictionaryNameEn = menu.DictionaryNameEn,
                DictionaryNamePt = menu.DictionaryNamePt,
                ParentId = menu.ParentId,
                IsLock = menu.IsLock
            };
            if (new DictionaryRepository().Insert(info))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }

        [LogInfo(type: "字典编辑")]
        public ActionResult EditDictionary(SystemDictionary menu)
        {
            var rrt = new ApiReturn();

            if (new DictionaryRepository().Update(s => s.Id == menu.Id, u => new SystemDictionary
            {
                DictionaryNameEn = menu.DictionaryNameEn,
                DictionaryName = menu.DictionaryName,
                DictionaryNamePt = menu.DictionaryNamePt,
                DictionaryValue = menu.DictionaryValue,
                Sort = menu.Sort,
                IsLock = menu.IsLock,
                ParentId = menu.ParentId
            }))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Menu

        [HttpPost]
        public JsonResult MenuGrid(int id = 0)
        {
            var tree = new MenuRepository().MenuList(id, LanguageAdmin);
            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "菜单新增")]
        public ActionResult CreateMenu(Menus menu)
        {
            var rrt = new ApiReturn();
            var info = new Menus()
            {
                SortNum = menu.SortNum,
                ButtonLink = menu.ButtonLink,
                MenuType = menu.MenuType,
                Url = menu.Url,
                Name = menu.Name,
                ParentId = menu.ParentId,
                Icon = menu.Icon,
                NameEn = menu.NameEn
            };
            if (new MenuRepository().CreateMenu(info))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "菜单编辑")]
        public ActionResult EditMenu(Menus menu)
        {
            var rrt = new ApiReturn();

            if (new MenuRepository().UpdateMenu(menu))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 所有菜单
        /// </summary>
        /// <returns></returns>
        public string GetAllMenu()
        {
            var list = new MenuRepository().CacheMenu(s => s.IsLock == 0, LanguageAdmin).ToArray();
            var json = new JArray(
                       from r in list
                       select new JObject(
                           new JProperty("id", r.Id),
                           new JProperty("name", r.Name + "[" + r.MenuType + "]"),
                           new JProperty("parentId", r._parentId)));
            return json.ToString();
        }
        #endregion

        #region Role

        [HttpPost]
        public JsonResult RoleGrid(string sort, string order, int page = 0, int rows = 50, int islock = 0,
            string rolename = "", int menuid = 0)
        {
            var query = new RoleQueryModel()
            {
                IsDesc = order == "asc" ? true : false,
                SortField = sort,
                PageSize = rows,
                Page = page,
                RoleName = rolename,
                MenuId = menuid,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin
            };
            var role = new RoleRepository().RoleGrid(query);
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetRoleMenu(string menuIds, int roleid)
        {
            var rrt = new ApiReturn();
            if (string.IsNullOrEmpty(menuIds) || roleid == 0) return Json(rrt, JsonRequestBehavior.AllowGet);
            var menuId = menuIds.Split(',').Select(int.Parse).Select(items => new RoleMenu
            {
                MenuId = items,
                RoleId = roleid
            });
            new RoleRepository().SetRoleMenu(menuId, roleid);
            rrt.code = 0;
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        public JObject GetRoleMenu(long roleid)
        {
            var list = new RoleRepository().RoleMenuList(roleid);
            var json = new JObject(
                new JProperty("menus",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("Id", r)))));
            return json;
        }
        [LogInfo(type: "角色新增")]
        public ActionResult CreateRole(Role role)
        {
            var rrt = new ApiReturn();

            var info = new Role()
            {
                RoleName = role.RoleName,
                RoleNameEn = role.RoleNameEn
            };
            if (new RoleRepository().Insert(info))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "角色编辑")]
        public ActionResult EditRole(Role role)
        {
            var rrt = new ApiReturn();
            if (new RoleRepository().Update(w => w.Id == role.Id, u => new Role
            {
                RoleName = role.RoleName,
                RoleNameEn = role.RoleNameEn
            }))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "角色删除")]
        public JsonResult DelRole(long id)
        {
            var rrt = new ApiReturn();
            if (new RoleRepository().FrozenOrDeleteRole(id, LockEnum.Delete))
                rrt.code = 0;
            else
                rrt.errors = "操作失败[已有账户分配该角色]";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "角色锁定、删除")]
        public JsonResult SetRoleLock(long id, int Lock)
        {
            var rrt = new ApiReturn();
            if (new RoleRepository().FrozenOrDeleteRole(id, (LockEnum)Lock))
                rrt.code = 0;
            else
                rrt.errors = "操作失败[已有账户分配该角色]";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Card
        /// <summary>
        /// 卡片新增
        /// </summary>
        /// <param name="cg">实体</param>
        /// cs
        /// <returns></returns>
        [LogInfo(type: "充值卡新增")]
        public JsonResult CardAdd(CardGroup cg)
        {
            var rrt = new ApiReturn();
            if (!string.IsNullOrEmpty(cg.GroupName) && cg.Amount > 0 && cg.Number > 0)
            {
                var card = new CardGroupRepository().AddCardGroup(cg);
                if (card)
                {
                    card = new CardRepository().AddCard(cg.Amount, cg.Number, cg.GroupId);
                }
                if (card)
                    rrt.code = 0;
                else
                    rrt.errors = "失败";
            }
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CardGrid(string sort, string order, int page = 1, int rows = 50)
        {
            var query = new CardQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc" ? true : false,
                SortField = sort,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin
            };
            var datagrid = new CardGroupRepository().CardList(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CardDetail(long? groupId, string sort, string order, int page = 1, int rows = 50)
        {
            var grid = new CardRepository().CardList(new CardQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc" ? true : false,
                SortField = sort,
                GroupId = groupId
            });
            return Json(grid, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region LogInfo
        [HttpPost]
        public JsonResult LogGrid(DateTime? logStartDate, DateTime? logEndDate, string logUser, string sort, string order, int page = 0, int rows = 50)
        {
            var query = new LogInfoQueryModel()
            {
                IsDesc = order == "desc",
                SortField = sort,
                Page = page,
                PageSize = rows,
                StartDate = logEndDate,
                EndDate = logStartDate,
                LogUser = logUser
            };
            var grid = new LogInfoRepository().LogGrid(query);
            return Json(grid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[LogInfo]
        public JsonResult DelLogInfo(string ids)
        {
            var rrt = new ApiReturn();
            var stringIds = ids.Split(',').Select(s => s);
            if (new LogInfoRepository().Delete(s => stringIds.Contains(s.Id)))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Adv
        public JsonResult AdvGrid(string sort, string order, int page = 1, int rows = 50)
        {
            var query = new BaseQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin
            };
            var datagrid = new SiteAdvRepository().AdvGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }

        [LogInfo(type: "新增轮播")]
        public ActionResult CreateAdv(SiteAdv adv)
        {
            var rrt = new ApiReturn();
            var info = new SiteAdv()
            {
                AdvTitle = adv.AdvTitle,
                AdvImg = adv.AdvImg,
                Type = adv.Type,
                Sort = adv.Sort,
                IsShow = YesNoEnum.Yes
            };
            if (new SiteAdvRepository().Insert(info))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }

        [LogInfo(type: "编辑轮播")]
        public ActionResult EditAdv(SiteAdv adv)
        {
            var rrt = new ApiReturn();
            if (new SiteAdvRepository().Update(w => w.Id == adv.Id, u => new SiteAdv
            {
                AdvTitle = adv.AdvTitle,
                Sort = adv.Sort,
                AdvImg = adv.AdvImg,
                Type = adv.Type
            }))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "轮播图(禁用、启用)")]
        public JsonResult SetAdvShow(int id, int show = 0)
        {
            var api = new ApiReturn();
            if (new SiteAdvRepository().Update(s => s.Id == id, w => new SiteAdv
                {
                    IsShow = (YesNoEnum)show
                }))
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "轮播删除")]
        public JsonResult DelAdv(int id)
        {
            var api = new ApiReturn();
            if (new SiteAdvRepository().Delete(s => s.Id == id))
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Message
        public JsonResult MessGrid(string title, string sort, string order, int page = 1, int rows = 50)
        {
            var query = new MessageQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin,
                Title = title
            };
            var datagrid = new MessageRepository().MessageGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }

        [LogInfo(type: "新增通告")]
        [ValidateInput(false)]
        public ActionResult CreateMess(Message msg)
        {
            var rrt = new ApiReturn();
            msg.CreateUser = CurrentAdmin.Id.Value;
            msg.CreateUserName = CurrentAdmin.LoginName;
            msg.IsDrafts = YesNoEnum.No;
            msg.CreateDate = DateTime.Now;
            if (new MessageRepository().Insert(msg))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }

        [LogInfo(type: "编辑通告")]
        [ValidateInput(false)]
        public ActionResult EditMess(Message msg)
        {
            var rrt = new ApiReturn();
            if (new MessageRepository().Update(w => w.Id == msg.Id, u => new Message
            {
                Title = msg.Title,
                Descriptions = msg.Descriptions,
                LastUpdateDate = DateTime.Now,
                Annexes = msg.Annexes,
                CreateUser = CurrentAdmin.Id.Value,
                CreateUserName = CurrentAdmin.LoginName
            }))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "通告(正常、草稿箱)")]
        public JsonResult SetMessDrafts(int id, int drafts = 0)
        {
            var api = new ApiReturn();
            if (new MessageRepository().Update(s => s.Id == id, w => new Message
            {
                IsDrafts = (YesNoEnum)drafts
            }))
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [LogInfo(type: "通告删除")]
        public JsonResult DelMess(int id)
        {
            var api = new ApiReturn();
            if (new MessageRepository().Delete(s => s.Id == id))
                api.code = 0;
            else
                api.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(api, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Task

        [LogInfo(type: "新增任务")]
        public ActionResult AddTask(SystemTask task)
        {
            var rrt = new ApiReturn();
            task.CreatedOn = DateTime.Now;
            task.TaskID = Guid.NewGuid();
            if (new SystemTaskRepository().Insert(task))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "编辑任务")]
        public ActionResult EditTask(SystemTask task)
        {
            var rrt = new ApiReturn();
            if (new SystemTaskRepository().Update(w => w.TaskID == task.TaskID, u => new SystemTask
            {
                TaskName = task.TaskName,
                Assembly = task.Assembly,
                CronExpressionString = task.CronExpressionString,
                CronRemark = task.CronRemark,
                Class = task.Class,
                Remark = task.Remark,
                Status = task.Status
            }))
                rrt.code = 0;
            else
                rrt.errors = "操作失败";
            return Json(rrt, "text/html", JsonRequestBehavior.AllowGet);
        }
        [LogInfo(type: "删除任务")]
        public JsonResult DelTask(string ids)
        {
            var rrt = new ApiReturn();
            var stringIds = ids.Split(',').Select(s => s);
            if (new SystemTaskRepository().Delete(s => stringIds.Contains(s.TaskID.ToString())))
                rrt.code = 0;
            else
                rrt.errors = RegisterEnum.Failure.ToDescription(LanguageAdmin);
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TaskGrid(string sort, string order, int page = 1, int rows = 50)
        {
            var query = new BaseQueryModel
            {
                Page = page,
                PageSize = rows,
                IsDesc = order == "desc",
                SortField = sort,
                RoleId = CurrentAdmin.RoleId,
                Language = LanguageAdmin
            };
            var datagrid = new SystemTaskRepository().TaskGrid(query);
            return Json(datagrid, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Common Upload
        public JsonResult UploadFiles(HttpPostedFileBase fileData, string action, string delfile = "")
        {
            var rrt = new ApiReturn();
            if (!string.IsNullOrEmpty(delfile) && delfile.IndexOf("../") == -1)
            {
                if (System.IO.File.Exists(delfile))
                {
                    System.IO.File.Delete(delfile);
                    rrt.code = 0;
                }
            }
            if (fileData != null)
            {
                if (fileData.FileName.EndsWith(".jpg") || fileData.FileName.EndsWith(".png") ||
                    fileData.FileName.EndsWith(".jpeg"))
                {
                    try
                    {
                        var InfoPath = "/UploadFiles/Adv/";
                        var filePath = Server.MapPath(InfoPath);
                        var fileName = Path.GetFileName(fileData.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        var saveName = Guid.NewGuid().ToString() + fileExtension;
                        var savaPath = Path.Combine(filePath, saveName);
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        fileData.SaveAs(savaPath);
                        rrt.code = 0;
                        rrt.data = new { path = InfoPath + saveName, name = fileName, size = fileData.ContentLength };
                    }
                    catch (Exception ex)
                    {
                        rrt.errors = "导入文件失败!";
                    }
                }
                else
                    rrt.errors = "请上传 .jpg .png .jpeg格式文件!";
            }
            else
                rrt.errors = "请选择文件上传!";
            return Json(rrt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Common ComboxEnum
        /// <summary>
        /// 用户类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxUserType()
        {
            var list = new List<ComBoxModel>();
            var userType = EnumHelper.ToSelectList<UserTypeEnum>(0, LanguageAdmin, false).Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            list.AddRange(userType);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 代理商等级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxUserLevel(int level = 0, bool all = false)
        {
            var list = new List<ComBoxModel>();
            var userType = EnumHelper.ToSelectList<LevelEnum>(0, LanguageAdmin, false).Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            if (!all)
                list.AddRange(userType.Where(s => int.Parse(s.Id) == level + 1));
            else
                list.AddRange(userType.Where(s => int.Parse(s.Id) > level));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 菜单类型
        /// </summary>
        /// <returns></returns>
        public JsonResult ComBoxMenuType()
        {
            var selectList = EnumHelper.ToSelectList<MenuEnum>(0, LanguageAdmin, false);
            var menuType = selectList.Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 角色类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxRoleType()
        {
            var roletype = new RoleRepository().GetList(s => s.IsLock == LockEnum.Nnormal).ToArray().Select(s => new ComBoxModel
            {
                Id = s.Id + "",
                Name = s.RoleName
            });
            return Json(roletype, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 交易类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxTradType(string bind = "")
        {
            var selectList = EnumHelper.ToSelectList<TradEnum>(0, LanguageAdmin, true);
            IEnumerable<ComBoxModel> menuType;
            if (bind == "")
                menuType = selectList.Select(item => new ComBoxModel
                {
                    Id = item.Value,
                    Name = item.Text
                });
            else
            {
                menuType = selectList.Where(s => bind.Split(',').Contains(s.Value)).Select(item => new ComBoxModel
                {
                    Id = item.Value,
                    Name = item.Text
                });
            }
            return Json(menuType.OrderBy(s => s.Id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 交易类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxCashAuditType(bool all = true, string notequl = "")
        {
            var selectList = EnumHelper.ToSelectList<CashAuditEnum>(0, LanguageAdmin, all);
            IEnumerable<ComBoxModel> menuType;
            if (notequl == "")
                menuType = selectList.Select(item => new ComBoxModel
                {
                    Id = item.Value,
                    Name = item.Text
                });
            else
                menuType = selectList.Where(s => !notequl.Split(',').Contains(s.Value)).Select(item => new ComBoxModel
                {
                    Id = item.Value,
                    Name = item.Text
                });
            return Json(menuType.OrderBy(s => s.Id), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否标识
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxYesNoType()
        {
            var selectList = EnumHelper.ToSelectList<YesNoEnum>(0, LanguageAdmin, false);
            var menuType = selectList.Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 是否标识
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxPlatFromType()
        {
            var selectList = EnumHelper.ToSelectList<PlatEnum>(0, LanguageAdmin, false);
            var menuType = selectList.Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 赛事开奖标识
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComBoxOpenType()
        {
            var selectList = EnumHelper.ToSelectList<OpenEnum>(0, LanguageAdmin, false);
            var menuType = selectList.Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 赛事数据来源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComDataSourceType()
        {
            var selectList = EnumHelper.ToSelectList<DataSourceEnum>(0, LanguageAdmin, false);
            var menuType = selectList.Select(item => new ComBoxModel
            {
                Id = item.Value,
                Name = item.Text
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 系统字典根节点
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ComDictionaryType()
        {
            var selectList = new DictionaryRepository().DictionaryList(s => s.ParentId == 0, LanguageAdmin);
            selectList.Add(new ComBoxModel
            {
                Id = "0",
                Name = "Root"
            });
            var menuType = selectList.OrderBy(s => s.Id).Select(item => new ComBoxModel
            {
                Id = item.Id,
                Name = item.Name
            });
            return Json(menuType, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion
    }
}