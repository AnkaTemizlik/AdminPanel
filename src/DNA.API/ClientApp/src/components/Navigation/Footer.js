import React from 'react'
import { useSelector } from "react-redux";
import clsx from 'clsx'
import { makeStyles } from '@material-ui/core/styles';
import { Typography, Grid, Paper, Box, Hidden, Tooltip } from '@material-ui/core'
import { ContactMenu, HomeMenu, SocialMenu, MainFooterMenu } from './Menus'
import { Trans, Tr } from '../../store/i18next'
import BrandLogo from './BrandLogo';
import LinksMenu from './Menus/LinksMenu';

const useStyles = makeStyles((theme) => ({
	root: {
		flexGrow: 1,
	},
	paper: {
		padding: theme.spacing(2),
		color: theme.palette.text.secondary,
	},
	center: {
		textAlign: 'center',
	}
}));

function Footer(props) {
	const { full } = props
	const classes = useStyles();
	const { version, workerVersion } = useSelector((state) => state.settings)

	return <div className={classes.root}>
		<Grid container spacing={2}>
			{/* Links  */}
			{full
				? <Grid item xs={12} sm={6} md={4}>
					<Paper className={classes.paper}>
						<LinksMenu menu={props.menus.links} />
					</Paper>
				</Grid> : null}

			{/* Main Footer Menu */}
			{full
				? <Grid item xs={12} sm={6} md={4}>
					<Paper className={classes.paper}>
						<MainFooterMenu menu={props.menus.main} />
					</Paper>
				</Grid> : null}

			{/* Contact */}
			{full
				? <Grid item xs={12} md={4}>
					<Paper className={classes.paper}>
						<ContactMenu menu={props.menus.contact} />
					</Paper>
				</Grid> : null}

			{/* Social */}
			<Grid item xs={12}>
				<Paper className={clsx(classes.paper, classes.center)}>
					<SocialMenu menu={props.menus.social} />
				</Paper>
			</Grid>

			{/* Copyright */}
			<Grid item xs={12}>
				<Box display="flex" justifyContent="center" m={3} alignItems="center">
					{"Copyright © "}
					{new Date().getFullYear() + "."}
					&nbsp;<Tr>All Rights Reserved</Tr>
					&nbsp;<Tooltip title={version}><span>v{workerVersion}</span></Tooltip>
				</Box>
				<Box display="flex" justifyContent="center">
					<BrandLogo width={80} />
				</Box>
			</Grid>
		</Grid>
	</div>
}

export default Footer