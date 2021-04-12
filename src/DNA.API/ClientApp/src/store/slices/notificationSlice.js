import { createSlice, createAsyncThunk, combineReducers } from '@reduxjs/toolkit';
import api from '../api'
import { showMessage } from './alertsSlice'

export const getNotifications = createAsyncThunk(
	'notification/getNotifications',
	async (params, { dispatch, getState }) => {
		const status = await api.actions.getNotifications('Take=10&Filter=["IsRead","=",false]&Sort=[{"selector":"UpdateTime","desc":true}]')
		if (status.Success) {
			dispatch(set(status.Resource))
		}
		else
			dispatch(showMessage(status))
		return status
	}
)

export const markAsReadOrUnread = createAsyncThunk(
	'notification/markAsReadOrUnread',
	async (params, { dispatch, getState }) => {
		const status = await api.actions.run("PUT", 'api/notification/mark-as-read/' + params.id)
		if (status.Success) {
			dispatch(mark({ success: status.Resource, id: params.id }))
		}
		else
			dispatch(showMessage(status))
		return status
	}
)

const initialState = {
	data: [],
	total: 0,
	error: null
};

const notificationSlice = createSlice({
	name: 'alerts',
	initialState,
	reducers: {
		set: (state, { payload }) => {
			state.data = payload.Items
			state.total = payload.TotalItems
		},
		mark: (state, { payload }) => {
			if (payload.success)
				state.data = state.data.filter(n => n.Id != payload.id)
			state.total = state.total - 1
		}
	}
})

export const { set, mark } = notificationSlice.actions;

export default notificationSlice.reducer;