import React from 'react'
import { useSelector } from "react-redux";
import { Link } from 'react-router-dom'
import { makeStyles } from '@material-ui/core/styles';
import { Button } from '@material-ui/core'
import { Trans } from '../../../store/i18next'

const useStyles = makeStyles((theme) => ({
	button: {
		textTransform: "none"
	}
}));

const HomeMenu = ({ menu }) => {
	const { home } = useSelector((state) => state.menus)
	const classes = useStyles();
	return home.menus.map((menu, index) => {
		return <Button key={index} className={classes.button} component={Link} to={menu.to} color="inherit">
			<Trans>{menu.label}</Trans>
		</Button>
	})
}

export default HomeMenu