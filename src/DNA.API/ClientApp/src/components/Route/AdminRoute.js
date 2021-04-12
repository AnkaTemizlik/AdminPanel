import React from "react";
import { connect } from "react-redux";
import { Route, Redirect } from "react-router-dom";

function AdminRoute(props) {
	const { isAdmin, isAuthenticated, children, ...rest } = props;
	return (
		<Route
			{...rest}
			render={({ location }) =>
				isAdmin && isAuthenticated ? (
					children
				) : (
						<Redirect
							to={{
								pathname: isAuthenticated ? "/" : "/auth/login",
								state: { from: location },
							}}
						/>
					)
			}
		/>
	);
}

const mapStateToProps = (state) => ({
	isAdmin: state.auth.user && state.auth.user.Roles.includes("Admin"),
	isAuthenticated: state.auth.token != null,
});

export default connect(mapStateToProps, null)(AdminRoute);
