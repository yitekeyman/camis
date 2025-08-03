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
}

export interface UpdatePassword {
    OldPassword: string;
    NewPassword: string;
}

export interface ResetPassword {
    UserName: string;
    NewPassword: string;
}

export interface UserModel {
    userName: string;
    fullName: string;
    phoneNo: string;
    status: number;
    roles: string[];
}
export interface UserDetailViewModal {
    userName: string;
    fullName: string;
    phoneNo: string;
    status: number;
    roles: string[];
    lastSeen: string;
    regOn: string;

}
