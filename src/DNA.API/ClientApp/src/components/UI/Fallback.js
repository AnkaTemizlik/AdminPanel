import React from 'react'
import { Backdrop, Box, CircularProgress, Icon, IconButton, Paper, Tooltip, Zoom } from '@material-ui/core'
//import { useBackendStatus } from "../../store/i18n";
import { Alert, AlertTitle } from '@material-ui/lab';

const Fallback = (props) => {
	//const { status } = useBackendStatus();
	console.warning("Fallback", props)
	// if (props.error) {
	// 	return <div>Error!
	// 		<span>{JSON.stringify(props.error)}</span>
	// 		<button onClick={props.retry}>Retry</button>
	// 	</div>;
	// } else if (props.pastDelay) {
	// 	return <div>Loading...</div>;
	// } else {
	// 	return null;
	// }

	return <Backdrop open>
		<Paper>
			<Box pt={1} pb={1} pl={2} pr={2} display="flex" alignItems="center">
				<Box p={2}>
					{props.error
						? <>
							<Alert severity="error" variant="outlined" style={{ minWidth: 240 }}
								action={
									<Tooltip title="refresh">
										<span>
											<IconButton color="inherit" size="small" onClick={props.retry}>
												<Icon>refresh</Icon>
											</IconButton>
										</span>
									</Tooltip>
								}>
								<AlertTitle>Error </AlertTitle>
								{typeof props.error == 'object' ? props.error.message : props.error}
							</Alert>
						</>
						: <CircularProgress />
					}
				</Box>

				{/* <Zoom direction="right" in={status && status.Success == false} mountOnEnter unmountOnExit>

					<Box p={2} style={{ minWidth: 350 }}>
						{status &&
							<Alert severity="error" variant="outlined">
								<AlertTitle>Loading </AlertTitle>
								{status.Message}
								<i>{status.tryCount && `. Try count: ${Math.ceil((status.tryCount) / 2)}`}</i>
							</Alert>
						}
					</Box>
				</Zoom> */}

			</Box>
		</Paper>
	</Backdrop>
}

export default Fallback