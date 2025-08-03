
import { baseUrl } from './urlConfig';
import Axios, { AxiosAdapter, AxiosInstance } from 'axios';
import { createBrowserHistory } from 'history';
import { configureStore } from '../store/configureStore';
import Swal from 'sweetalert2';
import { LoginPar, RegisterViewModel } from './pars/api.pars';
const base = document.getElementsByTagName('base')[0].baseURI;
const history = createBrowserHistory();




export class Api {

    private axios : AxiosInstance;
    private Props : any;
    constructor(props  : any,controller : string){
        this.Props = props;
        this.axios = Axios.create({
            baseURL : baseUrl + `/${controller}`,
            withCredentials : true,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
               'Access-Control-Allow-Headers': '*',
               "Access-Control-Allow-Origin": "*",
               'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
            }
        })

        this.axios.interceptors.response.use((response) => {
            console.log(response);
            return response;}, (error) => {
             console.log(error);
             Swal.close();
             if(error.message == "Network Error"){
                Swal.fire("Error","Server Is Down !!!","error");
                this.Props.auth.logout();
                console.log(this.Props);
                this.Props.history.push("/login");
            }

            else if(error.response || (error.response && error.response.data == "Access Denied")){
                console.log(error.response.status);
                if(error.response.status == 403 || error.response.status == 405){
                    Swal.fire("Error",error.response.data,"error");
                    this.Props.auth.logout();
                    console.log(this.Props);
                    this.Props.history.push("/login");
                }
                else{
                    Swal.fire("Error",error.response.data,"error");
                }
            }

            return Promise.reject(error);
            
        })
    }


    get<T>(path : string){
        return this.axios.get<T>(path);
    }

    post<T>(path : string ,data : any){
        return this.axios.post<T>(path,data);
    }
    


    
}