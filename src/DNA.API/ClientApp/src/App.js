
import React from "react";
import { withRouter } from "react-router-dom";
import { SnackbarProvider } from "notistack";
import IconButton from "@material-ui/core/IconButton";
import Close from "@material-ui/icons/Close";
import { ThemeProvider } from "@material-ui/styles";
import { Box } from "@material-ui/core";
import Root from "./containers/Root";
import createTheme from "./theme";
import ScrollTop from "./components/UI/ScrollTop";
import Plugin from './plugins'

const App = (props) => {

	window.development = process.env && process.env.NODE_ENV === "development";

	const notistackRef = React.createRef();
	const onSnackbarClickDismiss = (key) => () => {
		notistackRef.current.closeSnackbar(key);
	};
	props.history.listen((location, action) => {
		window.scrollTo(0, 0);
	});

	return (
		<ThemeProvider theme={createTheme()}
		>
			<SnackbarProvider
				ref={notistackRef}
				maxSnack={3}
				anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
				//preventDuplicate
				action={(key) => (
					<IconButton onClick={onSnackbarClickDismiss(key)}>
						<Close />
					</IconButton>
				)}
			>
				<Box height="100vh">
					<span id="back-to-top-anchor" style={{ visibility: "hidden", width: 0, height: 0 }}></span>
					<ScrollTop id="back-to-top-anchor" />
					<Root />
				</Box>
			</SnackbarProvider>
		</ThemeProvider>
	);
};

export default withRouter(App)
