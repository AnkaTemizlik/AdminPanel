import React, { useEffect } from 'react';
import { connect } from 'react-redux';
// import ReCAPTCHA from "react-google-recaptcha";
import * as actions from '../../store/actions';
import VpnKeyTwoTone from "@material-ui/icons/VpnKeyTwoTone";
import FormWrapper from "../../components/UI/FormContainer";
import Alert from '@material-ui/lab/Alert';
import { FormControl, Button } from "@material-ui/core";
import TextField from "../../components/UI/TextField";
import { Trans, Tr, useTranslation } from '../../store/i18next'
const development = (process.env && process.env.NODE_ENV === "development")

const Recovery = ({ loading, error, onRecovery, resetError, history }) => {
	let { t } = useTranslation();
	const [email, setEmail] = React.useState(null)
	const [isEmailValid, setIsEmailValid] = React.useState(true)
	const [recaptcha, setRecaptcha] = React.useState(development ? true : null)

	const handleRegister = event => {
		event.preventDefault();
		if (error)
			resetError()

		onRecovery({ email, recaptcha })
			.then((status) => {
				if (status.Success)
					history.push('/auth/recoveryConfirm');
			})
	};
	function checkEmail() {
		const isOk = email == null || /^[^<>\s@]+(@[^<>\s@]+(\.[^<>\s@]+)+)$/.test(email);
		setIsEmailValid(isOk)
	}
	function recaptchaHandler(value) {
		if (value !== recaptcha)
			setRecaptcha(value)
	}
	const isValid = () => {
		return recaptcha != null && isEmailValid && email != null
	}
	useEffect(() => {

	}, [])

	return (
		<FormWrapper title={t("Şifre Sıfırlama")} icon={VpnKeyTwoTone}
			comment={t("Şifreniz için size bir e-posta göndereceğiz. Lütfen sistemde kayıtlı olan e-posta adresinizi girin.")}
			loading={loading}>

			{error &&
				<Alert variant="outlined" severity="error">
					{error}
				</Alert>
			}
			<form noValidate onSubmit={handleRegister} >
				<TextField size="medium" id="emailForRegister"
					label={t("E-posta Adresi")}
					name="emailForRegister"
					onChange={(e) => setEmail(e.target.value)}
					onBlur={(e) => checkEmail()}
					required autoFocus disabled={loading}
					error={!isEmailValid}
					helperText={isEmailValid ? null : t("Geçerli bir e-posta adresi girin.")}
				/>

				{/* {development
                    ? null
                    : <FormControl variant="filled" fullWidth margin="normal" size="small" >
                        <ReCAPTCHA className="ReCAPTCHA"
                            sitekey="6Lf16xkTAAAAAIQuyURRLboyKIz2idSiWmAc0HyD"
                            onChange={(v) => recaptchaHandler(v)}
                            data-theme="dark" />
                    </FormControl>} */}

				<FormControl fullWidth margin="normal">
					<Button type="submit" variant="contained" color="secondary" disabled={!isValid() || loading}>
						{t("Gönder")}
					</Button>
				</FormControl>
			</form>

		</FormWrapper>
	)
}
const mapStateToProps = state => {
	return {
		loading: state.auth.loading,
		error: state.auth.error,
		// isAuthenticated: state.auth.token !== null
	};
};
const mapDispatchToProps = dispatch => {
	return {
		onRecovery: (data) => dispatch(actions.recovery(data)),
		resetError: () => dispatch(actions.reset())
	};
};

export default connect(mapStateToProps, mapDispatchToProps)(Recovery);