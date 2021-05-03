import React from 'react'
import { IconButton } from '@material-ui/core'
import * as Icons from '@material-ui/icons'
import { useTranslation } from '../../../store/i18next'

const SocialMenu = ({ minimal, menu }) => {
	const { t } = useTranslation()
	return <>
		{menu && menu.menus && menu.menus.map((m, index) => {
			if (minimal && !menu.primary)
				return null;
			if (m.visible == false)
				return null
			return <IconButton key={index}
				//component={Link}
				href={m.to}
				target="_blank"
				edge="start"
				title={t(m.label)}
				rel="noopener noreferrer"
				style={{
					color: m.color
				}}>
				{React.createElement(Icons[m.icon])}
			</IconButton>
		})}
	</>
}

export default SocialMenu