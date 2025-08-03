import React, { Component, FormEvent } from 'react'
import './login.scss';
import {Button, Input, Checkbox, Card, Row, Col} from 'antd';
import 'antd/dist/antd.css';
//import Login from '../modules/shared/login/login'
import {
  Layout, Menu, Breadcrumb, Icon, Form
} from 'antd';
import SecurityService from '../../_setup/services/security.service';
import {connect } from 'react-redux';
import { mapStateToProps } from '../../_setup/mapStateToProps';
import { mapDispatchToProps } from '../../_setup/mapDispatchToProps';
import LandService from '../../_setup/services/land.service';

const FormItem = Form.Item;
const {
  Header, Content, Footer, Sider,
} = Layout;
const SubMenu = Menu.SubMenu;

class L extends React.Component<any,any> {
  state = {
    collapsed: false,
  };
  private SecurityService  = new SecurityService(this.props);
  private LandService = new LandService(this.props);
  
  
  componentDidMount(){
    console.log(this);
  }

  componentWillMount(){
    if(this.props.authenticated && this.props.session != undefined){
      this.props.history.push('/')
    }
  }

  toggle = () => {
    this.setState({
      collapsed: !this.state.collapsed,
    });
  }

  load = () => {
    this.props.loader.start();
  }

  stop = () => {
    this.props.loader.stop();
  }


  handleSubmit = (e : FormEvent) => {
    var test : any = {
      accessibilityType : [{id : '1', name :"Name"}]
    }
    var self = this;
    e.preventDefault();
    this.props.form.validateFields((err:any, values:any) => {
      if (err == undefined) {
          this.SecurityService.Login({UserName : values.userName, Password: values.password}).then(function(response){
            console.log(response.data);
            self.props.auth.login(response.data);
            self.LandService.GetInitData().then((response) => self.props.base.set(response.data));
            self.props.history.push('/report');
          }).catch((error) => {console.log(error)})
      }
    });
  };

  render() {
    const { getFieldDecorator } = this.props.form;
    return (
      <Row>
        <Col span={7} offset={14}>
        <Card
        className="login-form"
      title="Login">
      
      
      <Form onSubmit={this.handleSubmit} >
        <FormItem>
          {getFieldDecorator("userName", {
            rules: [
              { required: true, message: "Please input your username!" },
             
            ]
          })(
            <Input
              prefix={<Icon type="user" style={{ fontSize: 13 }} />}
              placeholder="Username"
            />
          )}
        </FormItem>

        <FormItem>
          {getFieldDecorator("password", {
            rules: [{ required: true, message: "Please input your Password!" }]
          })(
            <Input
              prefix={<Icon type="lock" style={{ fontSize: 13 }} />}
              type="password"
              placeholder="Password"
            />
          )}
        </FormItem>

        <FormItem>
         
          <Button
            type="primary"
            htmlType="submit"
            className="login-form-button"
          >
            Log in
          </Button>
         
        </FormItem>
      </Form>
      </Card>
        </Col>
      </Row>
      
    );
  }
}
const Login = connect(mapStateToProps,mapDispatchToProps)(Form.create()(L));
export {Login};
//export { L as Login }