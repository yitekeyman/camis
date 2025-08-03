
import { baseUrl } from './urlConfig';
import Axios, { AxiosAdapter, AxiosInstance } from 'axios';
import { createBrowserHistory } from 'history';
import { configureStore } from '../store/configureStore';
import Swal from 'sweetalert2';
import { Api } from './api';
import { LoginPar, RegisterViewModel, UserViewModel, RegionConfigModel } from './pars/api.pars';
const base = document.getElementsByTagName('base')[0].baseURI;
const history = createBrowserHistory();




export default class ReportService {

    private Props : any;
    private api : Api;
    constructor(props  : any){
        this.Props = props;
        this.api = new Api(props,"Report");

    }

    GetAllRegion(){
        var path = `/GetAllRegions`;
        return this.api.get<any[]>(path);
    }

    GetZones(regionid : string){
        var path = `/GetZones?regionid=${regionid}`;
        return this.api.get<any[]>(path);
    }

    GetWoredas(zoneid : string){
        var path =  `/GetWoredas?zoneid=${zoneid}`;
        return this.api.get<any[]>(path);
    }


    SetRegionUrl(regionid : string, url : string, username : string, password : string){
        var data = { "regionid" : regionid , url : url, username, password};
        console.log(data);
        var path = `/SetRegionUrl`;
        return this.api.post(path,data);
    }

    UpdateRegionConfig(data : RegionConfigModel){
        var path = `/UpdateRegionConfig`;
        return this.api.post(path,data);
    }

    GetReport(data : any){
        var path =  `/GetReport`;
        return this.api.post(path,data);
    }

    GetFarms(region : string){
        var path = `/GetFarms?region=${region}`;
        return this.api.get(path);
    }

    
}
