import React from 'react'
import {
	withTranslation,
	useTranslation as useT,
	Trans
} from 'react-i18next'

const useTranslation = () => {
	//return useT()
	const { t } = useT()
	let translate = (val) => {
		let result = t(val)
		if (typeof result == "object")
			return val
		return result
	}
	return { t: translate }
}

const Tr = (props) => {
	const { t } = useT()
	let options = {}
	let newText = props.children
	let i = 0
	if (newText) {
		if (newText.indexOf('|') > 0) {
			let alertCode = newText.split('|')[0].trim()
			let values = newText.split('|')[1].trim()
			let translated = t(alertCode) + " " + values;
			return translated
		}
		else {
			newText = newText.replace(/\[(\w+)=(\w+)\]/g, (str, p1, p2) => {
				options[`${i}`] = `[${p1}:${p2}]`
				let index = "{{" + i + "}}"
				i++;
				return index;
			})
			let val = t(newText, options)
			return val;
		}
	}
	else {
		let val = t(newText)
		return val
	}
}

export {
	withTranslation,
	useTranslation,
	Tr,
	Trans
}
