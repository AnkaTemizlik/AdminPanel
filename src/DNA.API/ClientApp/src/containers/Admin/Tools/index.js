import React, { useEffect } from "react";
import { connect, useDispatch, useSelector } from "react-redux";
import { Switch, Route, useRouteMatch, useHistory } from "react-router-dom";
import { getSettings } from '../../../store/slices/settingsSlice'
import Container from "../../../components/Container";
import JsonToCSharp from "./JsonToCSharp";

const Tools = (props) => {
	let dispatch = useDispatch();
	let { path, url } = useRouteMatch();
	const { appsettings } = useSelector((state) => state)
	const { menus } = useSelector((state) => state)
	const isAuthenticated = useSelector((state) => state.auth.token !== null)
	let toolsMenu = menus ? menus.tools || {} : {};
	let history = useHistory();

	console.info("Tools", url)
	useEffect(() => {
		if (isAuthenticated)
			dispatch(getSettings())
	}, [dispatch, isAuthenticated]);

	return (
		<Container>
			<Switch>
				<Route path={`${url}/json-to-c-sharp`}>
					<JsonToCSharp />
				</Route>
				<Route path={`${url}`} exact>
					Select a tool
				</Route>
			</Switch>
		</Container>
	);
};

export default Tools;
