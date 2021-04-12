
import React, { useEffect } from "react";
import { connect } from 'react-redux'
import { useParams, Redirect } from "react-router-dom";
import Alert from '@material-ui/lab/Alert';
import { Button, Box, Typography, FormControl } from "@material-ui/core";
import MailOutline from "@material-ui/icons/MailOutline";
import * as actions from '../../store/actions'
import FormWrapper from "../../components/UI/FormContainer";
import { Trans, Tr, useTranslation } from '../../store/i18next'

const Confirm = (props) => {
	let { t } = useTranslation();
	const { loading, registered, onConfirm, error, history } = props
	const { email } = registered
	let { code } = useParams();

	useEffect(() => {
		if (code) {
			onConfirm({ code })
				.then((status) => {
					if (status.Success) {
						if (status.Resource.IsInitialPassword) {
							setTimeout(() => {
								history.push('/auth/changePassword/' + status.Resource.PasswordConfirmationCode);
							}, 2000)
						}
						else {
							setTimeout(() => {
								history.push('/auth/login');
							}, 2000)
						}
					}
				})
		}
	}, [code, history, onConfirm])

	return (
		<FormWrapper title={t("E-mail Confirmation")}
			icon={MailOutline}
			comment=""
			loading={loading}>

			{error || email || code ? null : <Redirect to="/auth/login" />}

			{error &&
				<Alert variant="outlined" severity="error">
					{t(error)}
				</Alert>
			}

			{email
				? <>
					<Box mt={2} >
						<Typography gutterBottom>
							{t("Size ait olduğundan emin olmak için e-posta adresine bir e-posta gönderdik. Lütfen gelen kutusunu kontrol edin")}
						</Typography>
						<Typography variant="body2" gutterBottom>
							{t("Gelen kutunuzda e-postayı göremiyorsanız Spam klasörünü kontrol edin.")}
						</Typography>
					</Box>
					<Box mt={4} style={{ minHeight: 200 }}>
						<FormControl fullWidth margin="normal">
							<Button type="submit" variant="contained" disabled color="secondary" style={{ textTransform: 'none' }}>
								{t("Doğrulama e-postasını tekrar gönder.")}
							</Button>
						</FormControl>
					</Box>
				</> : null}

			{code ?
				<Box mt={8} style={{ minHeight: 100 }}>
					<Typography color="secondary" align="center" variant="h6">
						{
							loading
								? t(`Kontrol ediliyor, lütfen bekleyin...`)
								: t(error) ? `` : t(`Onaylandı, yönlendiriliyor...`)
						}
					</Typography>
				</Box> : null}

		</FormWrapper>
	);
}

const mapStateToProps = state => {
	return {
		loading: state.auth.loading,
		error: state.auth.error,
		registered: state.auth.registered
		//isAuthenticated: state.auth.token !== null
	};
};
const mapDispatchToProps = dispatch => {
	return {
		onConfirm: (data) => dispatch(actions.confirm(data)),
		resetError: () => dispatch(actions.reset())
	};
};

export default connect(mapStateToProps, mapDispatchToProps)(Confirm)
