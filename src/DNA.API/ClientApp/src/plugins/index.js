import Plugin from './P00'

const AppId = "QzN8M6JS"

export { AppId }

document.title = "" + (Plugin.Name || '') + " " + (Plugin.Program || '')

export default {
	...(Plugin || {}),
	Color: Plugin.Color || "blue",
	Languages: Plugin.Languages ?? ["tr"]
}
