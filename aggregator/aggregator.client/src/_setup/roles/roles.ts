const adminNav = [
    {
        path : '/report',
        icon : 'dashboard',
        title : "Report",
    },
    {
        path : '/lands',
        icon : "compass",
        title : "Land Bank"
    },
    {
        path : '/users',
        icon : 'user',
        title : "Users"
    },
    {
        path : '/regions',
        icon : 'environment',
        title : "Regions"
    }
]

const defaultNav = [    {
    path : '/report',
    icon : 'dashboard',
    title : "Report",
},
{
    path : '/lands',
    icon : "compass",
    title : "Land Bank"
},];

export function GetNaviations(role:number) {

    switch (role) {
        case 1:
            return adminNav;
            break;
    
        default:
            return defaultNav
            break;
    }
}