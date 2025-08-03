import React, { FormEvent } from 'react';
import { Row, Card, Button, Modal, Form, Input, Select, Table } from 'antd';
import roles from '../../assets/data/UserRole.json';
import SecurityService from '../../_setup/services/security.service';
import { Link } from 'react-router-dom';

interface IStateProps {
    registerUserModal : boolean
    confirmDirty : boolean,
    users : any[] | undefined,
}

const { Option } = Select;

export class AddUsers extends React.Component<any,IStateProps>{

    securityService : SecurityService;

    constructor(props : any){
        super(props);
        this.state = {
            registerUserModal : false,
            confirmDirty : false,
            users : []
        }


        this.securityService = new SecurityService(this.props);
        this.toggleRegisterModal = this.toggleRegisterModal.bind(this);
        this.registerUser = this.registerUser.bind(this);
    }

    componentDidMount(){
        this.setState({ registerUserModal : this.props.registerUserModal });
    }

    componentWillReceiveProps(nextProps : any){
        this.setState({ registerUserModal : nextProps.registerUserModal });
    }



    toggleRegisterModal(){
        this.props.toggleRegisterModal();
    }

    registerUser(e : FormEvent<HTMLFormElement>){
      e.preventDefault();
      var self = this;
      this.props.form.validateFields((err: any, values : any) => {
        var s = Object.keys(err);
        console.log(s);
        if(s.length == 1 && s.indexOf("status") != -1 ){
            console.log(values);
            self.securityService.Register({
              FullName : values.fullname,
              Username : values.username,
              Password : values.password,
              PhoneNo : '',
              Roles : values.roles
            }).then((value) => {
              
              self.props.getUsers();
              self.toggleRegisterModal();
            });
        }
    })
    }

    handleConfirmBlur = (e : any) => {
        const value = e.target.value;
        this.setState({ confirmDirty: this.state.confirmDirty || !!value });
      };
    
      compareToFirstPassword = (rule : any, value : any, callback : any) => {
        const form = this.props.form;
        if (value && value !== form.getFieldValue('password')) {
          callback('Two passwords that you enter is inconsistent!');
        } else {
          callback();
        }
      };



    
      validateToNextPassword = (rule : any, value : any, callback : any) => {
        const form = this.props.form;
        if (value && this.state.confirmDirty) {
          form.validateFields(['confirm'], { force: true });
        }
        callback();
      };
      
    render(){
        const { getFieldDecorator } = this.props.form;
    
        const formItemLayout = {
          labelCol: {
            xs: { span: 24 },
            sm: { span: 8 },
          },
          wrapperCol: {
            xs: { span: 24 },
            sm: { span: 16 },
          },
        };
        const tailFormItemLayout = {
          wrapperCol: {
            xs: {
              span: 24,
              offset: 0,
            },
            sm: {
              span: 16,
              offset: 8,
            },
          },
        };


        return(
         
                <Modal
                title="Register User"
                onCancel={this.toggleRegisterModal}
                onOk={this.registerUser}
                visible={this.state.registerUserModal}
                >
                <Form {...formItemLayout} onSubmit={this.registerUser}>
                        <Form.Item label="Username">
                        {getFieldDecorator('username', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input your Username',
                            },
                            ],
                        })(<Input />)}
                        </Form.Item>
                        
                        <Form.Item label="Password" hasFeedback>
                        {getFieldDecorator('password', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input your password!',
                            },
                            {
                                validator: this.validateToNextPassword,
                            },
                            ],
                        })(<Input.Password />)}
                        </Form.Item>
                    <Form.Item label="Confirm Password" hasFeedback>
                    {getFieldDecorator('confirm', {
                        rules: [
                        {
                            required: true,
                            message: 'Please confirm your password!',
                        },
                        {
                            validator: this.compareToFirstPassword,
                        },
                        ],
                    })(<Input.Password onBlur={this.handleConfirmBlur} />)}
                    </Form.Item>

                    <Form.Item label="Fullname">
                        {getFieldDecorator('fullname', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input your fullname',
                            },
                            ],
                        })(<Input />)}
                        </Form.Item>

                        
                    <Form.Item label="Role">
                        {getFieldDecorator('roles', {
                            rules: [
                            {
                                required: true,
                                message: 'Please select role',
                            },
                            ],
                        })(<Select mode="multiple">
                          {
                            roles.map((item) => {
                              return <Option value={item.id}>{item.name}</Option>
                            })
                          }
                        </Select>)}
                        </Form.Item>
                </Form>
                
                </Modal>
            
               
        )
    }
}