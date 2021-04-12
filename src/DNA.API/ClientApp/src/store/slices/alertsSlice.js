import { createSlice, combineReducers } from '@reduxjs/toolkit';

const initialState = {
	message: null
};

const alertsSlice = createSlice({
	name: 'alerts',
	initialState,
	reducers: {
		showMessage: (state, action) => {
			state.message = action.payload
		},
		hideMessage: (state) => {
			state.message = null
		}
	}
})

export const {
	hideMessage,
	showMessage
} = alertsSlice.actions;

export default alertsSlice.reducer;