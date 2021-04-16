import React, { useState, useEffect } from 'react'
import { connect, useSelector } from 'react-redux'
import { Link as RouteLink, useHistory, useLocation } from "react-router-dom";
import { Grid, Button, Link, Box, FormControlLabel, Switch } from "@material-ui/core";
// import ReCAPTCHA from "react-google-recaptcha";
import * as actions from '../../store/actions'
import withSnack from '../../store/snack'
import { useTranslation, Tr, Trans } from '../../store/i18next'
import FormWrapper from '../../components/UI/FormContainer';
import TextField from '../../components/UI/TextField';
import AccountCircleTwoTone from '@material-ui/icons/AccountCircleTwoTone'

const development = (process.env && process.env.NODE_ENV === "development")

function useQuery() {
	return new URLSearchParams(useLocation().search);
}
const Login = (props) => {
	const { loading, isAuthenticated, error } = props
	const { snack } = props
	const { AppId } = useSelector((state) => state.settings)
	let history = useHistory();
	let location = useLocation();
	let query = useQuery();
	let { t } = useTranslation();

	//console.write("[Login].from", query.get("redirect"), location.state)

	let redirect = query.get("redirect")
	let { from } = location.state || { from: { pathname: redirect || "/" } }

	useEffect(() => {
		if (isAuthenticated) {
			history.replace(from);
		}
	})

	// useEffect(() => {
	//     if (from.pathname !== '/') {
	//         onSetAuthRedirectPath(from.pathname);
	//     }
	// }, [onSetAuthRedirectPath, from.pathname])

	const [values, setValues] = useState({
		valid: true, //development,
		remember: true,
		email: '',
		password: ''
	});

	useEffect(() => {
		if (error) {
			snack.error(error);
		}
	}, [snack, error])

	const loginHandler = (event) => {
		event.preventDefault();
		props.onAuth(values.email, values.password, AppId).then((response) => {
			if (response.Success) {
				if (response.Resource.IsInitialPassword) {
					history.replace("/auth/changePassword/" + response.Resource.PasswordConfirmationCode);
				}
			}
		})
	}

	function usernameHandler(event) {
		setValues({
			...values,
			email: event.target.value
		})
	}

	function passwordHandler(event) {
		setValues({
			...values,
			password: event.target.value
		})
	}

	function recaptchaHandler(value) {
		setValues({
			...values,
			valid: value !== null
		})
	}

	const rememberHandler = name => event => {
		setValues({ ...values, remember: event.target.checked });
	};

	return (<>{isAuthenticated
		? <Grid container alignItems="center" justify="center" style={{ height: 768 }}>
			<Grid item>
				Yönlendiriliyor...
			</Grid>
		</Grid>
		: <FormWrapper title={t('Login')} icon={AccountCircleTwoTone} comment={t("Type e-mail and password")} loading={loading}>

			<form noValidate onSubmit={loginHandler}>
				<TextField onChange={usernameHandler} name="email" id="email" label={t("EmailAddress")} autoComplete="email" required disabled={loading} />
				<TextField onChange={passwordHandler} name="password" id="password" label={t("Password")} type="password" autoComplete="password" required disabled={loading} />

				<Box ml={0} mt={1}>
					<FormControlLabel label={t("RememberMe")} disabled={loading}
						control={
							<Switch checked={values.remember} onChange={rememberHandler()} color="primary" inputProps={{ "aria-label": "primary checkbox" }} />
						} />
				</Box>
				{/* {development
                    ? null
                    : <Box ml={0} mt={2} mb={3}>
                        <ReCAPTCHA className="ReCAPTCHA"
                            sitekey="6Lf16xkTAAAAAIQuyURRLboyKIz2idSiWmAc0HyD"
                            onChange={recaptchaHandler} />
                    </Box>} */}


				<Box ml={0} mt={2} mb={3}>
					<Button type="submit" fullWidth variant="contained" color="primary" disabled={!values.valid || loading}>
						<Trans>Login</Trans>
					</Button>
				</Box>
				{/* <Grid container>
                    <Grid item xs>
                        <Link to="/auth/passwordRecovery" variant="body1" component={RouteLink}>
                            Şifreni mi unuttun?
						</Link>
                    </Grid>
                    <Grid item>
                        Hesabın yok mu? Şimdi&nbsp;
						<Link to="/auth/register" variant="body1" component={RouteLink}>
                            <b>kayıt ol</b>
                        </Link>
                    </Grid>
                </Grid> */}
			</form>
		</FormWrapper>
	}
	</>)
}


const mapStateToProps = state => {
	return {
		loading: state.auth.loading,
		error: state.auth.error,
		isAuthenticated: state.auth.token !== null,
		passwordConfirmationCode: state.auth.passwordConfirmationCode
	};
};

const mapDispatchToProps = dispatch => {
	return {
		onAuth: (email, password, key) => dispatch(actions.auth(email, password, key)),
		onSetAuthRedirectPath: (path) => dispatch(actions.setAuthRedirectPath(path))
	};
};

export default connect(mapStateToProps, mapDispatchToProps)(withSnack(Login))