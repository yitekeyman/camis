import React, { FormEvent } from 'react';
import { Card, Row, Form, Col, Select, Input, Icon, Button, DatePicker } from 'antd';
import ReportService from '../../_setup/services/report.service';
import ReportTypes from '../../assets/data/ReportTypes.json';
import './report.scss';
import Swal from 'sweetalert2';

const {Option } = Select;

interface IStateProps {
    regionList : any[],
    zoneList : any[],
    woredaList : any[],
    farmList : any[],

    summerizedBy : string;
    filteredBy : string;
    reportType : number,

    reportResponse : string,
    region : string


    


}

var summerizedTypes = [1,4,5,6,7,8,9,14,18,19,21]; //[1,4,5,6,7,8,9,14,18,19,21]
var filteredTypes = [1,2,3,4,7,8,9,10,12,13,20,21];  //[1,2,3,4,7,8,9,10,12,13,20,21];

let id = 0;
let sizeId = 0;
var list : number[] = [];


export class Report extends React.Component<any,IStateProps>{

    reportService : ReportService;

    constructor(props : any){
        super(props);

        this.state = {
            filteredBy : "",
            summerizedBy : "",

            regionList : [],
            zoneList : [],
            woredaList : [],
            farmList : [],
            reportType : 0,

            reportResponse : "",
            region : ""

 
        }

        this.reportService = new ReportService(this.props);
        this.GetAllRegions = this.GetAllRegions.bind(this);
        this.regionSelected = this.regionSelected.bind(this);
        this.zoneSelected= this.zoneSelected.bind(this);

        this.addDateField = this.addDateField.bind(this);
        this.removeDateField = this.removeDateField.bind(this);
        this.generateReport = this.generateReport.bind(this);
        this.isSummerizedReport = this.isSummerizedReport.bind(this);
        this.isFilteredReport = this.isFilteredReport.bind(this);

        this.regionRequired = this.regionRequired.bind(this);
        this.zoneRequired = this.zoneRequired.bind(this);
        this.woredaRequired = this.woredaRequired.bind(this);

        this.getFarms = this.getFarms.bind(this);

    }

    componentDidMount(){
        this.GetAllRegions();
        this.addDateField();
        this.addSizeField();

        for (var i = 2000; i <= 2025; i++) {
            list.push(i);
        }
    }

    GetAllRegions(){
        var self = this;
        this.reportService.GetAllRegion().then((response) => {
            console.log(response.data);
            self.setState({ regionList : response.data });
        })
    }

    regionSelected(value : any){
        var self = this;
        self.setState({ region : value },() => {
            console.log(value);
            if(this.state.reportType == 11){
                this.getFarms();
            }
        });
        
        self.reportService.GetZones(value).then((response) => {
            console.log(response.data);
            self.setState({ zoneList : response.data });
        });

    }

    getFarms(){
        var self = this;
        self.reportService.GetFarms(this.state.region).then( (response : any) => {
            self.setState({ farmList : response.data });
        })
    }

    handleChange(name : any, value : any){
        console.log(`${name}-${value}`)
        this.setState({ [name] : value} as Pick<IStateProps, keyof IStateProps>,() => {
            console.log(name);
            console.log(value);
            if(name == "reportType" && value == 11 && this.state.region != ""){
                this.getFarms();
            }
        } ) 
        
    }

    zoneSelected(value : any){
        var self = this;
        self.reportService.GetWoredas(value).then((response) => {
            console.log(response.data);
            self.setState({ woredaList : response.data });
        })
    }

    addSizeField = () => {
        const { form }  = this.props;
        const sizeKeys = form.getFieldValue('sizeKeys');
        const nextKeys = sizeKeys.concat(sizeId++);
        this.props.form.setFieldsValue({ sizeKeys: nextKeys });
    }

    removeSizeField = (k : any ) => {
        const { form } = this.props;
        const sizeKeys = form.getFieldValue('sizeKeys');
        if(sizeKeys.lenght === 1){
            return;
        }
        form.setFieldsValue({ sizeKeys : sizeKeys.filter((key : any) => key !== k), })
    }

    removeDateField =( k : any) => {
        const { form } = this.props;
        const dateKeys = form.getFieldValue('dateKeys');
        if(dateKeys.length === 1){
            return;
        }
        form.setFieldsValue({ dateKeys : dateKeys.filter((key : any) => key !== k), })
    }

