import React, { FormEvent } from 'react';
import { Row, Card, Button, Modal, Form, Input, Select, Table } from 'antd';
import roles from '../../assets/data/UserRole.json';
import SecurityService from '../../_setup/services/security.service';
import { Link } from 'react-router-dom';

interface IStateProps {
    changePasswordModal : boolean
    confirmDirty : boolean,
    username : string,
    user : any,
}

const { Option } = Select;

const getRoles = (values : string[]) => {
    if(values != undefined && values.length > 0){
            var selected = roles.filter((item) => {return values.indexOf(item.name) != -1} );
            var m = selected.map((i) => {return i.id} );
            return m;
    }
    else
        return [];

}

export class ChangePassword extends React.Component<any,IStateProps>{

    securityService : SecurityService;

    constructor(props : any){
        super(props);
        this.state = {
            changePasswordModal : false,
            confirmDirty : false,
            username : '',
            user : null,

        }


        this.securityService = new SecurityService(this.props);
        this.toggleChangePModal = this.toggleChangePModal.bind(this);
        this.changePassword = this.changePassword.bind(this);
    }

    componentWillMount(){
        this.setState({ changePasswordModal : this.props.changePasswordModal, username : this.props.username , user : this.props.user });
    }

    componentWillReceiveProps(nextProps : any){
        this.setState({ changePasswordModal : nextProps.changePasswordModal, username : nextProps.username, user : nextProps.user  });
    }



    toggleChangePModal(){
        this.props.toggleChangePasswordModal();
    }

    changePassword(e : FormEvent<HTMLFormElement>){
      e.preventDefault();
      var self = this;
      this.props.form.validateFields((err: any, values : any) => {
         var s = Object.keys(err);
          var data = {
              userName : values.username,
              newPassword : values.password
          };
          console.log(data);
          console.log(err);
          if(s.indexOf("password") == -1){
              self.securityService.ResetPassword(data).then((response) => {
                  self.toggleChangePModal();
              })
          }
        //  var s = Object.keys(err);
        //  if(s.length == 2 && s.indexOf("password") != -1 && s.indexOf("confirm") != -1){
        //      self.securityService.Update({
        //          FullName : values.fullname,
        //          Id : this.state.user.id,
        //          PhoneNo : this.state.user.phoneNo,
        //          Roles : values.roles,
        //          Status : values.status,
        //          UserName : values.username
        //      }).then(function(response){
        //         self.props.getUsers(); 
        //         self.toggleChangePModal();

        //      });
        //  }
        //self.toggleChangePModal();
       
    })
    }

   
      
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
                    this.state.username != '' || this.state.username != undefined ?
                <Modal
                title="Change Password"
                onCancel={this.toggleChangePModal}
                onOk={this.changePassword}
                visible={this.state.changePasswordModal}
                okText="Edit"
                >
                <Form {...formItemLayout} onSubmit={this.changePassword}>
                        <Form.Item label="Username">
                        {getFieldDecorator('username', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input your Username',
                            },
                            ],
                            initialValue : this.state.user.userName
                        })(<Input disabled={true} />)}
                        </Form.Item>
                      
                

                    <Form.Item label="Password">
                        {getFieldDecorator('password', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input password',
                            },
                            ],
                        })(<Input.Password />)}
                        </Form.Item>
                 
                </Form>
                
                </Modal> : null 
            
               
        )
    }
}