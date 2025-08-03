import React, { FormEvent } from 'react';
import { Row, Card, Button, Modal, Form, Input, Select, Table } from 'antd';
import roles from '../../assets/data/UserRole.json';
import SecurityService from '../../_setup/services/security.service';
import { Link } from 'react-router-dom';

interface IStateProps {
    editUserModal : boolean
    confirmDirty : boolean,
    user : any
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

export class EditUser extends React.Component<any,IStateProps>{

    securityService : SecurityService;

    constructor(props : any){
        super(props);
        this.state = {
            editUserModal : false,
            confirmDirty : false,
            user : null,

        }


        this.securityService = new SecurityService(this.props);
        this.toggleEditModal = this.toggleEditModal.bind(this);
        this.edituser = this.edituser.bind(this);
    }

    componentWillMount(){
        this.setState({ editUserModal : this.props.editUserModal, user : this.props.user });
    }

    componentWillReceiveProps(nextProps : any){
        this.setState({ editUserModal : nextProps.editUserModal, user : nextProps.user });
    }



    toggleEditModal(){
        this.props.toggleEditModal();
    }

    edituser(e : FormEvent<HTMLFormElement>){
      e.preventDefault();
      var self = this;
      this.props.form.validateFields((err: any, values : any) => {
         var s = Object.keys(err);
         if(s.length == 2 && s.indexOf("password") != -1 && s.indexOf("confirm") != -1){
             self.securityService.Update({
                 FullName : values.fullname,
                 Id : this.state.user.id,
                 PhoneNo : this.state.user.phoneNo,
                 Roles : values.roles,
                 Status : values.status,
                 UserName : values.username
             }).then(function(response){
                self.props.getUsers(); 
                self.toggleEditModal();

             });
         }
       
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
                    this.state.user != {} || this.state.user != undefined ?
                <Modal
                title="Edit User"
                onCancel={this.toggleEditModal}
                onOk={this.edituser}
                visible={this.state.editUserModal}
                okText="Edit"
                >
                <Form {...formItemLayout} onSubmit={this.edituser}>
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
                      
                

                    <Form.Item label="Fullname">
                        {getFieldDecorator('fullname', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input your fullname',
                            },
                            ],
                            initialValue : this.state.user.fullName
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
                            initialValue : getRoles(this.state.user.roles),
                        })(<Select mode="multiple">
                          {
                            roles.map((item) => {
                              return <Option value={item.id}>{item.name}</Option>
                            })
                          }
                        </Select>)}
                        </Form.Item>
                    <Form.Item label="status">
                        {
                            getFieldDecorator('status',{
                                rules : [ { required : true }],
                                initialValue : this.state.user.status
                            })(
                                <Select>
                                    <Option value={0}>Deactive</Option>
                                    <Option value={1}>Active</Option>
                                </Select>

                            )
                        }
                    </Form.Item>
                </Form>
                
                </Modal> : null 
            
               
        )
    }
}