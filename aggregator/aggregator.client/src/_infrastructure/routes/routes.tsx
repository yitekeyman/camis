import React from 'react';
import { Home } from '../../modules/home/home';
import { Users } from '../../modules/users/users';
import { Regions } from '../../modules/configuration/regions';
import { LandList } from '../../modules/land/land_list/land_list';
import { Profile } from '../../modules/land/land_profile/land_profile';
import { Report } from '../../modules/report/report';



// const User : React.Component<any,any> = React.lazy(() => import('../../modules/users/users'));
// const Objects = React.lazy(() => import('../../modules/objects/objects'));
// const Home = React.lazy(() => import('../../modules/home/home'));


const routes = [
    { path : '/', exact : true, name : "Home", component : Home },
    { path : '/report', exact : true, name : "Report", component : Report },
    { path : '/users', exact : true, name : "Users", component : Users },
    { path : '/regions', exact : true, name : "Regions", component : Regions },
    { path : '/lands', exact : true, name : "Land Bank", component : LandList },
    { path : '/land/profile/:id', name : "Land Profile", component : Profile }
]

export {routes};