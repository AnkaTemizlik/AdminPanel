import React from 'react'
import { Backdrop, Box, CircularProgress, Paper, Zoom } from '@material-ui/core'
import { useBackendStatus } from "../../store/i18n";
import { Alert, AlertTitle } from '@material-ui/lab';

const Fallback = (props) => {
	const { status } = useBackendStatus();
	return <Backdrop open>
		<Paper>
			<Box pt={1} pb={1} pl={2} pr={2} display="flex" alignItems="center">
				<Box p={2}>
					<CircularProgress />
				</Box>

				<Zoom direction="right" in={status && status.Success == false} mountOnEnter unmountOnExit>

					<Box p={2} style={{ minWidth: 350 }}>
						{status &&
							<Alert severity="error" variant="outlined">
								<AlertTitle>Loading </AlertTitle>
								{status.Message}
								<i>{status.tryCount && `. Try count: ${Math.ceil((status.tryCount) / 2)}`}</i>
							</Alert>
						}
					</Box>
				</Zoom>

			</Box>
		</Paper>
	</Backdrop>
}

export default Fallback