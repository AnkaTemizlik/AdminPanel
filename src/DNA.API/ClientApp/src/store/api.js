import axios from "./axios";
import Plugin from "../plugins";
import i18n from "i18next";

const check = (response, errorMessage) => {

	if (response) {
		if (response.data && response.data.Message) {
			if (response.data.Message.indexOf("|") > -1) {
				let alertCode = response.data.Message.split("|")[0].trim();
				let values = response.data.Message.split("|")[1].trim();
				let translated = i18n.t(alertCode);

				if (values.trim().length > 0) translated += " " + values;
				response.data.Message = translated;
			}
		}

		if (response.statusText === "OK" || response.statusText === "Created") {
			if (response.data) {
				if (response.data.Success === false) {
					if (response.data.Details) console.warn(response.data.Details);
				}
				return response.data;
			} else {
				// data yok
				return { Success: false, Message: `${response.statusText}` };
			}
		} else {
			// status 200 veya 201 değil
			if (response.data) {
				if (response.data.Success === false) if (response.data.Details) console.warn(response.data.Details);

				if (response.data.Success === true || response.data.Success === false) {
					return response.data;
				} else if (response.data.Message) {
					return { Success: false, Message: `${response.data.Message}`, Details: response.data.StackTrace };
				} else {
					let msg = null;
					let err = null;
					for (var i in response.data) {
						if (response.data.hasOwnProperty(i)) {
							err = i;
							msg = response.data[i] ? response.data[i][0] : null; // {"something_sure_happened":["Hiçbir şey olmasa bile, kesin bir şeyler oldu."]}
						}
					}
					if (msg) return { Success: false, Message: `${msg} <${response.statusText}:${err}>` };
					else return { Success: false, Message: `${response.data} <${response.statusText}>` };
				}
			} else {
				// data yok
				return { Success: false, Message: `${response.status}:${response.statusText}` };
			}
		}
	} else {
		return { Success: false, Message: `${errorMessage}` };
	}
};

const execute = (request) => {
	return request
		.then((response) => {
			const status = check(response);
			if (!status.Success)
				console.error("[API].error", status);
			return Promise.resolve(status);
		})
		.catch((error) => {
			console.error("[API].error", error.response, error.message);
			const status = check(error.response, error.message);
			return Promise.resolve(status);
		});
};

const execute2 = async (request) => {
	try {
		var response = await request
		const status = check(response);
		return status
	} catch (error) {
		console.error("[API].error 2", error.response, error.message);
		const status = check(error.response, error.message);
		return status
	}
};

