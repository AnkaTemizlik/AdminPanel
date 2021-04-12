import * as actionTypes from '../actions/actionTypes';

const updateObject = (oldObject, updatedProperties) => {
    return {
        ...oldObject,
        ...updatedProperties
    };
};

const initialState = {
    error: null,
    loading: false,
};

const reset = (state) => {
    return { ...state, error: null, loading: false, reset: true }
};

/* get settings ********************* */
const getEntitiesFail = (state, action) => {
    return updateObject(state, {
        error: action.error,
        loading: false,
        settings: {}
    });
};
const getEntitiesStart = (state, action) => {
    return updateObject(state, { error: null, loading: true });
};
const getEntitiesSuccess = (state, action) => {
    return updateObject(state, {
        error: null,
        loading: false,
        settings: action.settings
    });
};


const reducer = (state = initialState, action) => {
    switch (action.type) {
        case actionTypes.RESET: return reset(state, action);

        case actionTypes.GET_ENTITIES_START: return getEntitiesStart(state, action);
        case actionTypes.GET_ENTITIES_SUCCESS: return getEntitiesSuccess(state, action);
        case actionTypes.GET_ENTITIES_FAIL: return getEntitiesFail(state, action);
      

        default:
            return state;
    }
};

export default reducer;