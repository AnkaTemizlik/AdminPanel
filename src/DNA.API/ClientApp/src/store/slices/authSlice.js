import { createSlice, createAsyncThunk, createSelector, current } from '@reduxjs/toolkit'
import axios from 'axios'
import api from '../api';
import _ from 'lodash'

export const login = createAsyncThunk(
	'auth/login',
	async (params, { dispatch, getState }) => {
		const authData = {
			// username: params.email,
			// password: params.password,
			// key: params.key
			...params
		};
		delete axios.defaults.headers.common["Authorization"]
		let status = await api.auth.login(authData);
		if (status.Success) {
			let token = null;
			const expirationDate = new Date(new Date().getTime() + status.Resource.ExpiresIn * 1000);
			if (status.Resource.IsInitialPassword == false) {
				token = status.Resource.Token;
				delete status.Resource.Token;
				localStorage.setItem('token', token);
				localStorage.setItem('expirationDate', expirationDate);
				localStorage.setItem('user', JSON.stringify(status.Resource));
				axios.defaults.headers.common["Authorization"] = "Bearer " + token
			}
			dispatch(authSuccess({ token, user: status.Resource, expirationDate }));
			dispatch(checkAuthTimeout({ expirationTime: status.Resource.ExpiresIn }));
		}
		else {
			dispatch(logout());
			dispatch(authFail({ status }));
		}
	}
)

export const authCheckState = createAsyncThunk(
	'auth/authCheckState',
	async (params, { dispatch, getState }) => {
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
				dispatch(authSuccess({ token, user, expirationDate }));
				let expirationTime = (expirationDate.getTime() - new Date().getTime()) / 1000
				dispatch(checkAuthTimeout({ expirationTime }));
				return Promise.resolve({ Success: true })
			}
		}
	}
)

const authSlice = createSlice({
	name: 'menus',
	initialState: {
		token: null,
		isAuthenticated: false,
		user: {},
		error: null,
		expirationDate: null
	},
	reducers: {
		logout: (state, { payload }) => {
			localStorage.removeItem('token');
			localStorage.removeItem('expirationDate');
			localStorage.removeItem('user');
			localStorage.removeItem('menus');
			localStorage.removeItem('defaultMenus');
			state.isAuthenticated = false
			state.token = null
			state.user = {}
			state.expirationDate = null
		},
		authSuccess: (state, { payload }) => {
			state.token = payload.token
			state.user = payload.user
			state.isAuthenticated = payload.token != null
			state.expirationDate = payload.expirationDate
			state.error = null
			state.loading = false
		},
		authFail: (state, { payload }) => {
			var status = payload.status
			state.error = status.Message
		},
		checkAuthTimeout: (state, { payload }) => {
			setTimeout(() => {
				window.location.href = "/auth/login?redirect=" + window.location.pathname
			}, (payload.expirationTime * 1000) + 2000);
		},
		// authSuccess: (state, { payload }) => {

		// },
	}
})

export const {
	logout,
	authSuccess,
	authFail,
	checkAuthTimeout
} = authSlice.actions;

export default authSlice.reducer;
