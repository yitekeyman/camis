export interface IMainShellRoute {
    route:string;
    title:string;
    icon:string;
    click:string;
    child:any;
}

export interface IChildShellRoute {
    route:string;
    title:string;
    icon:string;
    click:any;
}