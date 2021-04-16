import React from 'react'
import { connect, useSelector } from 'react-redux'
import { Switch, Route, Link } from 'react-router-dom'
import { CssBaseline, Hidden, Box, IconButton } from '@material-ui/core'
import * as Treasury from "@mui-treasury/layout";
import Layout from '../../components/Layout';
import Header from '../../components/Navigation/Header'
import HomeMenu from '../../components/Navigation/Menus/HomeMenu'
import { HomeSidebar, SidebarTrigger } from '../../components/Navigation/Sidebars'
import Footer from '../../components/Navigation/Footer';
import HomeIcon from '@material-ui/icons/HomeTwoTone';
import Login from './Login'
import Register from './Register'
import Settings from './Settings'
import Account from './Account'
import Confirm from './Confirm'
import Logout from './Logout'
import { SocialMenu } from '../../components/Navigation/Menus';
import Recovery from './Recovery';
import ChangePassword from './ChangePassword';
import RecoveryConfirm from './RecoveryConfirm';
import LanguageChanger from '../../components/UI/LanguageChanger';

const config = {
	xs: Treasury.getDefaultScreenConfig({
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		}
	})
}

const Auth = (props) => {

	const menus = useSelector((state) => state.menus)

	return <Layout config={config}>
		{({ setSecondaryOpened }) => {
			return (<>
				<CssBaseline />
				<Header position="absolute">

					<IconButton component={Link} to="/" color="inherit">
						<HomeIcon />
					</IconButton>

					<Hidden xsDown>
						<HomeMenu menu={menus.home} isAuthenticated={props.isAuthenticated} />
					</Hidden>

					<Box flexGrow="1" />

					<Hidden smDown>
						<SocialMenu minimal />
					</Hidden>


					<LanguageChanger />
					{/* {props.isAuthenticated
                        ? null
                        : <AuthButton isAuthenticated={props.isAuthenticated} />} */}

					<SidebarTrigger secondary />

				</Header>

				{menus
					? <HomeSidebar setOpened={setSecondaryOpened} menus={menus} user={props.user} />
					: null}

				<Treasury.Content>
					<Switch>

						<Route path="/auth/register" component={Register} />
						<Route path="/auth/confirm/:code?" component={Confirm} />
						<Route path="/auth/login" component={Login} />
						<Route path="/auth/passwordRecovery" component={Recovery} />
						<Route path="/auth/recoveryConfirm" component={RecoveryConfirm} />
						<Route path="/auth/changePassword/:code?" component={ChangePassword} />

						<Route path="/auth/settings"><Settings /></Route>
						<Route path="/auth/account"><Account /></Route>
						<Route path="/auth/logout"><Logout /></Route>

					</Switch>
				</Treasury.Content>

				<Treasury.Footer>
					<Footer menus={menus} />
				</Treasury.Footer>
			</>)
		}}
	</Layout>
}

const mapStateToProps = state => {
	return {
		isAuthenticated: state.auth.token != null,
		user: state.auth.user
	}
}

export default connect(mapStateToProps)(Auth)