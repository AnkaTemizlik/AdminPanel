import _ from 'lodash'
import { createSlice, createAsyncThunk, createSelector, current } from '@reduxjs/toolkit';
import { appendMenusFromScreens, appendMenusFromConfig, resetPanelMenu, addTokenToHangfireMenu } from './menuSlice'
import { applyAppSettings } from './appsettingsSlice'
import { showMessage } from './alertsSlice'

import api from '../api'
import i18next from 'i18next'
import CustomStore from 'devextreme/data/custom_store';
import { isNotEmpty, rolesAllowed, toQueryString } from '../utils';

export const getSettings = createAsyncThunk(
	'panel/settings/getSettings',
	async (params, { dispatch, getState }) => {
		const status = await api.auth.getSettings()
		console.info("getSettings", status)
		dispatch(setSettings(status))
		if (status.Success) {
			let resource = { ...status.Resource }
			dispatch(applyAppSettings(resource.configs))
			dispatch(resetPanelMenu())
			const state = getState();
			dispatch(addTokenToHangfireMenu({ token: state.auth.token }))
			dispatch(appendMenusFromScreens({
				screenConfig: resource.screenConfig,
				user: state.auth.user
			}))
			dispatch(appendMenusFromConfig({
				configs: resource.configs,
				user: state.auth.user
			}))
			dispatch(getScreenConfig({
				screenConfig: resource.screenConfig,
				roles: state.auth.user.Roles
			}))
		}
		else
			dispatch(showMessage(status))
		return status
	}
)

const settingsSlice = createSlice({
	name: 'panel/settings',
	initialState: {
		error: null
	},
	reducers: {
		setSettings: (state, { payload }) => {
			if (payload.Success == true) {
				var appConfig = payload.Resource.configs.values
				if (appConfig)
					Object.keys(appConfig).map((s) => state[s] = appConfig[s])
				state.version = payload.Resource.configs.version
			}
			else
				state.error = payload
		}
	},
	extraReducers: {
		[getSettings.fulfilled]: (state, action) => {

		}
	}
})

const createColumnCustomStore = (col, url, o) => {
	var cs = new CustomStore({
		key: col.data.key || "Id",
		loadMode: col.data.loadMode || 'processed',
		cacheRawData: isNotEmpty(col.data.cacheRawData) ? col.data.cacheRawData == true : false,
		load: (loadOptions) => {
			let filter = (o && o.data && col.data.filter) ? [col.data.filter[0], col.data.filter[1], _.get(o.data, col.data.filter[2])] : null
			let path = filter ? toQueryString({ filter: filter }, url) : url;
			return api.execute("GET", path).then(status => {
				return status.Success ? status.Resource.Items : []
			})
		},
		byKey: (key) => {
			return []
		}
	})
	return cs
}

const configureColumns = (name, columns, config) => {

	let dateFormats = config.lists.DateFormats;

	columns.map((col, i) => {
		// translate title
		if (name) {
			col.caption = i18next.t(col.title || `${name}.${col.name}`)
			if (col.caption == `${name}.${col.name}`)
				col.caption = i18next.t(`${col.name}`)
		}
		else
			col.caption = i18next.t(col.title || `${col.name}`)

		col.dataField = col.name
		col.hidden = col.hidden == true ? true : i > 14

		col.editorType = "dxTextBox"

		// 'string' | 'number' | 'date' | 'boolean' | 'object' | 'datetime'
		if (col.type == "date" || col.type == "time" || col.type == "datetime") {
			if (!dateFormats)
				console.error("DateFormats list is not defined.")

			var formatData = dateFormats.find(f => f.localized == col.format)
			col.localized = col.format;
			if (formatData)
				col.format = formatData.format[i18next.language] ?? formatData.format.tr
			col.dataType = "date"
			col.editorType = "dxDateBox"
		}
		else if (col.type == "check") {
			col.dataType = "boolean"
			col.editorType = "dxCheckBox"
		}
		else if (col.type == "numeric" || col.type == "number") {
			col.dataType = "number"
			col.editorType = "dxNumberBox"
			if (col.currency)
				col.format = {
					style: "currency",
					currency: col.currency
				}
		}
		else if (col.type == "currency") {
			col.dataType = "currency"
		}
		else if (col.type == "textArea") {
			col.editorType = "dxTextArea"
			col.dataType = "string"
		}
		else
			col.dataType = "string"

		// LOOKUP kolon set et
		if (col.autoComplete) {
			// sabit bir liste (enum vb)
			col.allowHeaderFiltering = true
			col.editorType = "dxSelectBox"
			col.lookup = {
				dataSource: config.lists[col.autoComplete],
				displayExpr: "caption",
				valueExpr: "id"
			}
		}
		else if (col.data) {

			// ön tanımlı listeler ile doldur
			if (col.data.type == "simpleArray") {
				col.allowHeaderFiltering = true
				col.editorType = "dxSelectBox"
				if (col.data.filter) {

					// diğer kolon değeri değiştiğinde bu kolon değerini boşalt
					let cascadeColumn = columns.find(e => e.name == col.data.filter[2])
					if (cascadeColumn) {
						cascadeColumn.setCellValue = (rowData, value, currentRowData) => {
							_.set(rowData, cascadeColumn.name, value)
							_.set(rowData, col.name, null)
						}
					}

					// bir diğer kolona göre filtreleme yapacak
					col.dataSource = (o) => {
						let filterValue = _.get(o.data, col.data.filter[2]);
						let filter = (o && o.data && col.data.filter) ? [col.data.filter[0], col.data.filter[1], filterValue] : null

						return {
							key: col.data.key || "Id",
							store: config.lists[col.data.name],
							filter
						}
					}
				}
				else {
					// var olan listeyi preloaded listeyi doğrudan ver
					col.dataSource = {
						key: col.data.key || "Id",
						store: config.lists[col.data.name]
					}
				}
			}
			// api'den kolon verisi yükler (henüz örneği yok, simpleArray ile çözüldü)
			else if (col.data.type == "customStore") {
				col.editorType = "dxSelectBox"

				if (col.data.filter) {
					col.customStore = (o) => createColumnCustomStore(col, col.data.url, o)
					// diğer kolon değeri değiştiğinde bu kolon değerini boşalt
					let cascadeColumn = columns.find(e => e.name == col.data.filter[2])
					console.warn('customStore is not support filter (mao)', col.name, cascadeColumn, col.data)
					if (cascadeColumn) {
						cascadeColumn.setCellValue = (rowData, value, currentRowData) => {
							_.set(rowData, cascadeColumn.name, value)
							_.set(rowData, col.name, null)
						}
					}
				}
				else {
					col.customStore = new CustomStore({
						key: col.data.key || "Id",
						loadMode: col.data.loadMode || 'raw',
						cacheRawData: isNotEmpty(col.data.cacheRawData) ? col.data.cacheRawData == true : false,
						load: () => api.execute("GET", col.data.url).then(status => status.Success ? status.Resource.Items : [])
					})
				}
			}

			// display için birden fazla kolon kullanımı
			let colNames = _.clone(col.data.displayExpr)
			if (Array.isArray(colNames)) {
				col.data.displayExpr = (r) => colNames.map(f => isNotEmpty(r[f]) ? r[f] : f).join('')
			}
		}
	})
}

