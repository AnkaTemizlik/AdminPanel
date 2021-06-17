import { createSlice } from '@reduxjs/toolkit'
import i18next from '../i18n';
import _ from 'lodash'

const menusSlice = createSlice({
	name: 'menus',
	initialState: {},
	reducers: {
		resetPanelMenu: (state, action) => {
			state.panel = { ...state.default.panel }
		},
		loadMenu: (state, action) => {
			const menus = action.payload.Resource
			Object.keys(menus).map((s) => state[s] = menus[s])
			state.default = { ...action.payload.Resource }
		},
		addTokenToHangfireMenu: (state, action) => {
			const { token } = action.payload
			var jobsMenu = _.find(state.home.menus, { name: "Jobs" })
			jobsMenu.to = "/hangfire?access_token=" + token
		},
		appendMenusFromScreens: (state, action) => {
			let { screenConfig, user } = action.payload
			let definitions = []
			let index = 0;
			screenConfig.names.map((n, i) => {
				var s = screenConfig.screens[n];
				if (s.hideInSidebar !== true && s.isDefinitionModel !== true) {
					const newMenu = {
						label: s.title,
						to: "/screen/" + s.route,
						icon: s.icon,
						isHeaderVisible: true,
						areMenusVisible: s.subMenus && s.subMenus.length > 0,
						roles: s.roles,
						pluginPage: s.type == "customPage",
						menus: [],
					};

					s.subMenus && s.subMenus.map((sm) => {
						if (sm.showInSidebar == true) {
							var title = sm.title ? i18next.t(sm.title) : i18next.t(sm.name)
							newMenu.menus.push({
								name: sm.name,
								label: title,
								to: ("/" + sm.defaultFilter[0] + "/" + sm.defaultFilter[2]) + "?title=" + title,
								icon: sm.icon,
								isHeaderVisible: true,
								roles: sm.roles,
								pluginPage: s.type == "customPage",
								menus: [],
							})
						}
					})
					state.panel.menus.splice(index++, 0, newMenu);
				}

				if (s.isDefinitionModel)
					definitions.push(s)
			});

			let definitionsMenu = _.find(state.panel.menus, { name: "Definitions" }) || { label: "Tanımlar", to: "", name: "Definitions", areMenusVisible: true, isHeaderVisible: true, menus: [] }
			if (definitions.length > 0) {
				definitions.map((s) => {
					definitionsMenu.menus.push({
						label: s.title,
						to: "/screen/" + s.route,
						icon: s.icon,
						isHeaderVisible: true,
						roles: s.roles,
						pluginPage: s.type == "customPage",
						menus: [],
					});
				})
			}
			else
				definitionsMenu.visible = false

			var logsMenu = _.find(state.panel.menus, { name: "Logs" })
			if (!logsMenu) {
				state.panel.menus.push({ isDivider: true });
				state.panel.menus.push({
					label: "Logs",
					name: "Logs",
					to: "/logs",
					icon: "adb",
					roles: ["Admin"],
					menus: []
				});
			}
		},
		//appendMenusFromConfig: (state, action) => {

		// let { configs, user } = action.payload
		// let m = _.find(state.panel.menus, { name: "Settings" })

		// let settingsMenu = m ? m : {
		// 	label: "Settings",
		// 	name: "Settings",
		// 	to: "",
		// 	areMenusVisible: true,
		// 	isHeaderVisible: true,
		// 	roles: ["Admin", "Writer"],
		// 	menus: [],
		// };

		// settingsMenu.menus = settingsMenu.menus || []

		// settingsMenu.menus.push({
		// 	label: "Users",
		// 	to: "/users",
		// 	isHeaderVisible: true,
		// 	icon: "people",
		// 	roles: ["Admin", "Writer"],
		// 	menus: []
		// });


		// settingsMenu.menus.push({
		// 	label: configs.title,
		// 	to: "/settings",
		// 	icon: configs.icon,
		// 	roles: ["Admin", "Writer"],
		// 	menus: [],
		// });

		// if (!settingsMenu) {
		// 	state.panel.menus.push({ isDivider: true });
		// 	state.panel.menus.push(settingsMenu)
		// }

		// var adminSettingsMenu = _.find(state.admin.menus, { name: "Settings" })
		// if (!adminSettingsMenu) {
		// 	state.admin.menus.push({ isDivider: true });
		// 	state.admin.menus.push(settingsMenu)
		// }
		//}
	},
	extraReducers: {}
});

export const {
	//appendMenusFromConfig,
	appendMenusFromScreens,
	addTokenToHangfireMenu,
	resetPanelMenu,
	loadMenu
} = menusSlice.actions;

export default menusSlice.reducer;
