import React, { FormEvent } from 'react';
import { Row, Card, Button, Modal, Form, Input, Select, Table } from 'antd';

import SecurityService from '../../_setup/services/security.service';
import { Link } from 'react-router-dom';
import ReportService from '../../_setup/services/report.service';


interface IStateProps {
    editUrlModal : boolean,
    regions : any[] | undefined,
    code : string | undefined,
    url : string | undefined,   
    username : string | undefined,
    password : string | undefined,    

}

const { Option } = Select;

export class Regions extends React.Component<any,IStateProps>{

    reportService : ReportService;
    editRef : any;

    constructor(props : any){
        super(props);
        this.state = {
            regions : [],
            editUrlModal : false,
            code : '',
            url : '',

            username : '',
            password : ''
        }


        this.reportService = new ReportService(this.props);
        this.toggleEditModal = this.toggleEditModal.bind(this);
        this.getRegions = this.getRegions.bind(this);
        this.openEditUrlModal = this.openEditUrlModal.bind(this);
        this.editUrl = this.editUrl.bind(this);

        this.editRef = React.createRef();
        
    }

    openEditUrlModal(code : string, url : string, username : string, password : string){
        this.props.form.resetFields();
        console.log(code);
        console.log(url);
        this.setState({  code , url, username, password }, this.toggleEditModal);

    }

    componentDidMount(){
        this.getRegions();
    }

    getRegions(){
      var self = this;
      this.reportService.GetAllRegion().then((response) => {
          console.log(response.data);
          self.setState({ regions : response.data });
      })
    }

    editUrl(){

        var self = this;
        this.props.form.validateFields((err : any ,values : any ) => {
            if(!err){
                self.reportService.SetRegionUrl(self.state.code!,values.url, values.username, values.password).then((response) => {
                    self.getRegions();
                    self.toggleEditModal();
                })
            }
        })
    }



    toggleEditModal(){
      this.setState({ editUrlModal : !this.state.editUrlModal });
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

        const columns = [
            { title : "Code", dataIndex : "csaregionid", key:"id"},
            { title : "Name", dataIndex : "csaregionnameeng"},
            { title : "Url", dataIndex : "url"},
            { title : "Username", dataIndex : "username"},
            { title : "Password", dataIndex : "password" } ,
            { title : "Edit", render : (d : any) => <Button onClick={() =>  this.openEditUrlModal(d.csaregionid, d.url, d.username, d.password)} icon="edit" /> }]
        


        return(
            <Row>
                   <Modal
                title="Edit Url"
                onCancel={this.toggleEditModal}
                onOk={this.editUrl}
                visible={this.state.editUrlModal}
                >
                <Form {...formItemLayout} >
                        <Form.Item label="Url">
                        {getFieldDecorator('url', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input url',
                            },
                            ],
                            initialValue : this.state.url
                        })(<Input />)}
                        </Form.Item>

                        <Form.Item label="Username">
                        {getFieldDecorator('username', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input username',
                            },
                            ],
                            initialValue : this.state.username
                        })(<Input />)}
                        </Form.Item>

                        <Form.Item label="Password">
                        {getFieldDecorator('password', {
                            rules: [
                            {
                                required: true,
                                message: 'Please input password',
                            },
                            ],
                            initialValue : this.state.password
                        })(<Input.Password />)}
                        </Form.Item>
                        
                      
                </Form>
                
                </Modal>
                <Card
                title="Regions"
                >
                    <Table
                    dataSource={this.state.regions}
                    columns={columns} />

                </Card>
                </Row>
        )
    }
}