import React, { useEffect } from "react";
import { connect, useDispatch, useSelector } from "react-redux";
import { Switch, Link, useRouteMatch, useHistory } from "react-router-dom";
import { CssBaseline, Box, IconButton, Toolbar } from "@material-ui/core";
import BusinessCenter from "@material-ui/icons/BusinessCenter";
import { getSettings } from '../../store/slices/settingsSlice'
import AdminRoute from "../../components/Route/AdminRoute";
import Footer from "../../components/Navigation/Footer";
import Plugin from '../../plugins';
import Management from './Management/Management';
import Dashboard from './Dashboard';
import Layout from "../../components/Layout";
import NavigateBeforeIcon from "@material-ui/icons/NavigateBeforeTwoTone";
import HomeIcon from "@material-ui/icons/HomeTwoTone";
import * as Treasury from "@mui-treasury/layout";
import { HomeSidebar, SidebarTrigger } from "../../components/Navigation/Sidebars";
import Container from "../../components/Container";
import SidebarMenu from "../../components/UI/Menu/SidebarMenu";
import Users from "./Users";
import Statistics from "./Statistics";
import Settings from "./Settings";
import Logs from "./Logs";
import UserEdit from "./UserEdit";
import Tools from "./Tools";

const config = {
	xs: Treasury.getDefaultScreenConfig({
		sidebar: {
			variant: "temporary", // ["permanent","persistent","temporary"]
			width: 300,
		},
		secondarySidebar: {
			variant: "temporary",
			width: 300,
			collapsible: false,
		},
	}),
	md: Treasury.getDefaultScreenConfig({
		sidebar: {
			variant: "permanent",
		},
		secondarySidebar: {
			variant: "temporary",
			collapsible: false,
			width: 300,
		},
		header: {
			position: "fixed", //["absolute","fixed","relative","static","sticky"]
			clipped: true,
		},
	}),
};

const Admin = (props) => {
	let dispatch = useDispatch();
	let { path, url } = useRouteMatch();
	const { appsettings } = useSelector((state) => state)
	const { menus } = useSelector((state) => state)
	const isAuthenticated = useSelector((state) => state.auth.token !== null)
	let adminMenu = menus ? menus.admin || {} : {};
	let history = useHistory();
	const components = {
		Management: Management,
		Definitions: Plugin.Definitions,
		Statistics: Statistics,
		Dashboard: Dashboard,
		"user-management": Users,
	};

	useEffect(() => {
		if (isAuthenticated)
			dispatch(getSettings())
	}, [dispatch, isAuthenticated]);

	return (
		<Layout config={config}>
			{({ headerStyles, sidebarStyles, footerStyles, setOpened, setSecondaryOpened }) => (
				<>
					<CssBaseline />

					<Treasury.Header color="primary">
						<Toolbar>
							<SidebarTrigger />

							<IconButton onClick={() => history.goBack()} color="inherit">
								<NavigateBeforeIcon />
							</IconButton>

							<IconButton component={Link} to="/" color="inherit">
								<HomeIcon />
							</IconButton>

							<IconButton component={Link} to={path} color="inherit">
								<BusinessCenter />
							</IconButton>

							<Box flexGrow="1" />

							<SidebarTrigger secondary />
						</Toolbar>
					</Treasury.Header>

					{menus ? (
						<Treasury.Sidebar>
							<SidebarMenu setOpened={setOpened} menu={adminMenu} />
						</Treasury.Sidebar>
					) : null}

					{menus ?
						<HomeSidebar menus={menus} setOpened={setSecondaryOpened} user={props.user} />
						: null}

					<Treasury.Content>
						<Container>
							<Switch>
								{adminMenu.menus &&
									adminMenu.menus.map((menu, i) => {
										const SpecificPage = components[menu.name];
										if (SpecificPage)
											return (
												<AdminRoute key={i} path={`${url}${menu.to}`}>
													<SpecificPage menu={menu} />
												</AdminRoute>
											);
										else
											return null
									})}

								{appsettings.configs.length > 0 &&
									<AdminRoute path={`${url}/settings/:configId`}>
										<Settings />
									</AdminRoute>
								}

								<AdminRoute path={`${url}/Logs`}>
									<Logs />
								</AdminRoute>

								<AdminRoute path={`${url}/users/new`}>
									<UserEdit me={props.user} />
								</AdminRoute>

								<AdminRoute path={`${url}/users/:id`}>
									<UserEdit me={props.user} />
								</AdminRoute>

								<AdminRoute path={`${url}/users`}>
									<Users />
								</AdminRoute>

								<AdminRoute path={`${url}/tools`}>
									<Tools />
								</AdminRoute>

								<AdminRoute path={`${url}`}>
									<Dashboard menu={adminMenu} />
								</AdminRoute>

							</Switch>
						</Container>
					</Treasury.Content>

					<Treasury.Footer>
						<Footer menus={menus} />
					</Treasury.Footer>
				</>
			)}
		</Layout>
	);
};

const mapStateToProps = (state) => ({
	user: state.auth.user,
	isAuthenticated: state.auth.token != null,
	//menus: state.auth.menus,
});

export default connect(mapStateToProps)(Admin);
