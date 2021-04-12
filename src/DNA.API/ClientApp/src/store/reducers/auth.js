import * as actionTypes from '../actions/actionTypes';
import menus from '../../constants/menus';

const updateObject = (oldObject, updatedProperties) => {
	return {
		...oldObject,
		...updatedProperties
	};
};

const initialState = {
	token: null,
	user: null,
	error: null,
	loading: false,
	authRedirectPath: '/auth/login',
	registered: {},
	menus: menus,
	defaultMenus: menus,
	settings: {
		configs: null,
		screenConfig: null,
		currentSettings: null,
	}
	/*{
			admin: {},
			home: {},
			user: {},
			panel: {},
			contact: {},
			main: {},
			social: {},
			links: {},
	}*/
};

const reset = (state) => {
	return { ...state, error: null, loading: false, reset: true }
};
const authStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const authSuccess = (state, action) => {
	return updateObject(state, {
		token: action.token,
		user: action.user,
		error: null,
		loading: false
	});
};

const authFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false,
		token: null,
		user: null,
		menus: state.defaultMenus
	});
};

const authLogout = (state, action) => {
	return updateObject(state, {
		token: null,
		user: null,
		menus: menus,
		defaultMenus: menus
	});
};

const setAuthRedirectPath = (state, action) => {
	//if (state.authRedirectPath !== action.path)
	return updateObject(state, { authRedirectPath: action.path })
	//return state
}

/********************** */
const registerStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const registerSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false,
		registered: action.registered
	});
};

const registerFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false
	});
};
/* CONFIRM ********************* */
const confirmStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const confirmSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false
	});
};

const confirmFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false
	});
};
/* RECOVERY ********************* */
const recoveryStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const recoverySuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false
	});
};

const recoveryFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false
	});
};
/* changePassword ********************* */
const changePasswordStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const changePasswordSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false
	});
};

const changePasswordFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false
	});
};

/* init ********************* */
const initStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};

const initSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false,
		menus: action.menus,
		defaultMenus: action.defaultMenus
	});
};

const initFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false,
		menus: state.defaultMenus
	});
};
/* get settings ********************* */
const getSettingsFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false,
		settings: {}
	});
};
const getSettingsStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};
const getSettingsSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false,
		settings: { ...state.settings, ...action.settings }
	});
};
/* save settings ********************* */
const saveSettingsFail = (state, action) => {
	return updateObject(state, {
		error: action.error,
		loading: false
	});
};
const saveSettingsStart = (state, action) => {
	return updateObject(state, { error: null, loading: true });
};
const setCurrentConfig = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false,
		settings: {
			...state.settings,
			currentSettings: state.settings.configs.values,
		}
	});
};
const saveSettingsSuccess = (state, action) => {
	return updateObject(state, {
		error: null,
		loading: false,
		settings: { screenConfig: state.settings.screenConfig, configs: action.settings }
	});
};
const reducer = (state = initialState, action) => {
	switch (action.type) {
		case actionTypes.RESET: return reset(state, action);

		case actionTypes.SETTINGS_INIT_START: return initStart(state) // { ...state, loading: true, error: null };
		case actionTypes.SETTINGS_INIT_SUCCESS: return initSuccess(state, action) // { ...state, menus: action.menus || initialState.menus, loading: false, error: null };
		case actionTypes.SETTINGS_INIT_FAIL: return initFail(state, action) //{ ...state, menus: {}, loading: false, error: action.message };

		case actionTypes.AUTH_GET_SETTINGS_START: return getSettingsStart(state, action);
		case actionTypes.AUTH_GET_SETTINGS_SUCCESS: return getSettingsSuccess(state, action);
		case actionTypes.AUTH_GET_SETTINGS_FAIL: return getSettingsFail(state, action);
		case actionTypes.AUTH_SAVE_SETTINGS_START: return saveSettingsStart(state, action);
		case actionTypes.AUTH_SAVE_SETTINGS_SUCCESS: return saveSettingsSuccess(state, action);
		case actionTypes.AUTH_SAVE_SETTINGS_FAIL: return saveSettingsFail(state, action);
		case actionTypes.AUTH_SET_CURRENT_CONFIG: return setCurrentConfig(state, action);

		case actionTypes.AUTH_START: return authStart(state, action);
		case actionTypes.AUTH_SUCCESS: return authSuccess(state, action);
		case actionTypes.AUTH_FAIL: return authFail(state, action);
		case actionTypes.AUTH_LOGOUT: return authLogout(state, action);
		case actionTypes.SET_AUTH_REDIRECT_PATH: return setAuthRedirectPath(state, action);

		case actionTypes.AUTH_REGISTER_START: return registerStart(state, action);
		case actionTypes.AUTH_REGISTER_SUCCESS: return registerSuccess(state, action);
		case actionTypes.AUTH_REGISTER_FAIL: return registerFail(state, action);

		case actionTypes.AUTH_CONFIRM_START: return confirmStart(state, action);
		case actionTypes.AUTH_CONFIRM_SUCCESS: return confirmSuccess(state, action);
		case actionTypes.AUTH_CONFIRM_FAIL: return confirmFail(state, action);

		case actionTypes.AUTH_RECOVERY_START: return recoveryStart(state, action);
		case actionTypes.AUTH_RECOVERY_SUCCESS: return recoverySuccess(state, action);
		case actionTypes.AUTH_RECOVERY_FAIL: return recoveryFail(state, action);

		case actionTypes.AUTH_CHANGE_PASSWORD_START: return changePasswordStart(state, action);
		case actionTypes.AUTH_CHANGE_PASSWORD_SUCCESS: return changePasswordSuccess(state, action);
		case actionTypes.AUTH_CHANGE_PASSWORD_FAIL: return changePasswordFail(state, action);

		default:
			return state;
	}
};

export default reducer;