import axios from '../axios';
import api from '../api';
import * as actionTypes from './actionTypes';
import find from 'lodash/find'

export const reset = () => {
	return dispatch => {
		dispatch(actionTypes.RESET)
	}
}

/* LOGIN ************************* */
export const authStart = () => {
	return { type: actionTypes.AUTH_START, error: null };
};

export const authSuccess = (token, user) => {
	return {
		type: actionTypes.AUTH_SUCCESS,
		token: token,
		user: user
	};
};

export const authFail = (error) => {
	return {
		type: actionTypes.AUTH_FAIL,
		error: error
	};
};

export const logout = () => {
	return dispatch => {
		localStorage.removeItem('token');
		localStorage.removeItem('expirationDate');
		localStorage.removeItem('user');
		localStorage.removeItem('menus');
		localStorage.removeItem('defaultMenus');
		return dispatch({ type: actionTypes.AUTH_LOGOUT })
	}
};

export const checkAuthTimeout = (expirationTime) => {
	return dispatch => {
		// setTimeout(() => {
		//     dispatch(init())
		// }, expirationTime * 1000);
		setTimeout(() => {
			window.location.href = "/auth/login?redirect=" + window.location.pathname
		}, (expirationTime * 1000) + 2000);
	};
};

export const auth2 = (user, token, expirationDate) => {
	return dispatch => {
		localStorage.setItem('token', token);
		localStorage.setItem('expirationDate', expirationDate);
		localStorage.setItem('user', JSON.stringify(user));
		dispatch(authSuccess(token, user));
	}
}

export const auth = (email, password, key) => {
	return dispatch => {
		dispatch(authStart());

		const authData = {
			username: email,
			password: password,
			key: key
		};

		delete axios.defaults.headers.common["Authorization"]

		return api.auth.login(authData)
			// axios.post(url, authData)
			.then(status => {
				if (status.Success) {
					let token = null;
					if (status.Resource.IsInitialPassword == false) {
						const expirationDate = new Date(new Date().getTime() + status.Resource.ExpiresIn * 1000);
						token = status.Resource.Token;
						delete status.Resource.Token;

						localStorage.setItem('token', token);
						localStorage.setItem('expirationDate', expirationDate);
						localStorage.setItem('user', JSON.stringify(status.Resource));

						axios.defaults.headers.common["Authorization"] = "Bearer " + token
					}
					dispatch(authSuccess(token, status.Resource));
					dispatch(checkAuthTimeout(status.Resource.ExpiresIn));

				}
				else {
					dispatch(authFail(status.Message));
				}
				return Promise.resolve(status)
			})
		//.then(() => dispatch(init()))
	};
};

export const setAuthRedirectPath = (path) => {
	return {
		type: actionTypes.SET_AUTH_REDIRECT_PATH,
		path: path
	};
};

export const authCheckState = () => {
	return dispatch => {
		const token = localStorage.getItem('token');
		if (!token) {
			delete axios.defaults.headers.common["Authorization"]
			dispatch(logout())
			return Promise.resolve({ Success: false })
		} else {
			const expirationDate = new Date(localStorage.getItem('expirationDate'));
			if (expirationDate <= new Date()) {
				delete axios.defaults.headers.common["Authorization"]
				dispatch(logout())
				return Promise.resolve({ Success: false })
			} else {
				const user = JSON.parse(localStorage.getItem('user'));
				axios.defaults.headers.common["Authorization"] = "Bearer " + token
				dispatch(authSuccess(token, user));
				dispatch(checkAuthTimeout((expirationDate.getTime() - new Date().getTime()) / 1000));
				return Promise.resolve({ Success: true })
			}
		}
	};
};

/* INIT ************************* */
export const initStart = () => {
	return { type: actionTypes.SETTINGS_INIT_START, error: null };
};
export const initSuccess = (menus) => {
	return {
		type: actionTypes.SETTINGS_INIT_SUCCESS,
		menus: menus
	};
};
export const initFail = (error) => {
	return {
		type: actionTypes.SETTINGS_INIT_FAIL,
		error: error
	};
};
export const tryInit = () => {
	return dispatch => {
		if (localStorage.getItem('menus') != null) {
			var menus = JSON.parse(localStorage.getItem('menus'))
			dispatch(initSuccess(menus))
		}
		else {
			if (localStorage.getItem("token") == null) {
				if (localStorage.getItem("defaultMenus") != null)
					dispatch(initSuccess(JSON.parse(localStorage.getItem('defaultMenus'))))
				else
					dispatch(init())
			}
			else {
				dispatch(init())
			}
		}
	}
};
export const init = () => {
	return dispatch => {
		dispatch(initStart());
		return api.auth.init("W9OWXIXZMZ")
			.then((status) => {
				if (status.Success) {
					let menus = status.Resource
					let x = {
						admin: find(menus, ["name", "Admin"]),
						home: find(menus, ["name", "Home"]),
						user: find(menus, ["name", "User"]),
						panel: find(menus, ["name", "Panel"]),
						contact: find(menus, ["name", "Contact"]),
						main: find(menus, ["name", "Main"])
					}
					x.social = find(x.contact.menus, ["name", "Social"])
					x.links = find(x.contact.menus, ["name", "Links"])

					localStorage.setItem('menus', JSON.stringify(x));
					if (localStorage.getItem("token") == null) {
						localStorage.setItem("defaultMenus", JSON.stringify(x))
					}

					dispatch(initSuccess(x))
				}
				else
					dispatch(initFail(status.Message))
				return Promise.resolve(status)
			})

	}
};

