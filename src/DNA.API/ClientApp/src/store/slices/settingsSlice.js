import _ from 'lodash'
import { createSlice, createAsyncThunk, createSelector, current } from '@reduxjs/toolkit';
import { appendMenusFromScreens, resetPanelMenu, addTokenToHangfireMenu } from './menuSlice'
import { applyAppSettings } from './appsettingsSlice'
import { showMessage } from './alertsSlice'
import api from '../api'
import i18next from 'i18next'
import CustomStore from 'devextreme/data/custom_store';
import DataSource from "devextreme/data/data_source";
import { isNotEmpty, rolesAllowed, supplant, toQueryString } from '../utils';
import { ApiURL } from '../axios';

export const applyGlobalSettings = createAsyncThunk(
	'panel/settings/applyGlobalSettings',
	async (params, { dispatch, getState }) => {
		console.purple("applyGlobalSettings", params)
		let status = params;
		dispatch(setSettings(status))
		if (status.Success) {
			let resource = { ...status.Resource }
			dispatch(applyAppSettings(resource.configs))
		}
		else
			dispatch(showMessage(status))
		return status
	}
)

export const getSettings = createAsyncThunk(
	'panel/settings/getSettings',
	async (params, { dispatch, getState }) => {
		const status = await api.auth.getSettings()
		dispatch(setSettings(status))
		if (status.Success) {
			let resource = { ...status.Resource }
			console.purple("getSettings resource.configs", resource.configs)
			dispatch(applyAppSettings(resource.configs))
			dispatch(resetPanelMenu())
			const state = getState();
			dispatch(addTokenToHangfireMenu({ token: state.auth2.token }))
			dispatch(appendMenusFromScreens({
				screenConfig: resource.screenConfig,
				user: state.auth2.user
			}))
			// dispatch(appendMenusFromConfig({
			// 	configs: resource.configs,
			// 	user: state.auth.user
			// }))
			dispatch(getScreenConfig({
				screenConfig: resource.screenConfig,
				roles: state.auth2.user.Roles
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
				state.workerVersion = payload.Resource.configs.workerVersion
				state.Plugin = payload.Resource.configs.Plugin
				state.AppId = payload.Resource.configs.AppId
				state.MultiLanguage = payload.Resource.configs.MultiLanguage
				state.Logo = payload.Resource.configs.Logo
				state.LicenseStatus = payload.Resource.configs.LicenseStatus
			}
			else
				state.error = payload
		}
	},
	extraReducers: {
		//[getSettings.fulfilled]: (state, action) => {	}
	}
})

const createColumnCustomStore = (col, url, o) => {
	var cs = new CustomStore({
		key: col.data.key || "Id",
		//loadMode: col.data.loadMode || 'processed',
		cacheRawData: isNotEmpty(col.data.cacheRawData) ? col.data.cacheRawData == true : false,
		load: (loadOptions) => {
			console.purple("createColumnCustomStore load", loadOptions)
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
	return new DataSource({
		store: cs,
		paginate: true,
		pageSize: 10
	})
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

		col.editorOptions = {}
		col.editorOptions.showClearButton = col.showClearButton

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
			if (col.withTimeEdit)
				col.editorOptions.type = "datetime"
			col.editorType = "dxDateBox"
			col.editorOptions.dateSerializationFormat = col.dateSerializationFormat || "yyyy-MM-ddTHH:mm:ss"
		}
		else if (col.type == "check" || col.type == "boolean" || col.type == "bool") {
			col.dataType = "boolean"
			col.editorType = "dxCheckBox"
		}
		else if (col.type == "numeric" || col.type == "number") {
			col.dataType = "number"
			col.editorType = "dxNumberBox"
			if (col.currency) {
				col.dataType = "currency"
				col.format = {
					style: "currency",
					currency: col.currency || "TRY"
				}
			}
		}
		else if (col.currency || col.type == "currency") {
			col.dataType = "currency"
			col.format = {
				style: "currency",
				currency: col.currency || "TRY"
			}
		}
		else if (col.type == "textArea") {
			col.editorType = "dxTextArea"
			col.dataType = "string"
			col.editorOptions.autoResizeEnabled = true
		}
		else if (col.type == "color") {
			col.editorType = "dxColorBox"
			col.editorOptions.applyValueMode = "instantly"
		}
		// else if (col.type == "image") {
		// 	col.editorType = "dxColorBox"
		// }
		else {
			col.dataType = "string"
		}

		if (col.stringLength > 0) {
			//console.log("stringLength", col)
			col.editorOptions = { ...col.editorOptions, maxLength: col.stringLength }
		}

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
			col.data.valueExpr = col.data.valueExpr || "Id"
			col.data.displayExpr = col.data.displayExpr || "Name"
			let displayExpr = _.clone(col.data.displayExpr)
			col.getFilter = (searchValue) => {
				if (searchValue) {
					let f = []
					if (Array.isArray(displayExpr)) {
						displayExpr.map((d, i) => {
							f.push([d, "contains", searchValue])
							if (i != displayExpr.length - 1)
								f.push("or")
						})
					}
					else {
						f = [displayExpr, "contains", searchValue]
					}
					return f
				}
			}

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

				col.data.url = col.data.url || '/api/entity?name=' + col.data.name
				if (col.data && col.data.filter) {
					col.editorType = "dxSelectBox"
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
					col.editorType = "dxLookup"
					col.customStore = new CustomStore({
						key: col.data.key || "Id",
						loadMode: col.data.loadMode || 'raw',
						cacheRawData: isNotEmpty(col.data.cacheRawData) ? col.data.cacheRawData == true : true,
						load: (loadOptions) => {
							//console.purple("col customStore load", loadOptions)
							let url = col.data.url;
							if (loadOptions && loadOptions.take) {
								let params = {
									page: loadOptions.skip / loadOptions.take,
									take: loadOptions.take,
									requireTotalCount: false,
								}
								let filter = col.getFilter(loadOptions.searchValue)
								if (filter) params.filter = filter
								url = toQueryString(params, url)
							}
							//console.purple("col customStore load", loadOptions, col, params)
							return api.execute("GET", url)
								.then(status => {
									return status.Success ? status.Resource.Items : []
								})
						},
						byKey: (k) => {
							//console.purple("col customStore byKey", k)
							return api.execute("GET", "api/entity/" + col.data.name + "/" + k)
								.then(status => {
									return status.Success ? status.Resource : null
								})
						}
					})
				}
			}

			//displayExpr
			if (Array.isArray(displayExpr)) {
				col.data.displayExpr = (r) => {
					return r ? displayExpr.map(f => isNotEmpty(r[f]) ? r[f] : f).join('') : ''
				}
			}

		}

	})
}

