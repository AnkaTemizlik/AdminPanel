import Plugin from './P05'

const AppId = Plugin.AppId

export { AppId }

document.title = "" + (Plugin.Name || '') + " " + (Plugin.Program || '')

export const changeDxTheme = (color) => {
	require('../assets/dx.material.' + (color || Plugin.color || "blue") + '.css')
}

export default Plugin
