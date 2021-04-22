import React from 'react'
import { connect, useSelector } from 'react-redux'
import { Route, Switch } from 'react-router-dom'
import { CssBaseline, Box, Typography } from '@material-ui/core';
import * as Treasury from "@mui-treasury/layout";
import Header from '../../components/Navigation/Header'
import Layout from '../../components/Layout'
import { HomeSidebar, SidebarTrigger } from '../../components/Navigation/Sidebars/index';
import AuthButton from '../../components/Navigation/AuthButton';
import Main from './Main'
import About from './About'
import LanguageChanger from '../../components/UI/LanguageChanger';

const config = {
	xs: Treasury.getDefaultScreenConfig({
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		}
	}),
	sm: Treasury.getDefaultScreenConfig({
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		},
		header: {
			position: "fixed"
		}
	})
}

const Home = (props) => {
	const { Plugin } = useSelector((state) => state.settings)
	const menus = useSelector((state) => state.menus)
	return <Layout config={config}>
		{(args) => {
			const { footerStyles, setSecondaryOpened } = args
			return (<>
				<CssBaseline />

				<Header style={{ backgroundColor: "rgba(0, 0, 0, 0.3)" }}>

					<Box flexGrow="1" />

					<Typography variant="body2">{Plugin.CompanyName}</Typography>

					<Box pl={2}>
						<LanguageChanger />
					</Box>

					{menus ?
						(props.isAuthenticated
							? null
							: <AuthButton menu={menus.user} isAuthenticated={props.isAuthenticated} />)
						: null}

					<SidebarTrigger secondary />

				</Header>

				{menus
					? <HomeSidebar setOpened={setSecondaryOpened} menus={menus} user={props.user} />
					: null}

				<div>
					<Switch>
						<Route path="/about" exact component={About} />
						<Route path="/" component={Main} />
					</Switch>
				</div>
			</>
			)
		}}
	</Layout >
}

const mapStateToProps = (state) => ({
	user: state.auth.user,
	isAuthenticated: state.auth.token != null
})

export default connect(mapStateToProps)(Home)