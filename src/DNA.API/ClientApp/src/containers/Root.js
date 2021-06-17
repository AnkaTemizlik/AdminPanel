import React, { useEffect } from "react";
import { connect, useDispatch, useSelector } from "react-redux";
import { Route, Switch, Redirect, withRouter } from 'react-router-dom'
import Help from './Help'
import Home from './Home'
import Panel from './Panel'
import Auth from './Auth'
import Admin from './Admin'
import PrivateRoute from '../components/Route/PrivateRoute'
import AdminRoute from '../components/Route/AdminRoute'
import withSnack from "../store/snack";
import * as actions from "../store/actions";
import { Alert, AlertTitle } from "@material-ui/lab";
import { authCheckState } from '../store/slices/authSlice'

const Root = (props) => {
	let dispatch = useDispatch();
	const { onTryAutoSignup } = props
	const settings = useSelector((state) => state.settings)
	const alerts = useSelector((state) => state.alerts)

	useEffect(() => {
		if (alerts.message)
			props.snack.show(alerts.message)
	}, [dispatch, props.snack, alerts.message]);

	useEffect(() => {
		dispatch(authCheckState())
		onTryAutoSignup()
	}, [dispatch, onTryAutoSignup]);

	return <>
		{settings && settings.WarningMessage && settings.WarningMessage.Enabled &&
			<div id="warning-message">
				<Alert style={{ width: 360, border: "1px solid #ddd" }}
					severity={settings.WarningMessage.Severity}>
					{settings.WarningMessage.Title &&
						<AlertTitle>{settings.WarningMessage.Title}</AlertTitle>
					}
					{settings.WarningMessage.Message}
				</Alert>
			</div>
		}

		<Switch>

			<Route path="/view-email/:url/:confirmationCode/:uniqueId" ><Home /></Route>

			<PrivateRoute path="/panel" ><Panel /></PrivateRoute>

			<AdminRoute path="/admin" ><Admin /></AdminRoute>

			<AdminRoute path="/tools" ><Admin /></AdminRoute>

			<Route path="/auth" component={Auth} />

			<Route path="/c/:code" exact
				render={({ match }) => (
					<Redirect to={`/auth/confirm/${match.params.code}`} />
				)}
			/>

			<Route path="/r/:code" exact
				render={({ match }) => (
					<Redirect to={`/auth/changePassword/${match.params.code}`} />
				)}
			/>

			<Route path="/help"><Help /></Route>

			{/* <Route path="/"><Home /></Route> */}

			<Route path="/">
				{settings.Plugin.AuthSettings.GoPanelOnStart == true
					? <Redirect to="/panel" />
					: <Home />
				}
			</Route>

		</Switch>
	</>
}

const mapDispatchToProps = (dispatch) => {
	return {
		onTryAutoSignup: () => dispatch(actions.authCheckState()),
	};
};

export default connect(null, mapDispatchToProps)(withSnack(withRouter(Root)));