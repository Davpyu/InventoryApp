
/*
  ! Important: The permission constant should be sync with "/Seeders/Permission.json"
  ---------------------
*/
namespace DotNetService.Constants.Permission
{
  public static class PermissionConstant
  {
    public const string ALL = "all";
    public const string DEPRECATED = "deprecated";

    /* ----------------------------- User Management ---------------------------- */
    public const string USER_VIEW = "view-user";
    public const string USER_CREATE = "create-user";
    public const string USER_UPDATE = "update-user";
    public const string USER_DELETE = "delete-user";

    /* ----------------------------- Role Management ---------------------------- */
    public const string ROLE_VIEW = "view-role";
    public const string ROLE_CREATE = "create-role";
    public const string ROLE_UPDATE = "update-role";
    public const string ROLE_DELETE = "delete-role";

    /* -------------------------- Permission Management ------------------------- */
    public const string PERMISSION_VIEW = "view-permission";
    public const string PERMISSION_CREATE = "create-permission";
    public const string PERMISSION_UPDATE = "update-permission";
    public const string PERMISSION_DELETE = "delete-permission";

  }
}