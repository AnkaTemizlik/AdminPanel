import axios from 'axios';
const development = (process.env && process.env.NODE_ENV === "development")
const Host = 'http://192.168.1.200:8800'

// AXIOS HELP : https://kapeli.com/cheat_sheets/Axios.docset/Contents/Resources/Documents/index
// sample: { validateStatus: (status) => (status >= 200 && status < 300) } // 
const ApiURL = new URL(development
	? (window.location.origin.indexOf(':30') > -1
		? Host
		: window.location.origin)
	: `${window.location.origin}/`)

console.log("Development:", development)
console.log("Host:", Host)
console.log("Origin:", window.location.origin)
console.log("Axios:", ApiURL)

const instance = axios.create({
	baseURL: ApiURL.origin
});

export { ApiURL }

export default instance;
