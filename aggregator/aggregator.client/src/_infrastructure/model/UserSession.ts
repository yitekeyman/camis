import { UserPermissions } from "./UserPermissionData";
import { number } from "prop-types";

export interface UserSession
{
    Content: { [key: string]: any };
    EmployeeID: number | null;
    Username: string;
    Password: string;
    Role: number;
    Roles: number[];
    Id: number;
    CreatedTime: Date | string;
    LastSeen: Date | string;
    payrollSessionID: string;
    Token: string;
    wsisUserID: number;
    Permissions : UserPermissions;
    costCenterID: number;

}


export interface UserPassword
{
    userName: string;
    passwordHash: string;
    id: number;
    parentId: number;


}

export interface SecurityObject
{
    id: number;
    name: string;
    owner: number;
    childs : SecurityObject[],
    class: SecurityObjectClass;
    fullName: string;
    ownerName: string;
    parent: SecurityObject;

}

export enum SecurityObjectClass {
    soInherit = 1,
    soNoInherit = 2
}

export interface ActiveSession {
    IdleSeconds : number,
    LogedInTime : Date,
    SessionID : string,
    Source : string,
    UserName : string,
    UserID : number

}