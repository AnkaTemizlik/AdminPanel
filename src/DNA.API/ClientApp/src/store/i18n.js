import React, { useState } from 'react'
import { connect, useDispatch, useSelector } from "react-redux";
import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";
import HttpApi from 'i18next-http-backend'
import { locale } from "devextreme/localization";
import moment from 'moment'
import api from './api'
import axios from './axios'
import Plugin from '../plugins/index'

var BackendStatus;

function useBackendStatus() {
	const [status, setStatus] = useState(null);
	const [count, setCount] = useState(0);
	BackendStatus = function (status) {
		let c = count + 1;
		status.tryCount = c;
		setCount(c);
		setStatus(status)
	}
	return { status };
}

i18n
	.use({
		type: 'postProcessor',
		name: 'postProcess',
		process: function (value, key, options, translator) {
			return value;
		}
	})
	.use(initReactI18next)
	.use(HttpApi)
	.use(LanguageDetector)
	.init({
		//initImmediate: false,
		//lng: "tr",
		supportedLngs: Plugin.Languages,
		load: "languageOnly",
		lowerCaseLng: true,
		fallbackLng: Plugin.Languages[0],
		debug: true,
		postProcess: ["postProcess"],

		// resources: {
		// 	tr: {
		// 		common: {
		// 			name: "Adı Soyadı"
		// 		},
		// 		translations: {

		// 		}
		// 	},
		// 	en: {
		// 		common: {
		// 			name: "Name"
		// 		},
		// 		translations: {
		// 			"Adı": "Name",
		// 			"Kullanıcılar": "Users",
		// 			"Başlamak için, <i>src/App.js</i> aç ve kaydet.": "To get started, edit <1>src/App.js</1> and save to reload."
		// 		}
		// 	}
		// },
		backend: {
			loadPath: '{{lng}}|{{ns}}',
			//parse: (data) => { console.info("i18next", data) },
			jsonIndent: 2,
			request: function (options, url, payload, callback) {
				var lng = url.split('|')[0]
				var ns = url.split('|')[1]
				api.actions.run("GET", `/api/locales/${lng}/${ns}`)
					.then((status) => {
						BackendStatus && BackendStatus(status)
						if (status.Success) {
							callback(null, { status: 200, data: JSON.stringify(status.Resource) })
						}
						else {
							callback(status.Message, { status: 500 })
						}
					})
			}
		},
		detection: {
			order: ['querystring', 'cookie', 'localStorage', 'sessionStorage', 'navigator', 'htmlTag', 'path', 'subdomain'],
			caches: ['localStorage', 'cookie'],
		},
		//interpolation: {
		//escape: (str) => { return str; },
		//escapeValue: true, // not needed for react as it escapes by default
		//},
		ns: ["common", "plugin"],
		defaultNS: "common",
		fallbackNS: "plugin",
		//keySeparator: false, // we use content as keys
		react: {
			wait: true,
			useSuspense: true,
		}
	}, (err, t) => {

		moment.locale(i18n.language);
		locale(i18n.language);
		axios.defaults.headers.common['Accept-Language'] = i18n.language
		if (err)
			console.error("i18n", err)
	});

export {
	useBackendStatus
}

export default i18n

