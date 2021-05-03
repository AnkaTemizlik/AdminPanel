import React, { useEffect } from "react";
import { connect } from 'react-redux'
// import ReCAPTCHA from "react-google-recaptcha";
import Alert from '@material-ui/lab/Alert';
import { useParams } from "react-router-dom";
import { FormControl, Button, Typography, Box } from "@material-ui/core";
import * as actions from '../../store/actions'
import AccountCircleIcon from "@material-ui/icons/AccountCircleTwoTone";
import FormWrapper from "../../components/UI/FormContainer";
import TextField from "../../components/UI/TextField";
import { useTranslation } from '../../store/i18next'

const development = (process.env && process.env.NODE_ENV === "development")

const ChangePassword = ({ onChangePassword, registeredEmail, loading, error, resetError, history }) => {

	let { code } = useParams();
	let { t } = useTranslation();

	const [email, setEmail] = React.useState(registeredEmail || null)
	const [isEmailValid, setIsEmailValid] = React.useState(true)
	const [password, setPassword] = React.useState(null)
	const [passwordConfirm, setPasswordConfirm] = React.useState(null)
	const [isPasswordValid, setIsPasswordValid] = React.useState(true)
	const [recaptcha, setRecaptcha] = React.useState(null)
	const [successMessage, setSuccessMessage] = React.useState(null)

	const handleRegister = event => {
		event.preventDefault();
		if (error)
			resetError()

		onChangePassword({ email, password, passwordConfirm, recaptcha, code })
			.then((status) => {
				if (status.Success) {
					setSuccessMessage(`${t("Password saved. Redirecting")}`)
					setTimeout(() => {
						history.push('/auth/login');
					}, 2000)
				}
			})
	};
	function checkEmail() {
		const isOk = email == null || /^[^<>\s@]+(@[^<>\s@]+(\.[^<>\s@]+)+)$/.test(email);
		setIsEmailValid(isOk)
	}
	function handlePassword(pass) {
		if (pass.length >= 6)
			checkPassword()
		setPassword(pass)
	}
	function checkPassword() {
		const isOk = password == null || /^((?=\S*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9]).{6,})\S$/.test(password);
		setIsPasswordValid(isOk)
	}
	function checkPasswordConfirm() {
		return password === null || passwordConfirm === null || !isPasswordValid || password === passwordConfirm
	}
	function recaptchaHandler(value) {
		if (value !== recaptcha)
			setRecaptcha(value)
	}

	const isValid = () => {
		return password !== null && email !== null && isPasswordValid && password === passwordConfirm && isEmailValid
	}
	useEffect(() => {

	}, [])

	return (
		<FormWrapper title={t("New Password")}
			icon={AccountCircleIcon}
			comment={t("Type your e-mail address and new password")}
			loading={loading}>

			{error &&
				<Alert variant="outlined" severity="error">
					{t(error)}
				</Alert>
			}

			<Typography color="secondary" align="center" variant="h6">
				{
					loading
						? t(`Checking, please wait`)
						: t(error)
							? ``
							: t(successMessage)
				}
			</Typography>

			{successMessage ? null
				: <form noValidate onSubmit={handleRegister}>
					<Box mt={4}>
						<TextField size="medium"
							id="emailForRegister"
							label={t("Email")}
							name="emailForRegister"
							onChange={(e) => setEmail(e.target.value)}
							onBlur={(e) => checkEmail()}
							required autoFocus disabled={loading}
							error={!isEmailValid}
							helperText={isEmailValid ? null : t("Type a valid e-mail address")}
						/>

						<TextField size="medium"
							id="passwordForRegister"
							label={t("Password")}
							name="passwordForRegister"
							type="password"
							onChange={(e) => handlePassword(e.target.value)}
							onBlur={(e) => checkPassword()}
							required
							disabled={loading}
							error={!isPasswordValid}
							helperText={isPasswordValid ? null : t("Password length must atleast 8 char. Its includes atleast one uppercase and lowercase letter. Its must have number in your password")} />

						<TextField size="medium"
							id="passwordConfirmForRegister"
							label={t("Password Confirm")}
							name="passwordConfirmForRegister"
							type="password"
							onChange={(e) => setPasswordConfirm(e.target.value)}
							required
							disabled={loading}
							error={!checkPasswordConfirm()} />
					</Box>
					{/* {development
							? null
							: <FormControl variant="filled" fullWidth margin="normal"  >
									<ReCAPTCHA className="ReCAPTCHA"
											sitekey="6Lf16xkTAAAAAIQuyURRLboyKIz2idSiWmAc0HyD"
											onChange={(v) => recaptchaHandler(v)}
											data-theme="dark" />
							</FormControl>} */}

					<Box mt={6} style={{ display: "flex", justifyContent: "flex-end" }}>
						<FormControl margin="normal">
							<Button type="submit" variant="contained" color="secondary" disabled={!isValid() || loading}>
								{t("Save")}
							</Button>
						</FormControl>
					</Box>
				</form>
			}
		</FormWrapper >
	)
}
const mapStateToProps = state => {
	return {
		loading: state.auth.loading,
		error: state.auth.error,
	};
};
const mapDispatchToProps = dispatch => {
	return {
		onChangePassword: (data) => dispatch(actions.changePassword(data)),
		resetError: () => dispatch(actions.reset())
	};
};

export default connect(mapStateToProps, mapDispatchToProps)(ChangePassword);