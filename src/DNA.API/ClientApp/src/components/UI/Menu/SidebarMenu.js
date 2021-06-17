import React from 'react'
import { useHistory, useParams, useLocation } from "react-router-dom";
import { makeStyles } from '@material-ui/core/styles'
import { NavLink } from 'react-router-dom'
import { ListItem, ListItemIcon, ListItemText, Divider, List, ListSubheader, Icon, IconButton, Collapse, ListItemSecondaryAction, Box } from '@material-ui/core'
import { connect } from 'react-redux'
import { useTranslation } from '../../../store/i18next'
import { ExpandLess, ExpandMore } from '@material-ui/icons'
import { rolesAllowed } from '../../../store/utils';
import Iconify from '../Icons/Iconify';

const useStyles = makeStyles((theme) => ({
	item: {
		paddingTop: theme.spacing(0),
		paddingBottom: theme.spacing(0),
		textDecoration: "none"
	},
	selected: {
		color: theme.palette.secondary.main,
		fontWeight: "bold"
	},
	selectedSub: {
		color: theme.palette.primary.main,
		fontWeight: "bold"
	}
}));

// function allowed(menu) {
// 	if (Roles && menu.roles) {
// 		let intersection = menu.roles.filter(x => Roles.includes(x));
// 		if (!intersection)
// 			return false
// 		if (intersection.length == 0)
// 			return false
// 	}
// 	return true
// }

const Menu = ({ setOpened, menu, to, isAuthenticated, t, level, roles }) => {

	const classes = useStyles();
	const [toggle, setToggle] = React.useState(true);

	const renderItems = () => {

		var items = []
		var lastDiv = false
		let i = 0

		menu.menus.map((m) => {

			if (m.visible === false)
				return

			if (isAuthenticated && m.showOnAuth == false)
				return
			if (!isAuthenticated && m.showOnAuth == true)
				return

			if (m.isDivider) {
				if (!lastDiv) {
					items.push(<Divider key={i++} />)
					lastDiv = true
				}
				return
			}
			else if (m.dividerBefore == true) {
				if (!lastDiv) {
					items.push(<Divider key={i++} />)
					lastDiv = true
				}
			}

			if (m.menus && m.menus.length > 0) {
				if (rolesAllowed(m.roles, roles)) {
					items.push(<Menu roles={roles} key={i++} setOpened={setOpened} menu={m} to={`${to}${m.to ? m.to : ''}`} t={t} isAuthenticated={isAuthenticated} level={level + 1} />)
					lastDiv = false
				}
				return
			}

			if (rolesAllowed(m.roles, roles, m)) {
				items.push(<Item key={i++} setOpened={setOpened} m={m} to={to} t={t} level={level + 1} />)
				lastDiv = false
				return
			}
		})
		return items
	}

	return <List style={{ padding: '0 0 0 ' + (level * 8) + 'px' }}>
		{menu.isHeaderVisible
			? (menu.noLink
				? <ListSubheader className={classes.item}>
					{t(`${menu.label}`)}
				</ListSubheader>
				: <ListItem button role={undefined}
					onClick={() => {
						setOpened(false)
					}}
					component={NavLink}
					to={to}
					target={menu.target || "_self"}
					activeClassName={classes.selectedSub}
				>
					{menu.icon
						? <ListItemIcon>
							<Iconify icon={menu.icon} />
						</ListItemIcon>
						//? <ListItemIcon><Icon>{menu.icon}</Icon></ListItemIcon>
						: null}
					<ListItemText id={menu.label + to} primary={t(`${menu.label}`)} />
					<ListItemSecondaryAction>
						<IconButton edge="end" onClick={() => setToggle(!toggle)}>
							{toggle ? <ExpandLess /> : <ExpandMore />}
						</IconButton>
					</ListItemSecondaryAction>
				</ListItem>
			)
			: null
		}

		{menu.menus && menu.menus.length > 0
			? <Collapse in={toggle} timeout="auto" unmountOnExit>
				{renderItems()}
			</Collapse>
			: null
		}
	</List>
}

const Item = ({ setOpened, m, to, t, level }) => {
	const classes = useStyles();
	return <ListItem button style={{ paddingLeft: level > 1 ? 36 : 24 }}
		onClick={() => { setOpened(false) }}
		component={NavLink}
		to={to + m.to}
		target={m.target || "_self"}
		activeClassName={classes.selected}>
		{m.icon
			? <ListItemIcon>
				<Iconify icon={m.icon} />
			</ListItemIcon>
			: null}
		<ListItemText primaryTypographyProps={{
			variant: level > 1 ? "body2" : "body1"
		}} primary={t(`${(m.pluginPage ? "plugin:" : "")}${m.label}`)} />
	</ListItem>
}

const SidebarMenu = ({ menu, setOpened, isAuthenticated, user, roles }) => {

	const { t } = useTranslation()

	if (!menu)
		return null;

	if (!rolesAllowed(menu.roles, roles, menu))
		return null

	return <Box pt={2}>
		<Menu roles={roles} setOpened={setOpened} menu={menu} key={0} to={menu.to} t={t} isAuthenticated={isAuthenticated} level={0} />
	</Box>
}

const mapStateToProps = (state) => ({
	user: state.auth.user,
	isAuthenticated: state.auth.token != null,
	roles: state.auth.user ? state.auth.user.Roles : null,
})

export default connect(mapStateToProps)(SidebarMenu)