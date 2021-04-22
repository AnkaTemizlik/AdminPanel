import axios from 'axios';
const development = (process.env && process.env.NODE_ENV === "development")
const Host = 'http://localhost:8800'

// AXIOS HELP : https://kapeli.com/cheat_sheets/Axios.docset/Contents/Resources/Documents/index
// sample: { validateStatus: (status) => (status >= 200 && status < 300) } // 
let url = development
	? (window.location.origin.indexOf(':30') > -1
		? Host
		: window.location.origin)
	: `${window.location.origin}/`

console.log("Development:", development)
console.log("Host:", Host)
console.log("Origin:", window.location.origin)
console.log("Axios:", url)

const instance = axios.create({
	baseURL: url
});

export default instance;
