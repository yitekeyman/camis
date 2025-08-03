import React, { FormEvent } from 'react';
import { Row, Card, Button, Modal, Form, Input, Select, Table } from 'antd';
import roles from '../../assets/data/UserRole.json';
import SecurityService from '../../_setup/services/security.service';
import { Link } from 'react-router-dom';
import { AddUsers } from './add';
import { EditUser } from './edit';
import { ChangePassword } from './change_password';
interface IStateProps {
    registerUserModal : boolean,
    editUserModal : boolean,
    changePasswordModal : boolean,
    confirmDirty : boolean,
    users : any[] | undefined,
    selectedUserId : number,
    selectedUser : any,
    selectedUsername : string,
    
}

const { Option } = Select;

export class Users extends React.Component<any,IStateProps>{

    securityService : SecurityService;
    editRef : any;

    constructor(props : any){
        super(props);
        this.state = {
            registerUserModal : false,
            confirmDirty : false,
            users : [],
            editUserModal : false,
            selectedUserId : 0,
            changePasswordModal : false,
            selectedUser : {},
            selectedUsername : '',
        }


        this.securityService = new SecurityService(this.props);
        this.toggleRegisterModal = this.toggleRegisterModal.bind(this);
        this.toggleEditModal = this.toggleEditModal.bind(this);
        this.ChangePass =this.ChangePass.bind(this);
        this.toggleChangePasswordModal = this.toggleChangePasswordModal.bind(this);
        this.registerUser = this.registerUser.bind(this);
        this.getUsers = this.getUsers.bind(this);

        this.editRef = React.createRef();
        
    }

    componentDidMount(){
      var self = this;
        this.securityService.GetUsers().then(function(response){
            self.setState({ users : response.data });
        })
    }

    getUsers(){
      var self = this;
      this.securityService.GetUsers().then(function(response){
          self.setState({ users : response.data });
      })
    }

    toggleRegisterModal(){
        this.props.form.resetFields();
        this.props.form.setFieldsValue({"username" : "" , "fullname" : "", "roles" : []});
        this.setState({ registerUserModal : !this.state.registerUserModal})
    }

    toggleEditModal(){
      this.setState({ editUserModal : !this.state.editUserModal });
    }

    toggleChangePasswordModal(){
      this.setState({ changePasswordModal : !this.state.changePasswordModal });
    }

    EditUser(id : number){
      this.props.form.resetFields();
      var user = this.state.users!.filter(function(item: any){ return item.id == id})[0];
      this.setState({ selectedUserId : id, selectedUser : user }, this.toggleEditModal )
    };

    ChangePass(id : number){
      this.props.form.resetFields();
      var user = this.state.users!.filter(function(item: any){ return item.id == id})[0];
      this.setState({ selectedUserId : id, selectedUser : user, selectedUsername : user.userName }, this.toggleChangePasswordModal )
    };

    registerUser(e : FormEvent<HTMLFormElement>){
      e.preventDefault();
      var self = this;
      this.props.form.validateFields((err: any, values : any) => {
        if(!err){
            console.log(values);
            self.securityService.Register({
              FullName : values.fullname,
              Username : values.username,
              Password : values.password,
              PhoneNo : '',
              Roles : values.roles
            }).then((value) => {
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

        const column = [{
          title : "Name",
          dataIndex : "fullName"
        },
       { title : "Username", dataIndex : "userName"} ,
       { title : "Status", render : (d : any) => <p>{d.status == 1? 'Active' : 'Deactive'}</p>},
      { title : "Role", render : (d : any) => <p>{d.roles.join( ' , ')}</p>},
      { title : "Action ", render : (d : any) => <div><Button onClick={() => this.EditUser(d.id)} icon="edit" />
      <Button onClick={() => this.ChangePass(d.id)} icon="lock" /></div>}  ]

        return(
            <Row>
               
            <AddUsers {...this.props} toggleRegisterModal={this.toggleRegisterModal} registerUserModal={this.state.registerUserModal} getUsers={this.getUsers}/>
            <EditUser ref={this.editRef} {...this.props} toggleEditModal={this.toggleEditModal} editUserModal={this.state.editUserModal} user={this.state.selectedUser} getUsers={this.getUsers}/>
            <ChangePassword {...this.props} toggleChangePasswordModal={this.toggleChangePasswordModal} changePasswordModal={this.state.changePasswordModal} user={this.state.selectedUser} username={this.state.selectedUsername} />
                <Card
                title="Users"
                extra={<Button onClick={this.toggleRegisterModal}>Add User</Button>}
                >
                    <Table
                      dataSource ={this.state.users}
                      columns={column}
                    />
                </Card>
                </Row>
        )
    }
}