const screenConfigSlice = createSlice({
	name: 'panel/settings/screenConfig',
	initialState: {
		cards: {},
		lists: {},
		names: [],
		screens: {}
	},
	reducers: {
		getScreenConfig: (state, action) => {

			let { screenConfig: config, roles } = { ...action.payload };

			// translate autoCoplete lists 
			Object.keys(config.lists).map(n => config.lists[n].map(l => {
				l.caption = l.value
				if (l.value)
					l.caption = i18next.t(`${n}.${l.value}`)
			}))

			if (config.names) {
				config.names.map((n) => {
					var s = config.screens[n];

					//data source
					let ds = s.dataSource || {};

					if (ds && ds.type == "simpleArray")
						delete s.editing

					if (s.editing && s.editing.roles) {
						let allowed = rolesAllowed(roles, s.editing.roles)
						if (!allowed)
							delete s.editing
					}

					s.dataSource = {
						...ds,
						load: (qs) => api.execute("GET", (ds.load ? (`${ds.load}${ds.load.indexOf('?') > -1 ? '&' : '?'}${qs}`) : `/api/entity?${qs}`)),
						insert: (values) => api.execute("POST", (ds.insert ? `${ds.insert}` : `/api/entity/${n}`), values),
						update: (key, values) => api.execute("PUT", (ds.update ? `${ds.update}` : `/api/entity/${n}/${key}`), values),
						delete: (key) => api.execute("DELETE", (ds.delete ? `${ds.delete}` : `/api/entity/${n}/${key}`)),
						params: {
							name: n
						},
					}

					// SUB MODELS
					if (s.subModels && s.subModels.length > 0) {
						s.subModels.map((subModel, j) => {
							let x = { ...config.screens[subModel.name] }
							delete x.subModels
							delete x.filters
							delete x.hideInSidebar
							s.subModels[j].screen = x;
						})
					}
					else s.subModels = []

					// columns order
					s.columns = _
						.orderBy(s.columns, ['index'], ['asc'])
						.filter(c => rolesAllowed(roles, c.roles, c))

					configureColumns(n, s.columns, config)

					s.columns.map(c => {
						if (c.editWith && c.editWith.type == "table") {
							configureColumns(c.data.name, c.editWith.columns, config)
							c.editWith.columns = _.orderBy(c.editWith.columns, ['index'], ['asc'])
							console.log("configureColumns", c.editWith.columns)
						}
					})
				});
			}

			state.cards = config.cards;
			state.lists = config.lists;
			state.names = config.names;
			state.screens = config.screens;

		}
	},
	extraReducers: {
	}
});

export const { setSettings } = settingsSlice.actions;
const settings = settingsSlice.reducer;

const { getScreenConfig } = screenConfigSlice.actions;
const screenConfig = screenConfigSlice.reducer

export { settings, screenConfig, configureColumns };
