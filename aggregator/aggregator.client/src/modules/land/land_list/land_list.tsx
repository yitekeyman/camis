import React from 'react';
import LandService from '../../../_setup/services/land.service';
import { Card, Table, Button, Modal, Row, Col } from 'antd';
import { Link } from 'react-router-dom';
import './land_list.scss';
import LandSynchronize from '../land_synchronize/land_synchronize';
import Map from '../land_map/land_map';


interface IState{
    landList : any[],
    showSynchronizeModal : boolean
}

export class LandList extends React.Component<any,IState>{

    private landService : LandService;
    constructor(props : any){
        super(props);
        this.state = {
            landList : [],
            showSynchronizeModal : false,
        }
        this.landService = new LandService(props);
        this.toogleSynchronizeModal = this.toogleSynchronizeModal.bind(this);
        this.getLandList = this.getLandList.bind(this);
    }

    componentDidMount(){
        this.getLandList();
    }

    getLandList(){
        var self = this;
        this.landService.GetLandList().then((response) => {
            self.setState({ landList : response.data },() => {
                console.log(self.state)
            });
        })
    }

    toogleSynchronizeModal(){
        this.setState({ showSynchronizeModal : !this.state.showSynchronizeModal });
    }

    render(){
        var landType={ 1 : 'Prepared', 2 : 'Transferred '}
        var column = [
            { dataIndex : 'upin', title : "Upin"},
            { title : 'Area (ha)', render : (d  : any) => (d.area/10000).toFixed(2) },
            { title : 'Description', dataIndex : 'land.description'},
            { title : 'Type', render : (d : any) => d.land.landType == 1 ? "Prepared" : "Transferred" },
            { title : "Action", render : (d : any) => <Link to={`/land/profile/${d.landId}`}><Button icon="eye" /></Link> }
        ]
        return(
            <Row>
                <Col span={15}>
                <Card 
           title="Land List"
           extra={<Button icon="sync" onClick={this.toogleSynchronizeModal}>Synchronize Land</Button>}
           >
               <Table
                dataSource={this.state.landList}
                columns={column}
               />
               <Modal
               visible={this.state.showSynchronizeModal}
               footer={[]}
               onCancel={this.toogleSynchronizeModal}
               >
                <LandSynchronize {...this.props} close={this.toogleSynchronizeModal} updateList={this.getLandList}/>
               </Modal>
           </Card>
                </Col>
                <Col span={9}>
                    <Card
                    title="Map">
                    <Map {...this.props} />
                    </Card>

                </Col>

            </Row>
          
        )
    }
}