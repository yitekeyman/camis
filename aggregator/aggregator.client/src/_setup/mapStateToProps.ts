import { RootState } from '../_setup/store/MyTypes';

export function mapStateToProps(state : RootState ){
    return {
        authenticated : state.auth.authenticated,
        session : state.auth.session,
        loading : state.loader.loading,
        baseData : state.base
    }
  }
  