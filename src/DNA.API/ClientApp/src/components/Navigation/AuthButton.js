
import React from 'react'
import { Link } from 'react-router-dom'
import { makeStyles } from "@material-ui/core/styles";
import IconButton from '@material-ui/core/IconButton';
import Button from '@material-ui/core/Button';
import AccountCircleTwoTone from '@material-ui/icons/AccountCircleTwoTone';
import { Menu, MenuItem, Divider, ListItemIcon, Typography, Hidden, Icon } from '@material-ui/core';
import { usePopupState, bindMenu, bindTrigger } from 'material-ui-popup-state/hooks'
import { Trans, useTranslation } from '../../store/i18next'

const useStyles = makeStyles(theme => {
	return {
		button: {
		},
		loginButton: {
			textTransform: 'none',
			fontWeight: 'normal',
			[theme.breakpoints.down('sm')]: {
				fontWeight: 'bold',
			}
		},
		registerButton: {
			textTransform: 'none',
			fontWeight: 'bold'
		}
	}
});

const AuthButton = (props) => {

	const { t } = useTranslation()
	var classes = useStyles();
	const popupState = usePopupState({ variant: 'popover', popupId: 'authButton' })

	const { isAuthenticated, menu } = props

	return (isAuthenticated
		? <>
			<IconButton {...bindTrigger(popupState)} >
				<AccountCircleTwoTone />
			</IconButton>
			<Menu
				style={{ width: '240px' }}
				{...bindMenu(popupState)}
				// anchorEl={anchorEl}
				// keepMounted
				// open={Boolean(anchorEl)}
				// onClose={handleClose}
				getContentAnchorEl={null}
				disableScrollLock={true}
				anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
				transformOrigin={{ vertical: 'top', horizontal: 'left' }}
			>

				{menu.menus.map((m, i) => {
					if (m.divider)
						return <Divider key={i} />
					if (m.showOnAuth === false)
						return null
					return <MenuItem style={{ width: '240px' }} onClick={popupState.close} component={Link} to={m.to} key={i}>
						{m.icon &&
							<ListItemIcon>
								<Icon>{m.icon}</Icon>
							</ListItemIcon>}
						<Typography variant="inherit">
							<Trans>{m.label}</Trans>
						</Typography>
					</MenuItem>
				})}
			</Menu>
		</>
		: menu.menus.map((m, i) => {
			if (m.isLogin)
				return <Button component={Link} to={m.to} onClick={popupState.close} color="inherit"
					className={classes.loginButton}
					key={i}>
					<Trans>{m.label}</Trans>
				</Button>

			if (m.isRegister)
				return <Hidden smDown key={i}>
					<Button component={Link} to={m.to} onClick={popupState.close} color="inherit"
						className={classes.registerButton} >
						<Trans>{m.label}</Trans>
					</Button>
				</Hidden>
			if (isAuthenticated && m.showOnAuth === false)
				return null
			if (!isAuthenticated && m.showOnAuth === true)
				return null

			if (m.label)
				return <Button component={Link} to={m.to} onClick={popupState.close} color="inherit" key={i}>
					<Trans>{m.label}</Trans>
				</Button>

			return null
		})
	)
}

export default React.memo(AuthButton)