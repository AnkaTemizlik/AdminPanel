import axios from 'axios';
const development = (process.env && process.env.NODE_ENV === "development")
const Host = 'https://localhost:44389/'

// AXIOS HELP : https://kapeli.com/cheat_sheets/Axios.docset/Contents/Resources/Documents/index
// sample: { validateStatus: (status) => (status >= 200 && status < 300) } // 
const instance = axios.create({
	baseURL: development
		? (window.location.origin.indexOf(':30') > -1
			? Host
			: window.location.origin)
		: `${window.location.origin}/`
});

export default instance;
