



export enum PermissionState {
    Permit = 0,
    Deny = 1,
    Undefined = 2
}

export interface LoginPar
{
    UserName: string;
    Password: string;
    Role?: number; 
}

export interface RegisterViewModel
{
    Username: string;
    Password: string;
    FullName: string;
    PhoneNo: string;
    Roles: number[];
}

export interface UserViewModel
{
    Id: number;
    UserName: string;
    FullName: string;
    PhoneNo: string;
    Status: number;
    Roles: string[];
}


export interface RegionConfigModel
{
    regionid: string;
    url: string;
    username: string;
    password: string;

}

export interface ResetPasswordViewModel
{
    userName: string;
    newPassword: string;
}