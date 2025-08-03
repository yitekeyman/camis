export interface UserPermissionsData
{
    Denied: number[];
    ParentUserID: number;
    Permissions: number[];
    UserID: number;
    UserName: string;

}

export interface UserPermissions extends UserPermissionsData{

    PassWordHash: string;


    UID: number;
}