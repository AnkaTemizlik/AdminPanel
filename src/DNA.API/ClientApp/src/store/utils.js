export const capitalize = (s) => {
	return s
		? s
			.split(" ")
			.map((s) => s.charAt(0).toUpperCase() + s.slice(1))
			.join(" ")
		: s;
}

export const splitCamelCase = (s) => {
	if (!s) return s
	let sentense = s.replace(/([a-z0-9])([A-Z0-9])/g, "$1 $2");
	//return s.split(/([A-Z][a-z]+)/).join(' ');
	return capitalize(sentense);
}

export const isNotEmpty = (value) => {
	return value !== undefined && value !== null && value !== '';
}

export const supplant = (text, data, alternate) => {
	if (!text)
		return text
	if (!data)
		return text
	var result = text.replace(/{([^{}]*)}/g,
		function (a, b) {
			let val = data[b] || alternate[b]
			return isNotEmpty(val) ? val : a;
		})
	console.log("ActionsView", result)
	return result
}

export const toQueryString = (o, url) => {

	let params = url ? (url.indexOf('?') > -1 ? '&' : '?') : ''
	if (!!o) {
		for (const key in o) {
			const element = o[key];
			let val = typeof element == "string" ? element : JSON.stringify(element)
			params += `${key}=${val}&`;
		}
	}
	params = params.slice(0, -1);
	return url ? url + params : params
}

export const addQueryFilter = (filter, newVal) => {
	if (Array.isArray(filter)) {
		var result = [[...filter]];
		result.push("and");
		result.push(newVal)
		return result
	}
	else {
		return [...newVal]
	}
}

export const rolesAllowed = (userRoles, itemRoles, m) => {
	let allowed = true
	let intersection = []
	if (userRoles && itemRoles) {
		let intersection = itemRoles.filter(x => userRoles.includes(x));
		if (!intersection)
			allowed = false
		if (intersection.length == 0)
			allowed = false
	}
	return allowed
}

export const copy = (text) => {
	window.copy(text)
	console.info("copied: ", text)
}

// https://momentjs.com/docs/#/displaying/format

