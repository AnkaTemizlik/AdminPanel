import React from 'react'
import { connect } from 'react-redux'
import { Route, Switch } from 'react-router-dom'
import { CssBaseline, Box } from '@material-ui/core';

import * as Treasury from "@mui-treasury/layout";
import Header from '../../components/Navigation/Header'
//import Footer from '../../components/Navigation/Footer'
import Layout from '../../components/Layout'
import { HomeSidebar, SidebarTrigger } from '../../components/Navigation/Sidebars/index';
import AuthButton from '../../components/Navigation/AuthButton';

import Main from './Main'
import About from './About'

import Plugin from '../../plugins'
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
	console.write("[Home]", props)
	console.write("[Home].Plugin", Plugin)

	return <Layout config={config} >
		{(args) => {
			const { footerStyles, setSecondaryOpened } = args
			return (<>
				<CssBaseline />

				<Header style={{ backgroundColor: "rgba(0, 0, 0, 0.3)" }}>

					<Box flexGrow="1" />

					<h6 style={{ color: "gray" }}>{Plugin.Name}</h6>
					
					<Box pl={2}>
						<LanguageChanger />
					</Box>

					{props.menus ?
						(props.isAuthenticated 
							? null
							: <AuthButton menu={props.menus.user} isAuthenticated={props.isAuthenticated} />)
						: null}

					<SidebarTrigger secondary />

				</Header>

				{props.menus
					? <HomeSidebar setOpened={setSecondaryOpened} menus={props.menus} user={props.user} />
					: null}

				<div>
					<Switch>
						<Route path="/about" exact component={About} />
						<Route path="/" component={Main} />
					</Switch>
				</div>
				{/* 
                <Treasury.Footer className={footerStyles.primaryBorder}>
                    <Footer menus={props.menus}/>
                </Treasury.Footer> */}
			</>)
		}}
	</Layout >
}

const mapStateToProps = (state) => ({
	user: state.auth.user,
	isAuthenticated: state.auth.token != null,
	menus: state.auth.menus
})

export default connect(mapStateToProps)(Home)