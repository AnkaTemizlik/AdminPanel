import React from "react";
import { connect } from 'react-redux'
import { Route, Redirect } from "react-router-dom";

function PrivateRoute({ isAuthenticated, children, ...rest }) {
    return (
        <Route
            {...rest}
            render={({ location }) =>
                isAuthenticated ? (children) : (
                    <Redirect
                        to={{
                            pathname: "/auth/login",
                            state: { from: location }
                        }}
                    />
                )
            }
        />
    );
}

const mapStateToProps = (state) => ({
    isAuthenticated: state.auth.token != null
})

export default connect(mapStateToProps, null)(PrivateRoute)