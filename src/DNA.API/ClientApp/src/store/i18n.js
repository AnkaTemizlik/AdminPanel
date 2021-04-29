import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";
import { locale } from "devextreme/localization";
import moment from 'moment'
import axios from './axios'

i18n
	.use(initReactI18next)
	.use(LanguageDetector)
	.init({
		initImmediate: false,
		resources: {},
		supportedLngs: ['tr', 'en'],
		load: "languageOnly",
		lowerCaseLng: true,
		fallbackLng: 'tr',
		debug: process.env && process.env.NODE_ENV === "development",
		detection: {
			order: ['querystring', 'cookie', 'localStorage', 'sessionStorage', 'navigator', 'htmlTag', 'path', 'subdomain'],
			caches: ['localStorage', 'cookie'],
		},
		ns: ["common"],
		defaultNS: "common"
		// react: {
		// 	wait: false,
		// 	useSuspense: true,
		// }
	});

const setLanguage = (language) => {
	i18n.changeLanguage(language);
	moment.locale(language);
	locale(language);
	axios.defaults.headers.common['Accept-Language'] = language
	console.success('i18n', language)
}

export { setLanguage }

export default i18n

