import React, { useEffect } from "react";
import { connect } from 'react-redux'
// import ReCAPTCHA from "react-google-recaptcha";
import Alert from '@material-ui/lab/Alert';
import { FormControl, Button, Typography, Divider, FormGroup, Checkbox, FormControlLabel, Link } from "@material-ui/core";
import * as actions from '../../store/actions'
import AccountCircleIcon from "@material-ui/icons/AccountCircleTwoTone";
import FormWrapper from "../../components/UI/FormContainer";
import TextField from "../../components/UI/TextField";

const comment = "E-posta adresinizi girin ve bir parola belirleyin.";
const development = (process.env && process.env.NODE_ENV === "development")

const Register = ({ isAuthenticated, onRegister, loading, error, resetError, history }) => {
	const [email, setEmail] = React.useState(null)
	const [name, setName] = React.useState(null)
	const [password, setPassword] = React.useState(null)
	const [passwordConfirm, setPasswordConfirm] = React.useState(null)
	const [isPasswordValid, setIsPasswordValid] = React.useState(true)
	const [isEmailValid, setIsEmailValid] = React.useState(true)
	const [isNameValid, setIsNameValid] = React.useState(true)
	const [recaptcha, setRecaptcha] = React.useState(null)
	const [kvkk, setKvkk] = React.useState(false)
	const [kvkkError, setKvkkError] = React.useState(false)

	const handleRegister = event => {
		event.preventDefault();
		if (!kvkk) {
			setKvkkError(true)
		}
		else {
			if (error)
				resetError()

			onRegister({ email, branch: 1, password, passwordConfirm, recaptcha, fullName: name })
				.then((status) => {
					if (status.Success)
						history.push('/auth/confirm');
				})
		}
	};
	const handleKvkk = e => {
		var checked = e.target.checked
		if (checked)
			setKvkkError(false)
		setKvkk(checked)
	}
	function checkEmail() {
		const isOk = email == null || /^[^<>\s@]+(@[^<>\s@]+(\.[^<>\s@]+)+)$/.test(email);
		setIsEmailValid(isOk)
	}
	function checkName() {
		const isOk = name == null || name.length > 6;
		setIsNameValid(isOk)
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
		return password !== null && email !== null && isPasswordValid && password === passwordConfirm // && (development || recaptcha != null) 
			&& isEmailValid
	}

	useEffect(() => {
		if (isAuthenticated) {
			history.replace({ from: { pathname: "/" } });
		}
	})

	return (
		<FormWrapper title="Kayıt" icon={AccountCircleIcon} comment={comment} loading={loading}>

			{error &&
				<Alert variant="outlined" severity="error">
					{/* <AlertTitle>Hata</AlertTitle> */}
					{error}
				</Alert>
			}

			<form noValidate onSubmit={handleRegister} >

				<TextField size="medium" id="nameForRegister"
					label="Adınız ve Soyadınız "
					name="nameForRegister"
					onChange={(e) => setName(e.target.value)}
					type="text"
					onBlur={(e) => checkName(e)}
					required autoFocus disabled={loading}
					error={!isNameValid}
					helperText={isNameValid ? null : "Adınızı ve soyadınızı girin."}
				/>

				<TextField size="medium" id="emailForRegister"
					label="E-posta Adresi "
					name="emailForRegister"
					onChange={(e) => setEmail(e.target.value)}
					onBlur={(e) => checkEmail()}
					required autoFocus disabled={loading}
					error={!isEmailValid}
					helperText={isEmailValid ? null : "Geçerli bir e-posta adresi girin."}
				/>

				<Divider />
				<TextField size="medium" id="passwordForRegister" label="Şifre " name="passwordForRegister" type="password"
					onChange={(e) => handlePassword(e.target.value)}
					onBlur={(e) => checkPassword()}
					required
					disabled={loading}
					error={!isPasswordValid}
					helperText={isPasswordValid ? null : "Şifreniz en az 6 karakter, büyük harf, küçük harf ve rakam içermeli"} />

				<TextField size="medium" id="passwordConfirmForRegister" label="Şifre Tekrar " name="passwordConfirmForRegister" type="password"
					onChange={(e) => setPasswordConfirm(e.target.value)}
					required
					disabled={loading}
					error={!checkPasswordConfirm()} />

				{/* {development
                    ? null
                    : <FormControl variant="filled" fullWidth margin="normal"  >
                        <ReCAPTCHA className="ReCAPTCHA"
                            sitekey="6Lf16xkTAAAAAIQuyURRLboyKIz2idSiWmAc0HyD"
                            onChange={(v) => recaptchaHandler(v)}
                            data-theme="dark" />
                    </FormControl>} */}

				<FormControl variant="filled" fullWidth margin="normal"  >
					<Typography variant="body2">
						Bu bilgilerinizi daha sonra panelinizde de görebilir ve dilerseniz değiştirebilirsiniz.
                    </Typography>
				</FormControl>

				<FormControl required error={true} component="fieldset">
					<FormGroup>
						<FormControlLabel
							control={
								<Checkbox style={{ color: kvkkError ? "red" : 'unset' }} onChange={handleKvkk} name="kvkk" />
							}
							label={
								<Typography color={kvkkError ? "error" : "initial"} variant="body1"><Link href="/kvkk-aydinlatma" target="_blank">KVKK Aydınlatma</Link> metnini okudum</Typography>
							} />
					</FormGroup>
				</FormControl>

				<FormControl fullWidth margin="normal">
					<Button type="submit" variant="contained" color="secondary" disabled={!isValid() || loading}>
						Kayıt Ol
					</Button>
				</FormControl>

			</form>
		</FormWrapper>
	);
}
const mapStateToProps = state => {
	return {
		loading: state.auth.loading,
		error: state.auth.error,
		isAuthenticated: state.auth.token !== null
	};
};
const mapDispatchToProps = dispatch => {
	return {
		onRegister: (data) => dispatch(actions.register(data)),
		resetError: () => dispatch(actions.reset())
	};
};
export default connect(mapStateToProps, mapDispatchToProps)(Register)
