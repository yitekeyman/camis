import React, { Component, FormEvent } from 'react';
import { Button, Form, Select } from 'antd';
import ReportService from '../../../_setup/services/report.service';
import Swal from 'sweetalert2';
import LandService from '../../../_setup/services/land.service';

const { Option } = Select;

interface IStateProps {
    regionList : any[],
}

export default class LandSynchronize extends Component<any, IStateProps>{
    reportService : ReportService;
    landService : LandService;
    constructor(props : any){
        super(props);
        this.state = {
            regionList : []
        }
        this.reportService = new ReportService(this.props);
        this.landService = new LandService(this.props);
        this.GetAllRegions = this.GetAllRegions.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount(){
        this.GetAllRegions();
    }

    GetAllRegions(){
        var self = this;
        this.reportService.GetAllRegion().then((response) => {
            console.log(response.data);
            self.setState({ regionList : response.data });
        })
    }

    handleSubmit(e : FormEvent<HTMLFormElement>){
        var self = this;
        e.preventDefault();
        this.props.form.validateFields((err : any, values : any) => {
            if(!err){
                console.log(values.regions);
                Swal.showLoading();
                self.landService.SynchronizeLand(values.regions).then((response) => {
                    console.log(response);
                    Swal.close();
                    Swal.fire("",response.data,"success");
                    self.props.updateList();
                })
                //.catch((error) => { Swal.close(); Swal.fire("",error,"error")});
            }
        })

    }



    render(){
        const { getFieldDecorator, getFieldValue } = this.props.form;
        return(
            <div>
            <h1>Synchronize Land </h1>
            <Form onSubmit={this.handleSubmit}>
            <Form.Item label="Region" style={{display : 'block'}} >
                            {
                                getFieldDecorator('regions',{
                                    rules : [
                                        { required :true , message : "Select Region"}
                                    ]
                                })(
                                    <Select placeholder="Select Regions" mode="multiple">
                                       {this.state.regionList.map((item : any) =>{
                                           return <Option value={item.csaregionid}>{item.csaregionnameeng}</Option>
                                       })}
                                    </Select>
                                )
                            }
                        </Form.Item>
                        <Button.Group>
                        <Button icon="sync" type="primary" htmlType="submit">
                        Synchronize
                    </Button>
                    <Button  onClick={this.props.close}>Close</Button>
                        </Button.Group>
                       
            </Form>
            </div>

        )
    }
}