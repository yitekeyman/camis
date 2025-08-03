import React, { Component } from 'react'
import './App.scss';
import {Button} from 'antd';
import 'antd/dist/antd.css';
//import Login from '../modules/shared/login/login'
import {
  Layout, Menu, Breadcrumb, Icon,
} from 'antd';
import { HashRouter, Route, Switch } from 'react-router-dom';
import { Dashboard } from '../modules/dashboard/dashboard';
import { Login } from '../modules/login/login';



const {
  Header, Content, Footer, Sider,
} = Layout;
const SubMenu = Menu.SubMenu;


class App extends Component{
  render(){
    return (  
      <HashRouter>
        <Switch>
          <Route exact path="/login" component={(Login)} {...this.props}/>
          <Route  path="/" component={Dashboard} {...this.props}/>




        </Switch>
      </HashRouter>
    )
  }
}

export {App};