import React, { Component } from 'react';
import { Timeline, Card, Table, Row, Col } from 'antd';
import './land_profile.scss';
import LandService from '../../../_setup/services/land.service';
import { BaseType } from '../../../_infrastructure/model/accessibility_type';
import months from '../../../assets/data/months.json';
import Map from '../land_map/land_map';

function filterMonth(id : number){
    var filtered = months.filter((x) => x.id == id );
    return filtered[0].name;
}

function filterType(props : any, dict : string, values : number[]){
    console.log(props);
    console.log(dict);
    console.log(values);
    var filtered : BaseType[] = props[dict];
    var f = filtered.filter((x) => {
        return values.indexOf(x.id) != -1;
    });
    var resp= f.map( el => el.name );
    return resp;
}

function filterType2(props : any, dict : string, values : number){
    console.log(props);
    console.log(dict);
    console.log(values);
    var filtered : BaseType[] = props[dict];
    var f = filtered.filter((x) => {
        return values == x.id;
    });
    return f[0].name;
}





export class Profile extends Component<any,any>{

    landService : LandService;
    mapRef : React.RefObject<any>
    constructor(props : any){
        super(props);
        this.state = {
            land : undefined
        }
        this.landService = new LandService(this.props);
        this.mapRef = React.createRef();
    }

    componentDidMount(){
        var self = this;
        console.log(this.props);
        var id = this.props.match.params.id;
        this.landService.GetLand(id).then((response) => {
            self.setState({ land : response.data },() => {
                
                var g = this.state.land.parcels[this.state.land.upins[0]];
                if (g) {
                  var parts = g.geometry.split(";");
                  this.mapRef.current.setWorkFlowGeomByWKT(parts[parts.length-1]);
                }
          
            });
        })
        console.log(this.mapRef);

    }

    render(){
    
        const climateTableColumn = [{
            title : "Month",
            render : (d : any) => filterMonth(d.month)
        }, { title : "Percipitation (cm)", render : (d : any) => `${d.precipitation} cm`},
            { title : "Mean Minimum Temperature (°C)", render : (d : any) => `${d.temp_low} °C`},
            { title : "Mean Maximum Temperature (°C)", render : (d : any) => `${d.temp_high} °C`},
            { title : "Average Temperature (°C)", render : (d : any) => `${d.temp_avg} °C`}]
        return(
            <Row>
                <Col span={16}>
                <Card
            title="Land Profile"
            >
                {
                    this.state.land != undefined ?
                    <Timeline>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Upin</h3>
                        <ul>
                            <li>{this.state.land.upins.join(' ')}</li>
                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                    <div className="profileProp">
                        <h3>Land Holder</h3>
                        <ul>
                            <li>{this.state.land.holdership}</li>
                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Area</h3>
                        <ul>
                            <li>{(this.state.land.area/10000).toFixed(2)} ha</li>
                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Location</h3>
                        <ul>
                            <li>{this.state.land.centroidX}*E  -  {this.state.land.centroidY}*N</li>
                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Accessibile By</h3>
                        <ul>
                            {
                                filterType(this.props.baseData,"accessibiltyType", this.state.land.accessablity).map((item) => {
                                    return <li>{item}</li>
                                })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Agro-Echology Zone</h3>
                        <ul>
                            {

                              this.state.land.agroEchologyZone.map( (item : any) => {
                                  return <li>{filterType2(this.props.baseData,"agroType",item.agroType)} - {item.result}</li>
                              })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Land Suitable For</h3>
                        <ul>
                            {
                                filterType(this.props.baseData,"investmentType", this.state.land.investmentType).map((item) => {
                                    return <li>{item}</li>
                                })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Moisture Source</h3>
                        
                            {

                                    <ul>
                                  <li>{filterType2(this.props.baseData,"moistureSource",this.state.land.moistureSource)}</li>
                                  </ul>                     
                            }
                            {
                                this.state.land.moistureSource == 2 ?
                                <div>
                                    <h4>Generalized Water Sampling Parameters</h4>
                                    <ul>
                                    {

                                        this.state.land.irrigationValues.waterSourceParameter.map( (item : any) => {
                                            return <li>{filterType2(this.props.baseData,"waterSourceType",item.waterSourceType)} - {item.result}</li>
                                        })
                                        }
                                    </ul>    

                                    <h4>Generalized Water Sampling Parameters</h4>
                                    <ul>
                                    {

                                        this.state.land.irrigationValues.surfaceWater.map( (item : any) => {
                                            return <li>{filterType2(this.props.baseData,"surfaceWaterType",item.surfaceWaterType)} - {item.result}</li>
                                        })
                                        }
                                    </ul>    
                                    
                                    <h3>Ground Water</h3>
                        <ul>
                            {
                                filterType(this.props.baseData,"groundWaterType", this.state.land.irrigationValues.groundWater).map((item) => {
                                    return <li>{item}</li>
                                })
                            }

                        </ul>
                                    </div>
                                : null
                            }

            
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Topography</h3>
                        <ul>
                            {

                              this.state.land.topography.map( (item : any) => {
                                  return <li>{filterType2(this.props.baseData,"topographyType",item.topographyType)} - {item.result}</li>
                              })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Soil Chemical and Physical Properties</h3>
                        <ul>
                            {

                              this.state.land.soilTests.map( (item : any) => {
                                  return <li>{filterType2(this.props.baseData,"soilTestType",item.testType)} - {item.result}</li>
                              })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Existing Land Use</h3>
                        <ul>
                            {
                                filterType(this.props.baseData,"usageType", this.state.land.existLandUse).map((item) => {
                                    return <li>{item}</li>
                                })
                            }

                        </ul>
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Climatology</h3>
                        <Table
                        dataSource={this.state.land.climate}
                        columns={climateTableColumn}
                        pagination={false} />
                        </div>
                    </Timeline.Item>
                    <Timeline.Item >
                        <div className="profileProp">
                        <h3>Description</h3>
                        <ul>
                            <li>{this.state.land.description}</li>
                        </ul>
                        </div>
                    </Timeline.Item>
                </Timeline>         

                    : null
                }
                  
            </Card>

                </Col>
                
                <Col span={8}>
                    <Card 
                    title="Map">
                    <Map ref={this.mapRef} {...this.props} />
                    </Card>
                </Col>
         
            </Row>
 
        )
    }
}