    generateReport(e : FormEvent<HTMLFormElement>){
        var self = this;
        e.preventDefault();
        this.props.form.validateFields((err : any ,values : any) => {
           if(!err){
               const { dateKeys, dates, sizeKeys, sizes }  = values;
               values.dates = dateKeys.map( ( key : any ) => {if(dates[key] != undefined) {
                return dates[key];
                }});
               values.sizes = sizeKeys.map( ( key : any ) => sizes[key]);
               values.dates = values.dates.filter( (x : any) => x != undefined);
               values.sizes = values.sizes.filter( (x : any) => x != undefined);
               values.dates = values.dates.map( (x : any) => ({ 'date' : x }) )
               values.farmSizes = values.sizes.map( (x : any) => ({ 'size' : x }) )
               values.selectedReportType = values.reportType;
               console.log(values);
               Swal.showLoading();
               self.reportService.GetReport(values).then((response : any) => {
                   console.log(response.data);
                   self.setState({ reportResponse : response.data });
                   Swal.close();
               });
               
            //    .catch( error =>  { console.log(error);})
           }
        })
    }

    addDateField = () => {
        const { form }  = this.props;
        const dateKeys = form.getFieldValue('dateKeys');
        const nextKeys = dateKeys.concat(id++);
        this.props.form.setFieldsValue({ dateKeys: nextKeys });
    }



    isSummerizedReport(){
        return summerizedTypes.indexOf(this.state.reportType) != -1 ;
    }

    isFilteredReport(){
        return true;
        // return filteredTypes.indexOf(this.state.reportType) != -1 ;
    }

    regionRequired(){
        return this.isFilteredReport() &&  ["1", "2","3"].indexOf(this.state.filteredBy) != -1;
    }

    zoneRequired(){
        return this.isFilteredReport() &&  ["2","3"].indexOf(this.state.filteredBy) != -1;
    }

    woredaRequired(){
        return this.isFilteredReport() &&  ["3"].indexOf(this.state.filteredBy) != -1;
    }



