
import { baseUrl } from './urlConfig';
import Axios, { AxiosAdapter, AxiosInstance } from 'axios';
import { createBrowserHistory } from 'history';
import { configureStore } from '../store/configureStore';
import Swal from 'sweetalert2';
import { Api } from './api';
import { LoginPar, RegisterViewModel, UserViewModel, RegionConfigModel } from './pars/api.pars';
import { BaseData } from '../../_infrastructure/state/baseDataState';
const base = document.getElementsByTagName('base')[0].baseURI;
const history = createBrowserHistory();




export default class LandService {

    private Props : any;
    private api : Api;
    constructor(props  : any){
        this.Props = props;
        this.api = new Api(props,"Land");

    }

        
    GetLandList(){
        var path = `/GetLandList`;
        return this.api.get<any[]>(path);

    }

    GetLandListOfRegion(region : string){
        var path = `/GetLandListOfRegion?region=${region}`;
        return this.api.get<any[]>(path);
    }

    GetInitData(){
        var path = `/GetInitData`;
        return this.api.get<BaseData>(path);
    }

    GetLand(id : string){
        var path = `/GetLand?id=${id}`;
        return this.api.get<any>(path);
    }

    SynchronizeLand(id : any){
        var path = `/SynchronizeLand`;
        console.log(id);
        return this.api.post<string>(path,id);
    }
}
