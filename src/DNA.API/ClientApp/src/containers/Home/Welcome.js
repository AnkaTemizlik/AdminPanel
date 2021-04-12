
import React from 'react';
import { connect, useDispatch, useSelector } from "react-redux";
import { makeStyles } from "@material-ui/core/styles";
import { Box, Typography, Toolbar, Grid, Container } from '@material-ui/core';
import { Tr, useTranslation } from '../../store/i18next'
import { HomeMenu } from '../../components/Navigation/Menus';
import logo from '../../assets/logo6432.png'
import logo256 from '../../assets/logo.png'
import Plugin from '../../plugins'

const rnd = Math.floor((Math.random() * 6) + 1);

const useStyles = makeStyles(theme => {
	return {
		container: {
			paddingLeft: theme.spacing(12),
			[theme.breakpoints.down('xs')]: {
				paddingLeft: theme.spacing(1)
			}
		},
		logo: {
			marginTop: -44,
			width: 240,
			[theme.breakpoints.down('xs')]: {
				marginTop: -12,
				width: 160
			}
		}
	}
});


const Welcome = (props) => {

	var classes = useStyles();
	const { t } = useTranslation()
	const isAuthenticated = useSelector((state) => state.auth.token !== null)
	const homeMenu = useSelector((state) => state.menus.home)
	const { version } = useSelector((state) => state.settings)

	return <Box className={`bg-${rnd}`} style={{
		backgroundRepeat: "no-repeat !important",
		backgroundPosition: "center center",
		backgroundSize: "cover",
		backgroundAttachment: "fixed"
	}}
	>
		<div className="bg-overlay"></div>

		<Container maxWidth="lg" className={classes.container}>
			<Grid
				container
				direction="column"
				justify="space-around"
				alignItems="flex-start"
				style={{ height: "100vh" }}
			>
				<Box pt={18}></Box>
				<Grid item>
					<Toolbar>
						<img className={classes.logo} src={logo256} alt="" />
						<Typography variant="h2" style={{ color: "lightgray", paddingTop: 1 }}>{Plugin.Program || 'API'}</Typography>
						<Typography variant="body2" style={{ color: "lightgray", padding: "28px 0 0 16px" }}>{version}</Typography>
					</Toolbar>
				</Grid>
				<Box pt={1}></Box>
				<Grid item>
					<Toolbar>
						<Typography variant="h6" style={{ color: "white" }}>
							<Tr>{Plugin.Description || 'Access points to the background workers'}</Tr>
						</Typography>
					</Toolbar>
				</Grid>
				<Box flexGrow="1"></Box>
				<Grid item style={{ width: "100%" }}>
					<Toolbar>
						<HomeMenu menu={homeMenu} isAuthenticated={isAuthenticated} />
					</Toolbar>
					<Toolbar>
						<Box flexGrow="1"></Box>
						<Typography variant="caption" style={{ color: "lightgray" }}>
							Powered by &nbsp;<img style={{ margin: -2 }} src={logo} alt="" />
						</Typography>
					</Toolbar>
				</Grid>
				<Box pt={8}></Box>
			</Grid>
		</Container>
	</Box>
}

export default Welcome