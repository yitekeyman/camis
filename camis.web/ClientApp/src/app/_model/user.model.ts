export interface LoginUser {
    username: string;
    password: string;
}

export interface RegisterUser {
    username: string;
    password: string;
    roles: number[];
    fullname: string;
    phoneNo: string;
    email: string;
}

export interface UpdatePassword {
    OldPassword: string;
    NewPassword: string;
}

export interface ResetPassword {
    UserName: string;
    NewPassword: string;
}
export interface LookUpModel{
  id: number;
  name: string;
}
export interface UserModel {
    userName: string;
    fullName: string;
    phoneNo: string;
    status: number;
    roles: LookUpModel[];
    email:string;
}
export interface UserDetailViewModal {
    userName: string;
    fullName: string;
    phoneNo: string;
    status: number;
    roles: LookUpModel[];
    lastSeen: string;
    regOn: string;
    email:string;

}
