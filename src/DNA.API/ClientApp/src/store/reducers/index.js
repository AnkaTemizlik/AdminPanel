//import { combineReducers } from 'redux';
import { createSlice, combineReducers } from '@reduxjs/toolkit';
import auth from './auth'
import auth2 from '../slices/authSlice'
import entity from './entity'
import { screenConfig, settings } from '../slices/settingsSlice'
import appsettings from '../slices/appsettingsSlice'
import menus from '../slices/menuSlice'
import alerts from '../slices/alertsSlice'
import notification from '../slices/notificationSlice'
import panel from '../../containers/Panel/store'
import Plugin from '../../plugins'

const createReducer = asyncReducers => {
	return combineReducers({
		auth,
		auth2,
		entity,
		settings,
		screenConfig,
		appsettings,
		menus,
		panel,
		alerts,
		notification,
		...asyncReducers,
		...Plugin.slices
	});
}

export default createReducer