/* REGISTER ************************* */
export const registerStart = () => {
	return { type: actionTypes.AUTH_REGISTER_START, error: null };
};

export const registerSuccess = (status) => {
	return {
		type: actionTypes.AUTH_REGISTER_SUCCESS,
		registered: status.Resource
	};
};

export const registerFail = (error) => {
	return {
		type: actionTypes.AUTH_REGISTER_FAIL,
		error: error,
		registered: null
	};
};

export const register = (data) => {
	return dispatch => {
		dispatch(registerStart());

		return api.auth.register({ ...data })
			.then((status) => {
				status.Success
					? dispatch(registerSuccess(status))
					: dispatch(registerFail(status.Message))
				return Promise.resolve(status)
			})
	}
}

/* CONFIRM ************************* */
export const confirmStart = () => {
	return { type: actionTypes.AUTH_CONFIRM_START, error: null };
};

export const confirmSuccess = () => {
	return { type: actionTypes.AUTH_CONFIRM_SUCCESS };
};

export const confirmFail = (error) => {
	return {
		type: actionTypes.AUTH_CONFIRM_FAIL,
		error: error
	};
};

export const confirm = (data) => {
	return dispatch => {
		dispatch(confirmStart());
		return api.auth.confirm(data)
			.then((status) => {
				status.Success
					? dispatch(confirmSuccess(status))
					: dispatch(confirmFail(status.Message))
				return Promise.resolve(status)
			})
	}
}

/* RECOVERY ************************* */
export const recoveryStart = () => {
	return { type: actionTypes.AUTH_RECOVERY_START };
};

export const recoverySuccess = () => {
	return {
		type: actionTypes.AUTH_RECOVERY_SUCCESS
	};
};

export const recoveryFail = (error) => {
	return {
		type: actionTypes.AUTH_RECOVERY_FAIL,
		error: error
	};
};

export const recovery = (data) => {
	return dispatch => {
		dispatch(recoveryStart());
		return api.auth.recovery(data)
			.then((status) => {
				status.Success
					? dispatch(recoverySuccess(status))
					: dispatch(recoveryFail(status.Message))
				return Promise.resolve(status)
			})
	}
}

/* CHANGE PASSWORD ************************* */
export const changePasswordStart = () => {
	return { type: actionTypes.AUTH_CHANGE_PASSWORD_START, user: null, passwordConfirmationCode: null };
};

export const changePasswordSuccess = () => {
	return {
		type: actionTypes.AUTH_CHANGE_PASSWORD_SUCCESS
	};
};

export const changePasswordFail = (error) => {
	return {
		type: actionTypes.AUTH_CHANGE_PASSWORD_FAIL,
		error: error
	};
};

export const changePassword = (data) => {
	return dispatch => {
		dispatch(changePasswordStart());
		return api.auth.changePassword(data)
			.then((status) => {
				status.Success
					? dispatch(changePasswordSuccess(status))
					: dispatch(changePasswordFail(status.Message))
				return Promise.resolve(status)
			})
	}
}


/* SETTINGS ************************* */
export const getSettingsStart = () => {
	return { type: actionTypes.AUTH_GET_SETTINGS_START };
};
export const getSettingsSuccess = (status) => {
	return {
		type: actionTypes.AUTH_GET_SETTINGS_SUCCESS,
		settings: status.Resource
	};
};
export const getSettingsFail = (error) => {
	return {
		type: actionTypes.AUTH_GET_SETTINGS_FAIL,
		error: error
	};
};
export const saveSettingsStart = () => {
	return { type: actionTypes.AUTH_SAVE_SETTINGS_START };
};
export const saveSettingsSuccess = (status) => {
	return {
		type: actionTypes.AUTH_SAVE_SETTINGS_SUCCESS,
		settings: status.Resource
	};
};
export const saveSettingsFail = (error) => {
	return {
		type: actionTypes.AUTH_SAVE_SETTINGS_FAIL,
		error: error
	};
};

export const saveSettings = (id, data) => {
	return dispatch => {
		dispatch(saveSettingsStart());
		return api.auth.saveSettings(id, data)
			.then((status) => {
				status.Success
					? dispatch(saveSettingsSuccess(status))
					: dispatch(saveSettingsFail(status.Message))
				return Promise.resolve(status)
			})
	}
}

export const setCurrentConfig = (id) => {
	return {
		type: actionTypes.AUTH_SET_CURRENT_CONFIG,
	};
};
