
import React from "react";
import { Button, Box, Typography, FormControl } from "@material-ui/core";
import VpnKeyTwoTone from "@material-ui/icons/VpnKeyTwoTone";
import FormWrapper from "../../components/UI/FormContainer";

const RecoveryConfirm = () => {

	return (
		<FormWrapper title="Şifre Sıfırlama"
			icon={VpnKeyTwoTone}
			comment="">
			<Box mt={3}>
				<Typography gutterBottom>
					Size ait olduğundan emin olmak için adresine bir e-posta gönderdik. Lütfen gelen kutusunu kontrol edin ve yönergeleri takip edin. </Typography>
			</Box>
			<Box mt={2}>
				<Typography variant="body2" gutterBottom>
					Gelen kutunuzda e-postayı göremiyorsanız 'Spam' klasörünü kontrol edin.</Typography>
			</Box>
			<Box mt={6} style={{ minHeight: 200 }}>
				<FormControl fullWidth margin="normal">
					<Button type="submit" variant="contained" disabled color="primary" style={{ textTransform: 'none' }}>
						Şifre Oluşturma e-postasını tekrar gönder. </Button>
				</FormControl>
			</Box>
		</FormWrapper>
	);
}

export default RecoveryConfirm
