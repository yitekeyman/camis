import { Reducer } from 'redux';
import {LoadActions} from '../actions/LoadActions';
import  LoadState from '../../_infrastructure/state/LoadState';

let loadInitialState : LoadState  = {
    loading : false
}

const loadReducer : Reducer<LoadState> = (
    state = loadInitialState,
    action
) : LoadState => {
    state = state || loadInitialState;

    if (action.type == LoadActions.StartLoad) {
        console.log("Starting");
        return {...state , loading : true };
    }
    if (action.type == LoadActions.StopLoad) {
        console.log("Stopping");
        return {...state, loading : false };
    }

    return state;
}

export default loadReducer;