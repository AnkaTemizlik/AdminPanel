import React, { } from "react";
import { useSelector } from 'react-redux';
import { useParams } from "react-router-dom";
import { Box, Grid, Paper } from "@material-ui/core";
import ModelEditForm from "./ModelEditForm";

const View = React.memo((props) => {
	const { name, action } = props;
	const params = useParams();
	const screen = useSelector(state => state.screenConfig.screens[name])

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Box p={1}>
					<Paper id="screnViewPaper" style={{ minHeight: '260px' }}>
						<Box p={4}>
							{screen &&
								<ModelEditForm
									id={params.id}
									action={action}
									name={name}
									showCaption={true}
									simple={false}
									onInsert={props.onInsert}
									actions={screen.actions}
								/>
							}
						</Box>
					</Paper>
				</Box>
			</Grid>
		</Grid>
	</>
})

export default View