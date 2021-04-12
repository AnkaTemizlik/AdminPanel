import axios from 'axios';
import i18n from './i18n'
const development = (process.env && process.env.NODE_ENV === "development")

// AXIOS HELP : https://kapeli.com/cheat_sheets/Axios.docset/Contents/Resources/Documents/index
// sample: { validateStatus: (status) => (status >= 200 && status < 300) }
const instance = axios.create({
	baseURL: development
		? (window.location.origin.indexOf(':30') > -1
			? 'http://localhost:56811/'
			: window.location.origin)
		: `${window.location.origin}/`
});

export default instance;