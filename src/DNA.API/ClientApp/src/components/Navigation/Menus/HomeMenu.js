import React from 'react'
import { Link } from 'react-router-dom'
import { makeStyles, useTheme } from '@material-ui/core/styles';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import { Button, Divider, Icon, IconButton, Tooltip } from '@material-ui/core'
import { Trans, useTranslation } from '../../../store/i18next'

const useStyles = makeStyles((theme) => ({
	divider: {
		margin: theme.spacing(2.4, 2),
		backgroundColor: theme.palette.common.white
	},
	button: {
		textTransform: "none",
		minWidth: 0
	}
}));

const HomeMenu = ({ menu, isAuthenticated }) => {
	const theme = useTheme();
	const smUp = useMediaQuery(theme.breakpoints.up('sm'));
	const { t } = useTranslation();
	const classes = useStyles();

	return menu.menus.map((m, index) => {
		if (m.isDivider) {
			return <Divider key={index} orientation="vertical" flexItem className={classes.divider} />
		}

		if (m.showOnAuth)
			if (!isAuthenticated)
				return null

		return smUp
			? <Button key={index}
				className={classes.button}
				component={Link}
				to={m.to}
				target={m.target || "_self"}
				style={{ color: "white" }}
				startIcon={m.icon ? <Icon>{m.icon}</Icon> : undefined}
			>
				<Trans>{m.label}</Trans>
			</Button>
			: <Tooltip title={t(m.label)} key={index}>
				<span>
					<IconButton component={Link} edge="start"
						to={m.to}
						target={m.target || "_self"}
						style={{ color: "white" }} >
						<Icon>{m.icon}</Icon>
					</IconButton>
				</span>
			</Tooltip>
	})
}

export default HomeMenu