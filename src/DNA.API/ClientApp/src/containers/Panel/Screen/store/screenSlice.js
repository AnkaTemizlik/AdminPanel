import { createSlice, createAsyncThunk, createSelector } from '@reduxjs/toolkit';
import api from '../../../../store/api'
import { showMessage } from '../../../../store/slices/alertsSlice'

export const selectData = createSelector(
	[(state) => state.panel.screen.data], (data) => data
)

export const selectScreen = createSelector(
	[(state) => state.panel.screen], (screen) => screen
)

export const getById = createAsyncThunk(
	'panel/screen/getById',
	async ({ name, id }, { dispatch, getState }) => {
		let status = await api.entity.getById(name, id)
		if (!status.Success)
			dispatch(showMessage(status.Message))
		return status;
	}
)

export const setCurrentScreen = createAsyncThunk(
	'panel/screen/setCurrentScreen',
	async (name, { dispatch, getState }) => {
		let query = { ...initialQuery, name }
		let screenConfig = getState().screenConfig
		let screen = screenConfig.screens[query.name]
		dispatch(applyCurrentScreen({ screen: { ...screen }, name: query.name }))
		return await screen
	}
)

let initialData = {
	rows: [],
	total: 0
}

var initialQuery = {
	page: 0,
	size: 10,
	search: "",
	filter: null,
	name: null,
	conditions: []
}

var initialState = {
	currentScreen: null,
	name: null,
	row: null,
	data: initialData,
	loading: false,
	query: initialQuery
}

const screenSlice = createSlice({
	name: 'panel/screen',
	initialState: { ...initialState },
	reducers: {
		reset: () => initialState,
		applyCurrentScreen: (state, { payload }) => {
			const { screen, name } = payload;
			state.currentScreen = screen
			state.name = name
			state.row = null
			state.data.rows = []
			state.data.total = 0
			state.query.name = name
			state.loading = false
		},
		setQuery: (state, { payload }) => {
			state.query = payload
		},
		setRow: (state, { payload }) => { state.row = payload },
		setLoading: (state, { payload }) => { state.loading = payload },
	},
	extraReducers: {
		[getById.fulfilled]: (state, { payload }) => {
			if (payload)
				state.row = payload.Success ? payload.Resource : null
		}
	}
});

export const {
	reset,
	applyCurrentScreen,
	setData,
	setRow,
	setQuery,
	setLoading
} = screenSlice.actions;

export default screenSlice.reducer;
