
import { baseUrl } from './urlConfig';
import Axios, { AxiosAdapter, AxiosInstance } from 'axios';
import { createBrowserHistory } from 'history';
import { configureStore } from '../store/configureStore';
import Swal from 'sweetalert2';
import { Api } from './api';
import { LoginPar, RegisterViewModel, UserViewModel, ResetPasswordViewModel } from './pars/api.pars';
const base = document.getElementsByTagName('base')[0].baseURI;
const history = createBrowserHistory();




export default class SecurityService {

    private Props : any;
    private api : Api;
    constructor(props  : any){
        this.Props = props;
        this.api = new Api(props,"Auth");

    }

    Logout(sessionID : string){
        var path = "/Logout";
        return this.api.post(path,sessionID);
    }

    
    Login(data: LoginPar){
        var path = "/Login";
        return this.api.post(path,data);
    }

    GetUsers(){
        var path = `/GetUsers`;
        return this.api.get<any[]>(path);
    }

    Register(data : RegisterViewModel){
        var path = `/Register`;
        return this.api.post(path,data);
    }

    Update(data : UserViewModel){
        var path = `/Update`;
        return this.api.post(path,data);
    }

    ResetPassword(data: ResetPasswordViewModel){
        var path = `/ResetPassword`;
        return this.api.post(path,data);
    }

    
}
