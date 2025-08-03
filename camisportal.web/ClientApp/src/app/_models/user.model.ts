export interface LoginUser {
    username:string;
    password:string;
}

export interface SystemUserRequest {
    FullName:string;
    Role:number;
    PhoneNo:string;
    EMail:string;
    Region:string;
    UserName:string;
    Password:string;
    CamisUserName:string;
    CamisPassword:string;
}
export interface SystemUserResponse {
    Id:string;
    fullName:string;
    role:number;
    phoneNo:string;
    eMail:string;
    region:string;
    userName:string;
    password:string;
    camisUserName:string;
    camisPassword:string;
}