    render(){

        const { getFieldDecorator, getFieldValue } = this.props.form;
        const formItemLayout = {
            labelCol: {
              xs: { span: 24 },
              sm: { span: 12 },
              fontWeight : "bold"
            },
            wrapperCol: {
              xs: { span: 24 },
              sm: { span: 12 },
            },
          };


          const formItemLayout2 = {
            labelCol: {
              xs: { span: 24 },
              sm: { span: 8 },
              fontWeight : "bold"
            },
            wrapperCol: {
              xs: { span: 24 },
              sm: { span: 16 },
            },
          };

          const formItemLayoutWithOutLabel = {
            wrapperCol: {
              xs: { span: 24, offset: 0 },
              sm: { span: 16, offset: 8 },
            },
          };

          getFieldDecorator('dateKeys', { initialValue: [] });
          getFieldDecorator('sizeKeys', { initialValue: [] });

          const dateKeys = getFieldValue('dateKeys');
          const sizeKeys = getFieldValue('sizeKeys');

          const dateFormItems = dateKeys.map((k : any, index : any) => (
            <Form.Item
                labelCol={{sm : { span : 4}}}
              {...(index === 0 ? formItemLayout2 : formItemLayoutWithOutLabel)}
              label={index === 0 ? 'Date' : ''}
              required={false}
              key={k}
            >
              {getFieldDecorator(`dates[${k}]`, {
               // validateTrigger: ['onChange', 'onBlur'],
                rules: [
                  {
                    required: this.state.reportType == 5,
                    message: "Please select date.",
                  },
                ],
              })(<DatePicker   style={{width : '80%', marginRight : 8}}/>)}
              {dateKeys.length > 1 ? (
                <Icon
                  className="dynamic-delete-button"
                  type="minus-circle-o"
                  onClick={() => this.removeDateField(k)}
                />
              ) : null}
            </Form.Item>
          ));

          const sizeFormItems = sizeKeys.map((k : any, index : any) => (
            <Form.Item
                labelCol={{sm : { span : 4}}}
              {...(index === 0 ? formItemLayout2 : formItemLayoutWithOutLabel)}
              label={index === 0 ? 'Size' : ''}
              required={false}
              key={k}
            >
              {getFieldDecorator(`sizes[${k}]`, {
               // validateTrigger: ['onChange', 'onBlur'],
                rules: [
                  {
                    required: this.state.reportType == 6,
                    message: "Please input size.",
                  },
                ],
              })(<Input   style={{width : '80%', marginRight : 8}}/>)}
              {sizeKeys.length > 1 ? (
                <Icon
                  className="dynamic-delete-button"
                  type="minus-circle-o"
                  onClick={() => this.removeSizeField(k)}
                />
              ) : null}
            </Form.Item>
          ));


        return(
            <div>
                <Card
                title="Report">
                    <Form className="" {...formItemLayout} onSubmit={this.generateReport}>
                <Row gutter={24}>
                <Col span={10}>
                        <Form.Item label="Report Types" style={{display : 'block'}} >
                            {
                                getFieldDecorator('reportType',{
                                    rules : [ { required : true, message : "Select Report Type "}]
                                })(
                                    <Select dropdownClassName="itemdropdown" onSelect={(value) => this.handleChange("reportType",value)}>
                                        {
                                            ReportTypes.map((item) => {
                                                return <Option value={item.id}>{item.title}</Option>
                                            })
                                        }
                                    </Select>
                                )
                            }
                        </Form.Item>
                       
                    </Col>
                    {
                        summerizedTypes.indexOf(this.state.reportType) != -1 ?
                        <Col span={6}>
                        {
                            
                        }
                        <Form.Item label="Summerized By" style={{display : 'block'}} 
                        >
                            {
                                getFieldDecorator('summerizedBy',{
                                 rules : [
                                     { required : false, message : "Select Summary type"}
                                 ]
                                })(
                                    <Select>

                                        <Option value="1">Region</Option>
                                        <Option value="2">Zone</Option>
                                        <Option value="3">Woreda</Option>
                                    </Select>
                                )
                            }
                        </Form.Item>
                       
                    </Col>

                        : null
                    }
                    {
                      
                        <Col span={6}>
                        <Form.Item label="Filtered By" style={{display : 'block'}} >
                            {
                                getFieldDecorator('filteredBy',{
                                    rules : [
                                        { required : this.isFilteredReport(), message : "Select Filter Type"}
                                    ]
                                })(
                                    <Select onSelect={ (value) => this.handleChange("filteredBy",value)}>
                                        {/* <Option value="4  ">All Regions</Option> */}
                                        <Option value="1">Region</Option>
                                        <Option value="2">Zone</Option>
                                        <Option value="3">Woreda</Option>
                                    </Select>
                                )
                            }
                        </Form.Item>
                        </Col>
           
                    }
                   
                        {
                            this.regionRequired() ?

                        <Col span={6}>
                        <Form.Item label="Region" style={{display : 'block'}} >
                            {
                                getFieldDecorator('regions',{
                                    rules : [
                                        { required :this.regionRequired , message : "Select Region"}
                                    ]
                                })(
                                    <Select onSelect={this.regionSelected} mode="multiple">
                                       {this.state.regionList.map((item : any) =>{
                                           return <Option value={item.csaregionid}>{item.csaregionnameeng}</Option>
                                       })}
                                    </Select>
                                )
                            }
                        </Form.Item>
                        </Col>
                            :null
                        }


                        {
                           this.zoneRequired() ?

                            <Col span={6}>
                            <Form.Item label="Zone" style={{display : 'block'}} >
                                {
                                    getFieldDecorator('zone',{
                                        rules : [
                                            { required : this.zoneRequired , message : "Select Zone"}
                                        ]
                                    })(
                                        <Select onSelect={this.zoneSelected} dropdownClassName="zoneDropDown">
                                           {this.state.zoneList.map((item : any) =>{
                                               return <Option value={item.nrlaisZoneid}>{item.csazonenameeng}</Option>
                                           })}
                                        </Select>
                                    )
                                }
                            </Form.Item>
                            </Col>
                            :null
                        }



                        {
                            this.woredaRequired() ?

                        <Col span={6}>
                        <Form.Item label="Woreda" style={{display : 'block'}} >
                            {
                                getFieldDecorator('woreda',{
                                    rules : [
                                        { required : this.woredaRequired, message : "Select Woreda"}
                                    ]
                                })(
                                    <Select dropdownClassName="woredaDropDown" >
                                       {this.state.woredaList.map((item : any) =>{
                                           return <Option value={item.nrlaisWoredaid}>{item.woredanameeng}</Option>
                                       })}
                                    </Select>
                                )
                            }
                        </Form.Item>
                        </Col>
                            :null
                        }


                        {
                            this.state.reportType == 5 ?
                        <Col span={6}>
                        {dateFormItems}
                        <Form.Item {...formItemLayoutWithOutLabel}>
                        <Button type="dashed" onClick={this.addDateField} style={{ width: '100%' }}>
                            <Icon type="plus" /> Add Date Field
                        </Button>
                        </Form.Item>

                        </Col>
                            : null
                        }

                        {
                            this.state.reportType == 6 ?
                        <Col span={6}>
                        {sizeFormItems}
                        <Form.Item {...formItemLayoutWithOutLabel}>
                        <Button type="dashed" onClick={this.addSizeField} style={{ width: '100%' }}>
                            <Icon type="plus" /> Add Size Field
                        </Button>
                        </Form.Item>

                        </Col>
                            : null
                        }

                        {
                            this.state.reportType == 10 ?
                            <Col span={6}>
                                <Form.Item label="From Date" style={{display : 'block'}}>
                                {getFieldDecorator(`fromDate`, {
                                // validateTrigger: ['onChange', 'onBlur'],
                                    rules: [
                                    {
                                        required: this.state.reportType == 10,
                                        message: "Please select date.",
                                    },
                                    ],
                                })(<DatePicker   style={{width : '100%', marginRight : 8}}/>)}
                                </Form.Item>
                            </Col>

                            : null
                        }

{
                            this.state.reportType == 10 ?
                            <Col span={6}>
                                <Form.Item label="End Date" style={{display : 'block'}}>
                                {getFieldDecorator(`endDate`, {
                                // validateTrigger: ['onChange', 'onBlur'],
                                    rules: [
                                    {
                                        required: this.state.reportType == 10,
                                        message: "Please select date.",
                                    },
                                    ],
                                })(<DatePicker   style={{width : '100%', marginRight : 8}}/>)}
                                </Form.Item>
                            </Col>

                            : null
                        }

                        {
                            this.state.reportType == 11 ?
                         
                            <Col span={6}>
                            <Form.Item label="Start Year" style={{display : 'block'}} >
                                {
                                    getFieldDecorator('startYear',{
                                        rules : [ { required : this.state.reportType == 11, message : "Select Start Year "}]
                                    })(
                                        <Select >
                                            {
                                                list.map((item) => {
                                                    return <Option value={item}>{item}</Option>
                                                })
                                            }
                                        </Select>
                                    )
                                }
                            </Form.Item>
                           
                        </Col>

                            : null
                        }

{
                            this.state.reportType == 11 ?
                         
                            <Col span={6}>
                            <Form.Item label="End Year" style={{display : 'block'}} >
                                {
                                    getFieldDecorator('endYear',{
                                        rules : [ { required : this.state.reportType == 11, message : "Select End Year "}]
                                    })(
                                        <Select >
                                            {
                                                list.map((item) => {
                                                    return <Option value={item}>{item}</Option>
                                                })
                                            }
                                        </Select>
                                    )
                                }
                            </Form.Item>
                           
                        </Col>

                            : null
                        }

{
                            this.state.reportType == 11 ?
                         
                            <Col span={6}>
                            <Form.Item label="Farm" style={{display : 'block'}} >
                                {
                                    getFieldDecorator('farmId',{
                                        rules : [ { required : this.state.reportType == 11, message : "Select Farm "}]
                                    })(
                                        <Select dropdownClassName="itemdropdown">
                                            {
                                                this.state.farmList.map((item : any) => {
                                                    return <Option value={item.id}>{item.operator.name}</Option>
                                                })
                                            }
                                        </Select>
                                    )
                                }
                            </Form.Item>
                           
                        </Col>

                            : null
                        }




                </Row>
                <Row>
                    <Col>
                    <Button type="primary" htmlType="submit">
                        Generate
                    </Button>
          </Col>
                </Row>
                </Form>
                    </Card>
                    <Card>
                        <div dangerouslySetInnerHTML={{ __html : this.state.reportResponse }}>

                        </div>
                    </Card>
            </div>
        )
    }
}