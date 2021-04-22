import React from 'react';
import { useSelector } from "react-redux";
import { makeStyles } from "@material-ui/core/styles";
import { Box, Typography, Toolbar, Grid, Container } from '@material-ui/core';
import { Tr } from '../../store/i18next'
import { HomeMenu } from '../../components/Navigation/Menus';
import logo from '../../assets/logo6432.png'
import logo256 from '../../assets/logo.png'

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
			width: 200,
			[theme.breakpoints.down('xs')]: {
				marginTop: -12,
				width: 140
			}
		}
	}
});

const Welcome = (props) => {

	var classes = useStyles();
	const isAuthenticated = useSelector((state) => state.auth.token !== null)
	const homeMenu = useSelector((state) => state.menus.home)
	const settings = useSelector((state) => state.settings)

	const { version, Plugin, Logo } = settings

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
						<img className={classes.logo} src={Plugin.CompanyLogo || logo256} alt="" style={{ marginRight: 16 }} />
						<Typography variant="h3" style={{ color: "lightgray", paddingTop: 1 }}>{Plugin.ProgramName || 'API'}</Typography>
					</Toolbar>
				</Grid>
				<Box pt={1}></Box>
				<Grid item>
					<Toolbar>
						<Typography variant="h6" style={{ color: "white" }}>
							<Tr>{Plugin.Description || 'Access points to the background workers'}</Tr>
						</Typography>
						<Typography variant="body2" style={{ color: "lightgray", paddingLeft: 8 }}>v{version}</Typography>
					</Toolbar>
				</Grid>
				<Box flexGrow="1"></Box>
				<Grid item style={{ width: "100%" }}>
					<Toolbar>
						<HomeMenu menu={homeMenu} isAuthenticated={isAuthenticated} />
					</Toolbar>
					<Toolbar>
						<Box flexGrow="1"></Box>
						<Typography variant="caption" style={{ color: "lightgray", display: "flex", alignItems: "center" }}>
							<span>Powered by &nbsp;</span>
							<img style={{ marginLeft: 8 }} src={Logo || logo} alt="" />
						</Typography>
					</Toolbar>
				</Grid>
				<Box pt={8}></Box>
			</Grid>
		</Container>
	</Box>
}

export default Welcome