const init = (id) => {
	let url = "/api/auth/init?id=" + id;
	return execute(axios.get(url));
};
const login = (data) => {
	let url = "/api/auth/login";
	return execute(axios.post(url, data));
};
const register = (data) => {
	let url = "/api/auth/register";
	return execute(axios.post(url, data));
};
const confirm = (data) => {
	let url = "/api/auth/confirm";
	return execute(axios.post(url, data));
};
const recovery = (data) => {
	let url = "/api/auth/recovery";
	return execute(axios.post(url, data));
};
const changePassword = (data) => {
	let url = "/api/auth/changePassword";
	return execute(axios.post(url, data));
};
const getSettings = async () => {
	let url = "/api/auth/settings";
	return await execute2(axios.get(url));
};
const saveSettings = (id, data) => {
	let url = "/api/auth/settings/" + id;
	return execute(axios.post(url, data));
};
const getUsers = (query) => {
	let url = `/api/auth/users`;
	let queryString = "";
	if (query.size > 0) queryString += `page=${query.page}&take=${query.size}&requireTotalCount=true`;

	url += (queryString.length > 0 ? "?" : "") + queryString;
	return execute(axios.get(url));
};
const getUser = (id) => {
	let url = `/api/auth/users/${id}`;
	return execute(axios.get(url));
};
const saveUser = (user, key) => {
	let url = "/api/auth/users";
	return execute(axios.post(url, { ...user, key: key }));
};
const auth = {
	init,
	login,
	register,
	confirm,
	recovery,
	changePassword,
	getSettings,
	saveSettings,
	getUsers,
	getUser,
	saveUser,
};
/** LOGS */
const getLogs = (query) => {

	let url = `/api/log`;
	if (typeof query == "string") {
		url += (query.indexOf('?') > -1 ? '' : '?') + query
	}
	else {
		let queryString = "";
		if (query.size > 0) queryString += `page=${query.page}&take=${query.size}&requireTotalCount=true`;
		if (query.filter) {
			var f = "";
			for (const key in query.filter) {
				if (query.filter.hasOwnProperty(key)) {
					const element = query.filter[key];
					if (element) {
						if (f.length > 0) f += " AND ";
						if (key == "search") {
							f += `(Message LIKE '%${element}%' OR Logger LIKE '%${element}%' OR Callsite LIKE '%${element}%' OR Exception LIKE '%${element}%')`;
						} else {
							f += key + "='" + element + "'";
						}
					}
				}
			}
			queryString += (queryString.length > 0 && f.length > 0 ? "&" : "") + (f.length > 0 ? "filter=" + encodeURIComponent(f) : "");
		}
		url += (queryString.length > 0 ? "?" : "") + queryString;
	}
	return execute(axios.get(url));
};
/** LOGS */
const getFileLogs = () => {
	let url = `/api/log/file`;
	return execute(axios.get(url));
};
const getLog = (id) => {
	let url = "/api/log/" + id;
	return execute(axios.get(url));
};
const log = {
	getLogs,
	getLog,
	getFileLogs,
};
/** ENTITY */
const getById = async (name, id) => {
	let url = `/api/entity/${name}/${id}`;
	return await execute2(axios.get(url));
}

const getEntities = async (query) => {
	console.info("getEntities", query)
	let url = `/api/entity`;
	if (typeof query == "string")
		return await execute2(axios.get(url + (query.indexOf('?') > -1 ? '' : '?') + query));
	else {
		let queryString = "";
		if (query.size > 0) queryString += `page=${query.page}&take=${query.size}&requireTotalCount=true`;
		queryString += "&name=" + query.name;
		if (query.filter) {
			var f = query.filter;
			queryString += (queryString.length > 0 && f.length > 0 ? "&" : "") + (f.length > 0 ? "filter=" + f : "");
		}
		url += (queryString.length > 0 ? "?" : "") + queryString;
		return await execute2(axios.post(url, query.conditions || []));
	}
};

/** CARDS */
const getCards = (names) => {
	let url = `/api/entity/cards`;
	let queryString = "";
	names &&
		names.map((n) => {
			if (queryString.length > 0) queryString += "&";
			queryString += "Names[]=" + n;
		});

	url += queryString.length > 0 ? "?" + queryString : "";
	return execute(axios.get(url));
};
const entity = {
	getEntities,
	getCards,
	getById
};
/** NOTIFICATION */
const getNotifications = (query) => {
	let url = "/api/notification?RequireTotalCount=true&" + query;
	return execute(axios.get(url));
};
/** SCREEN ACTIONS */
const run = (method, url, params) => {
	console.info("getEntities run", method, url, params)
	if (method == "GET") return execute(axios.get(url));
	else if (method == "DELETE") return execute(axios.delete(url, params));
	else if (method == "POST") return execute(axios.post(url, params));
	else if (method == "PUT") return execute(axios.put(url, params));
	else {
		var status = { Success: false, Error: `The '${method}' request method is not implemented.` };
		console.error("[API].error", status);
		return Promise.resolve(status);
	}
};
const actions = {
	run,
	getNotifications
};
/** exports */
const exports = {
	auth,
	log,
	entity,
	actions,
	execute: run,
	...Plugin.api
};

export default exports