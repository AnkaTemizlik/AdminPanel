import React from "react";
import { makeStyles } from "@material-ui/core/styles";
import Modal from "@material-ui/core/Modal";
import Paper from "@material-ui/core/Paper";
import Backdrop from "@material-ui/core/Backdrop";
import { Typography, Divider, Box, Button, Grid, LinearProgress } from "@material-ui/core";
import { Trans } from "../../store/i18next";

const useStyles = makeStyles((theme) => ({
	paper: {
		//position: 'absolute',
		padding: theme.spacing(2, 4, 2),
		outline: "none",
		margin: theme.spacing(0, 1),
	},
	container: {
		margin: theme.spacing(3, 0, 3),
	},
	divider: {
		margin: theme.spacing(2, -4),
	},
	linearProgress: {
		display: 'absolute',

	}
}));

const ModalComponent = ({ children, open, onClose, title, ok, cancel, okText, cancelText, params, loading = false }) => {

	const classes = useStyles();

	return (
		<Modal
			open={open}
			onClose={onClose}
			// aria-labelledby="simple-modal-title"
			// aria-describedby="simple-modal-description"
			closeAfterTransition
			// //disableScrollLock
			BackdropComponent={Backdrop}
			BackdropProps={{
				timeout: 300,
			}}
			style={{
				display: "flex",
				justifyContent: "center",
				alignItems: "center",
			}}
		>
			{/* <Box display="flex" alignItems="center" justifyContent="center" style={{ height: "100vh", width: "100%", outline: "none", }}> */}
			<Grid item xs={12} sm={9} md={6} lg={5} style={{ outline: "none" }}>
				<Paper className={classes.paper} elevation={8}>
					<Typography variant="h6">
						<Trans>{title}</Trans>
					</Typography>

					<Divider className={classes.divider} />

					<Box mt={-2} style={{ height: 4 }}>{
						loading &&
						<LinearProgress color="secondary" />}</Box>

					<div className={classes.container}>{children}</div>

					<Divider className={classes.divider} />

					<Box display="flex">
						{cancelText && (
							<Button variant="outlined" onClick={() => cancel({ ...params, open: false })} disabled={loading}>
								<Trans>{cancelText}</Trans>
							</Button>
						)}
						<div style={{ flexGrow: 1 }}></div>
						{okText && (
							<Button variant="contained" color="primary" onClick={() => ok({ ...params, open: false })} disabled={loading}>
								<Trans>{okText}</Trans>
							</Button>
						)}
					</Box>
				</Paper>
			</Grid>
			{/* </Box> */}
		</Modal>
	);
};

export default ModalComponent