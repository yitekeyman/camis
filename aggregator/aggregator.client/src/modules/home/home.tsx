import React from 'react';
import { Report } from '../report/report';


export class Home extends React.Component<any,any>{

    render(){
        return(
            <div>
                <Report {...this.props}/>
            </div>
        )
    }
}