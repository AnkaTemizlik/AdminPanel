import axios from '../axios';
import api from '../api';
import * as actionTypes from './actionTypes';

export const reset = () => {
    return dispatch => {
        dispatch(actionTypes.RESET)
    }
}

/* GET SETTINGS ************************* */
export const getEntitiesStart = () => {
    return { type: actionTypes.GET_ENTITIES_START };
};
export const getEntitiesSuccess = (status) => {
    return {
        type: actionTypes.GET_ENTITIES_SUCCESS,
        settings: status.resource
    };
};
export const getEntitiesFail = (error) => {
    return {
        type: actionTypes.GET_ENTITIES_FAIL,
        error: error
    };
};

export const getEntities = (data) => {
    return dispatch => {
        dispatch(getEntitiesStart());
        return api.auth.getEntities(data)
            .then((status) => {
                status.Success
                    ? dispatch(getEntitiesSuccess(status))
                    : dispatch(getEntitiesFail(status.Message))
                return Promise.resolve(status)
            })
    }
}


