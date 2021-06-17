import { createSlice, createAsyncThunk, createSelector, current } from '@reduxjs/toolkit';
import { setSettings } from './settingsSlice'
import api from '../api'
import i18n from '../i18n';
import { isNotEmpty } from '../utils';

export const selectSystemSettings = createSelector(
	[(state) => state.appSettings], (appSettings) => appSettings && appSettings.configs ? appSettings.configs[0] : {}
)

export const selectModuleSettings = createSelector(
	[(state) => state.appSettings], (appSettings) => appSettings && appSettings.configs ? appSettings.configs[1] : {}
)

export const saveAppSettings = createAsyncThunk(
	'settings/saveAppSettings',
	async (params, { dispatch, getState }) => {
		const state = getState().appsettings
		const status = await api.auth.saveSettings(0, state.changes)
		dispatch(setSettings({ ...status, Resource: { configs: status.Resource } }))
		let resource = status.Resource;
		dispatch(applyAppSettings(resource))
		return resource
	}
)

export const applyAppSettings = createAsyncThunk(
	'settings/applyAppSettings',
	async (configs, { dispatch, getState }) => {
		configs._ = configs._ || {}
		for (const key in configs.values) {
			const obj = configs.values[key];
			configs._[key] = prepareFieldOptions(key, configs.config, obj)
			var field = configs.config.Fields[key];
			var action = field ? field.action : null
			if (action)
				configs._[key]["action"] = { ...action }
		}
		dispatch(getAppSettings(configs))
		return configs
	}
)

export const prepareFieldOptions = (sectionName, config, values, parentName) => {

	let fields = [];
	let objects = [];
	let forms = [];

	Object.keys(values).map(function (p) {

		let fullname = parentName ? `${parentName}:${sectionName}:${p}` : `${sectionName}:${p}`;
		var options = getFieldProps(config, p, fullname);
		var { textArray, keyValue, visible, action } = options;
		if (visible) {
			let obj = { name: p, fullname, value: values[p], options };

			if (textArray || keyValue) {
				forms.push({
					...obj,
					type: "form"
				});
			}
			else if (typeof values[p] === "object") {
				if (isNotEmpty(values[p]))
					objects.push({
						...obj,
						type: "object",
						_: prepareFieldOptions(p, config, values[p], parentName ? `${parentName}:${sectionName}` : `${sectionName}`)
					});
			}
			else {
				fields.push({
					...obj,
					type: "field"
				});
			}
		}
	});

	let result = {
		section: sectionName,
		fields,
		objects,
		forms
	}

	return result
}

export const getFieldProps = (config, section, fullname) => {
	let name = section
	let options = {
		visible: true,
		caption: name
	}

	if (!config)
		return options;

	let fieldConfig = config.Fields[fullname];

	if (fieldConfig) {
		options.multiline = fieldConfig.multiline == true || fieldConfig.type == "textArray" || fieldConfig.type == "multiline"
		options.visible = fieldConfig.visible !== false
		options.readOnly = fieldConfig.readOnly

		if (fieldConfig.caption)
			options.caption = fieldConfig.caption

		options.inputType = fieldConfig.inputType || "text"

		if (fieldConfig.type == "check")
			options.check = true;
		else if (fieldConfig.type == "number") {
			options.number = true;
			options.inputType = "number"
		}
		else if (fieldConfig.type == "password")
			options.password = true;
		else if (fieldConfig.type == "textArea")
			options.textArea = true;
		else if (fieldConfig.type == "textArray")
			options.textArray = true;
		else if (fieldConfig.type == "keyValue")
			options.keyValue = true;
		else if (fieldConfig.type == "select")
			options.selectList = config.AutoCompleteLists[fieldConfig.options]

		if (fieldConfig.restartRequired)
			options.restartRequired = true;

		if (fieldConfig.showIcon)
			options.showIcon = true;

		options.action = { ...fieldConfig.action }

		if (options.keyValue) {
			options.keyCaption = fieldConfig.keyCaption;
			options.valueCaption = fieldConfig.valueCaption;
			options.valueType = fieldConfig.valueType;
		}

		if (fieldConfig.required)
			options.require = true

		if (fieldConfig.autoComplete) {
			options.autoComplete = Array.isArray(fieldConfig.autoComplete)
				? fieldConfig.autoComplete.map((f) => { return { model: f, fields: config.AutoCompleteLists[f] } })
				: { model: fieldConfig.autoComplete, fields: config.AutoCompleteLists[fieldConfig.autoComplete] }
		}
	}

	return options
}

const appsettingsSlice = createSlice({
	name: 'settings/appsettings',
	initialState: {
		configs: [],
		sectionNames: [],
		currentSection: null,
		changes: {},
		changeCount: 0,
		restartWarn: false
	},
	reducers: {
		getAppSettings: (state, action) => {
			state.configs = action.payload
			state.changes = {}
			state.restartWarn = false
			state.changeCount = 0
			let currentConfig = action.payload
			state.sectionNames = Object.keys(currentConfig.values).map((sec) => {
				let options = currentConfig.config.Fields[sec]
				if (options && options.visible === false)
					return null;
				return { name: sec, caption: i18n.t(options.caption || sec) }
			})
		},
		changeSection: (state, action) => {
			state.currentSection = action.payload
			state.changes = {}
			state.restartWarn = false
			state.changeCount = 0
		},
		addChange: (state, action) => {
			const { fullname, value } = action.payload
			var change = {}
			change[fullname] = value
			state.changes = { ...state.changes, ...change }
			state.changeCount = Object.keys(state.changes).length

		},
		discardChanges: (state, action) => {
			state.changes = {}
			state.restartWarn = false
			state.changeCount = 0
			state.currentSection = null
		},
		checkRestartWarn: (state, action) => {
			var { fieldName, selectedSection } = action.payload
			let config = state.configs.config
			let options = getFieldProps(config, selectedSection, fieldName)
			state.restartWarn = options.restartRequired == true
		}
	},
	extraReducers: {
		[applyAppSettings.fulfilled]: (state, action) => {
		},
		[applyAppSettings.rejected]: (state, action) => {
			console.error("applyAppSettings.rejected", action.error)
			state.error = action.payload
		}
	}
});

export const {
	getAppSettings,
	changeSection,
	addChange,
	discardChanges,
	checkRestartWarn,
	saveChanges
} = appsettingsSlice.actions;


export default appsettingsSlice.reducer