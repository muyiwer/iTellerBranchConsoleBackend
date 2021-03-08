using iTellerBranch.Model.ViewModel;
using iTellerBranch.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTellerBranch.Repository.Service
{
    public class ResourceService:BaseService, IResourceService
    {

        public object GetRole(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex, // filtering of each of them will be done on FE
                data = new
                {
                    Roles = db.ROLES.Select(x=> new
                    {
                        x.RoleId,
                        x.RoleDesc
                    }).ToArray(),
                    Modules = db.Modules.Select(x=> new
                    {
                        x.ModuleId,
                        x.ModuleName,
                        x.FontIcon
                    }).ToArray(),
                    Resources = db.Resources.Select(x=> new
                    {
                        x.ModuleId,
                        x.ResourceId,
                        x.ResourceName,
                        x.ResourceDesc,
                        x.Path,
                        x.FontIcon
                    }).ToArray(),
                    RoleResources = db.RoleResources.Select(x=> new
                    {
                        x.ResourceId,
                        x.ROLES.RoleId,
                        x.ROLES.RoleDesc,
                        x.Resources.ResourceName,
                        x.Resources.Path,
                        x.Resources.FontIcon
                    })
                }

            };
        }

        public object UpdateRole(ROLES Role)
        {
            try
            {
                var dbRoles = db.ROLES.Find(Role.RoleId); 
                if(dbRoles == null)
                {
                    return GetRoleResource(false, "Role does not exist");
                }
                if(dbRoles.RoleDesc != Role.RoleDesc)
                {
                    if (!IsRoleNameExist(Role.RoleDesc))
                    {
                        dbRoles.RoleDesc = Role.RoleDesc;
                        db.Entry(dbRoles).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return GetRoleResource(true, "Role updated successfully");
                    }
                    else
                    {
                        return GetRoleResource(false, "Role name already exist");
                    }
                }
                else
                {
                    return GetRoleResource(true, "");
                }
            }
            catch (Exception ex)
            {
                return GetRoleResource(false, "Server Error", ex);
            }
        }

        public object AssignRole(RoleResModel roleResModel)
        {
            try
            {
                List<RoleResources> roleResources = new List<RoleResources>();
                var resources = db.RoleResources.Where(x => x.RoleId == roleResModel.RoleId).ToList();
                if(resources.Count() > 0 )
                {
                    db.RoleResources.RemoveRange(resources);
                    db.SaveChanges();
                    
                    foreach(int resourceId in roleResModel.ResourceId)
                    {
                        roleResources.Add(new RoleResources
                        {
                            ResourceId = resourceId,
                            RoleId = roleResModel.RoleId
                        });
                    }
                    db.RoleResources.AddRange(roleResources);
                    db.SaveChanges();
                    return GetRoleResource(true, "");
                }
                else
                {
                    foreach (int resourceId in roleResModel.ResourceId)
                    {
                        roleResources.Add(new RoleResources
                        {
                            ResourceId = resourceId,
                            RoleId = roleResModel.RoleId
                        });
                    }
                    db.RoleResources.AddRange(roleResources);
                    db.SaveChanges();
                    return GetRoleResource(true, "");
                }
            }
            catch (Exception ex)
            {
                return GetRoleResource(false, "Server Error", ex);
            }
        }

        public object CreateRole(ROLES Role)
        {
            try
            {
                if (!IsRoleNameExist(Role.RoleDesc))
                {
                    db.ROLES.Add(Role);
                    db.SaveChanges();
                    return GetRoleResource(true, "Role created successfully");
                }
                else
                {
                    return GetRoleResource(false, "Role Already exists");
                }
            }
            catch (Exception ex)
            {
                return GetRoleResource(false, "Server Error", ex);
            }
        }

        public object DeleteRole(int Id)
        {
            try
            {
                if (IsRoleExist(Id))
                {
                    var roleResources = db.RoleResources.Where(x => x.RoleId == Id).ToList();
                    if(roleResources.Count() > 0)
                    {
                        db.RoleResources.RemoveRange(roleResources);
                    }
                    db.ROLES.Remove(db.ROLES.Find(Id));
                    db.SaveChanges();
                    return GetRoleResource(true, "Role deleted successfully");
                }
                else
                {
                    return GetRoleResource(false, "Role does not exist");
                }
            }
            catch (Exception ex)
            {
                return GetRoleResource(false, "Server Error", ex);
            }
        }

        public object GetRoleResource(bool success, string message, Exception ex = null)
        {
            return new
            {
                success,
                message,
                innerError = ex, // filtering of each of them will be done on FE
                data = new
                {
                    Roles = db.ROLES.Select(x => new
                    {
                        x.RoleId,
                        x.RoleDesc
                    }).ToArray(),
                    Modules = db.Modules.Select(x => new
                    {
                        x.ModuleId,
                        x.ModuleName,
                        x.FontIcon
                    }).ToArray(),
                    Resources = db.Resources.Select(x => new
                    {
                        x.ModuleId,
                        x.Modules.ModuleName,
                        x.Sequence,
                        x.Show,
                        x.ShowMenu,
                        x.ResourceId,
                        x.ResourceName,
                        x.ResourceDesc,
                        x.Path,
                        x.FontIcon
                    }).ToArray(),
                    RoleResources = db.RoleResources.Select(x => new
                    {
                        x.ResourceId,
                        x.ROLES.RoleId,
                        x.ROLES.RoleDesc,
                        x.Resources.ResourceName
                       
                    })
                }

            };
        }

        public object GetRoleResource(int id)
        {
            return new
            {
                Modules = db.Modules.Select(x => new
                {
                    x.ModuleId,
                    x.ModuleName,
                    x.FontIcon,
                    Resources = db.ROLES.Where(z => z.RoleId == id).Select(z => new
                    {
                        Resources = z.RoleResources.Where(y => y.Resources.ModuleId == x.ModuleId).Select(y => new
                        {

                            y.Resources.ModuleId,
                            y.Resources.Modules.ModuleName,
                            y.Resources.Sequence,
                            y.Resources.Show,
                            y.Resources.ShowMenu,
                            y.Resources.ResourceId,
                            y.Resources.ResourceName,
                            y.Resources.ResourceDesc,
                            y.Resources.Path,
                            y.Resources.FontIcon
                        }).OrderBy(y=>y.Sequence)
                    }).FirstOrDefault()
                }).ToArray(),
                RoleResources = db.ROLES.Where(x => x.RoleId == id).Select(x => new
                {
                    Resources = x.RoleResources.Select(y => new
                    {
                        y.Resources.ModuleId,
                        y.Resources.Modules.ModuleName,
                        y.Resources.Sequence,
                        y.Resources.Show,
                        y.Resources.ShowMenu,
                        y.Resources.ResourceId,
                        y.Resources.ResourceName,
                        y.Resources.ResourceDesc,
                        y.Resources.Path,
                        y.Resources.FontIcon
                    })
                }).FirstOrDefault()
            };
        }

        public object GetRoleResourceByRoleName(string rolename)
        {
            return new
            {
                Modules = db.Modules.Select(x => new
                {
                    x.ModuleId,
                    x.ModuleName,
                    x.FontIcon,
                    Resources = db.ROLES.Where(z => z.RoleDesc == rolename).Select(z => new
                    {
                        Resources = z.RoleResources.Where(y => y.Resources.ModuleId == x.ModuleId).Select(y => new
                        {

                            y.Resources.ModuleId,
                            y.Resources.Modules.ModuleName,
                            y.Resources.Sequence,
                            y.Resources.Show,
                            y.Resources.ShowMenu,
                            y.Resources.ResourceId,
                            y.Resources.ResourceName,
                            y.Resources.ResourceDesc,
                            y.Resources.Path,
                            y.Resources.FontIcon
                        }).OrderBy(y => y.Sequence)
                    }).FirstOrDefault()
                }).ToArray(),
                RoleResources = db.ROLES.Where(x => x.RoleDesc == rolename).Select(x => new
                {
                    Resources = x.RoleResources.Select(y => new
                    {

                        y.Resources.ModuleId,
                        y.Resources.Modules.ModuleName,
                        y.Resources.Sequence,
                        y.Resources.Show,
                        y.Resources.ShowMenu,
                        y.Resources.ResourceId,
                        y.Resources.ResourceName,
                        y.Resources.ResourceDesc,
                        y.Resources.Path,
                        y.Resources.FontIcon
                    })
                }).FirstOrDefault()
            };
        }
        

        private bool IsRoleExist(int id)
        {
            return db.ROLES.Find(id) != null ? true : false;
        }


        private bool IsRoleNameExist(string roleName)
        {
            return db.ROLES.Where(x=> x.RoleDesc == roleName).Count() > 0 ? true : false;
        }

        public object AssignRole(int Id, List<string> ResourceId)
        {
            throw new NotImplementedException();
        }
    }
}
