import React, { Component } from 'react';

import { Button } from 'antd';
// import 'antd/dist/antd.css';  // DELETED – not in v5
import { Layout, Menu, Breadcrumb } from 'antd';
import { HashRouter, Route, Routes } from 'react-router-dom'; 
import { Dashboard } from '../modules/dashboard/dashboard';
import { Login } from '../modules/login/login';

const { Header, Content, Footer, Sider } = Layout;
const { SubMenu } = Menu;

class App extends Component {
  render() {
    return (
      <HashRouter>
        <Routes>
          <Route path="/login" element={<Login {...this.props} />} />
          <Route path="/" element={<Dashboard {...this.props} />} />
        </Routes>
      </HashRouter>
    );
  }
}

export { App };