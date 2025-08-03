import React, { Component, Suspense } from 'react'
import './dashboard.scss';
import {Button, Form, Spin, Row, Col} from 'antd';
import 'antd/dist/antd.css';
//import Login from '../modules/shared/login/login'
import {
  Layout, Menu, Breadcrumb, Icon,
} from 'antd';
import { Link, Switch, Route } from 'react-router-dom';


import { Container } from 'reactstrap';
import { connect } from 'react-redux'; 
import SecurityService from '../../_setup/services/security.service';
import {routes} from '../../_infrastructure/routes/routes';
import { GetNaviations } from '../../_setup/roles/roles';
import { mapStateToProps } from '../../_setup/mapStateToProps';
import { mapDispatchToProps } from '../../_setup/mapDispatchToProps';

const {
  Header, Content, Footer, Sider,
} = Layout;
const SubMenu = Menu.SubMenu;

class Dash extends React.Component<any,any> {

  private SecurityService : SecurityService; 

    constructor(props : any){
      super(props);
      this.state = {
        collapsed: false,
        pathname : "/"
      };
      this.SecurityService = new SecurityService(this.props);
      this.Logout = this.Logout.bind(this);
    }

  
  componentWillMount(){
    console.log(this.props);
    if(!this.props.authenticated || this.props.session == undefined){
      this.props.history.push('/login');
      this.props.auth.logout();
    }
    else{
      this.setState({
        pathname : this.props.location.pathname
      });
    }
    console.log(this.props);
  }

  componentWillReceiveProps(nextProps :any){
    this.setState({
      pathname : nextProps.location.pathname
    });
  }

  Logout(){
    var self = this;
    self.SecurityService.Logout(self.props.session.payrollSessionID).then(function(response){
      console.log(response.data);
      self.props.auth.logout();
      self.props.history.push('/login');
    })
  }

  toggle = () => {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  }

  loading = () => <div className="animated fadeIn pt-1 text-center">Loading...</div>
  
  render() {
    return (
      <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={this.state.collapsed}
        onCollapse={this.toggle}
      >
        <Row>
          <Col span={6}>
          <img src="../camis.png" className="logo-image" />
          </Col>
          {
            !this.state.collapsed ? 
            <Col span={18}>
            <p className="logo_title" >Aggregator </p>
           <p></p>
         </Col>
            : null
          }
        
          </Row>
        
       
        
        <Menu theme="dark" mode="inline"
        defaultSelectedKeys={[this.state.pathname]}
        >
                {
          this.props.session != undefined ?
          GetNaviations(this.props.session.role).map(function(item){
            return (
            <Menu.Item key={item.path}>
                      <Link to={item.path}>
                      <Icon type={item.icon} />
                        <span>{item.title}</span>
                    </Link>
                        
                      </Menu.Item>
            )
          }) : null
        }
      
        </Menu>
      </Sider>
      <Layout className="layout_container">
      <Header style={{ background: '#fff', padding: 0 }}>
           
                 <Menu
        theme="light"
        mode="horizontal"
        style={{ lineHeight: '64px' }}
      >
         <Icon
              className="trigger"
              type={this.state.collapsed ? 'menu-unfold' : 'menu-fold'}
              onClick={this.toggle}
            />
        <Menu.Item key="1" className="logout" onClick={this.Logout}>Logout</Menu.Item>
      </Menu>
          </Header>
        <Content style={{ margin: '50px 50px' }} >
        <Spin
                        spinning={false}
                        >
                       
                        <Container fluid>   
                                <Suspense fallback={this.loading}>
                                <Switch>
                                    {routes.map((route, idx) => {
                                    return <Route
                                        key={idx}
                                        path={route.path}
                                        exact={route.exact}
                                        name={route.name}
                                     
                                        render = {
                                            (props : any) => (<route.component {...this.props} {...props} />)
                                        }
                                    />
                                    })}
                                </Switch>
                            </Suspense>    
                        </Container>
                        </Spin>
        </Content>
        <Footer style={{ textAlign: 'center' }} className="footer_layout">
          CAMIS Aggregator ©2019 Created by INTAPS Consultancy PLC
        </Footer>
      </Layout>
    </Layout>
    );
  }
}


const Dashboard = connect(mapStateToProps,mapDispatchToProps)(Form.create()(Dash));
export {Dashboard};