const createFullDataSource = (ds, n) => {
	return {
		...ds,
		load: (qs) => api.execute("GET", (ds.load ? (`${ds.load}${ds.load.indexOf('?') > -1 ? '&' : '?'}${qs || 'name=' + (ds.name || n)}`) : `/api/entity?${qs || 'name=' + (ds.name || n)}`)),
		byKey: (key) => api.execute("GET", (ds.byKey ? (`${ds.byKey}/${key}`) : `/api/entity/${n}/${key}`)),
		insert: (values) => api.execute("POST", (ds.insert ? `${ds.insert}` : `/api/entity/${n}`), values),
		update: (key, values) => api.execute("PUT", (ds.update ? `${ds.update}/${key}` : `/api/entity/${n}/${key}`), values),
		delete: (key) => api.execute("DELETE", (ds.delete ? `${ds.delete}/${key}` : `/api/entity/${n}/${key}`)),
		params: {
			name: n
		},
	}
}

const createDataSource = (ds, n) => {
	return new DataSource({
		...ds,
		load: () => {
			let url = ds.load
				? (`${ds.load}${ds.load.indexOf('?') > -1 ? '&' : '?'}${'name=' + (ds.name || n)}`)
				: `/api/entity?${'name=' + (ds.name || n)}`
			console.purple("createSelectDataSource", url, ds)
			return api.execute("GET", url)
		},
		byKey: (id) => {
			//return api.execute("GET", `/api/entity/${ds.name || n}/${id}`)
		},
		params: {
			name: ds.name || n
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
					if (!isNotEmpty(s.newRowDefaultValues))
						s.newRowDefaultValues = {}

					//data source
					let ds = s.dataSource || {};

					if (ds && ds.type == "simpleArray")
						delete s.editing

					if (s.editing && s.editing.roles) {
						let allowed = rolesAllowed(roles, s.editing.roles)
						if (!allowed)
							delete s.editing
					}

					s.dataSource = createFullDataSource(ds, n)

					// columns order
					s.columns = _
						.orderBy(s.columns, ['index'], ['asc'])
						.filter(c => rolesAllowed(roles, c.roles, c))

					configureColumns(n, s.columns, config)

					s.columns.map(c => {
						if (c.editWith && c.editWith.type == "table") {
							configureColumns(c.data.name, c.editWith.columns, config)
							c.editWith.columns = _.orderBy(c.editWith.columns, ['index'], ['asc'])
							//console.log("configureColumns", c.editWith.columns)
						}
						if (isNotEmpty(c.defaultValue)) {
							// eslint-disable-next-line no-eval
							s.newRowDefaultValues[c.name] = eval(c.defaultValue)
						}
					})

					//Calendar Resources
					if (s.calendar) {
						s.calendar.dataSource = createFullDataSource(s.calendar.dataSource, n)
						s.calendar.resources && s.calendar.resources.map(r => {
							r.label = i18next.t(r.label)
							var resourceColumn = s.columns.find(c => c.name == r.fieldExpr)
							if (resourceColumn) {
								console.info("resourceColumn", resourceColumn)
								r.dataSource = new DataSource({
									store: resourceColumn.customStore || resourceColumn.dataSource || resourceColumn.lookup.dataSource
								})
							}
						})
					}

				});
				config.names.map((n) => {
					var s = config.screens[n];
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
