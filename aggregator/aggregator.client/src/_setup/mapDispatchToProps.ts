import { bindActionCreators } from 'redux';
import { AuthActionCreators } from '../_setup/actions/AuthActions';
import { baseDataActionCreators } from '../_setup/actions/BaseDataActions';
import { LoadActionCreators } from '../_setup/actions/LoadActions';


export function mapDispatchToProps(dispatch : any){
    return {
        auth : bindActionCreators(AuthActionCreators,dispatch),
        loader : bindActionCreators(LoadActionCreators, dispatch),
        base : bindActionCreators(baseDataActionCreators,dispatch)
    }
  }