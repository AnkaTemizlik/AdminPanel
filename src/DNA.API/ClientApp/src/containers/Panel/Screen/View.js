import React, { useEffect } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams, useRouteMatch, useLocation } from "react-router-dom";
//import { Switch, Link, useRouteMatch, useHistory, Redirect, Route } from "react-router-dom";
import { Box, Grid, Icon, IconButton, Paper, Toolbar, Tooltip } from "@material-ui/core";
import Form, { Item } from 'devextreme-react/form';
import { useTranslation } from "../../../store/i18next";
import { selectScreen, getById } from './store/screenSlice'
import ArrayStore from 'devextreme/data/array_store';

function useQuery() {
	return new URLSearchParams(useLocation().search);
}

const View = React.memo((props) => {
	const { name } = props;
	const { t } = useTranslation();
	const history = useHistory();
	const params = useParams();
	const dispatch = useDispatch();
	const { currentScreen, row, loading } = useSelector(selectScreen)

	useEffect(() => {
		if (name && params.id)
			dispatch(getById({ name, id: params.id }))
	}, [dispatch, name, params.id])

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Toolbar component={Paper}>
					<IconButton edge="end" onClick={() => dispatch(getById({ name, id: params.id }))} disabled={loading}>
						<Icon>refresh</Icon>
					</IconButton>
				</Toolbar>
			</Grid>
		</Grid>

		<Paper id="screnViewPaper" style={{ minHeight: '260px' }}>
			<Box p={4}>
				{row && currentScreen &&
					<Form formData={{ ...row }}
						id={`idFor-${name}-${params.id}-form`}
						colCount={2}
					>
						{currentScreen.columns.map((c, i) => {
							// if (c.hidden == true)
							// 	return null
							let readOnly = c.hidden == true || c.allowEditing != true;
							let editorOptions = {
								readOnly: readOnly,
								disabled: !readOnly,
								dataType: c.dataType,
								dataField: c.dataField,
								editorType: c.editorType
							}
							if (c.customStore) {
								editorOptions.dataSource = c.customStore
								editorOptions.displayExpr = c.data.displayExpr
								editorOptions.keyExpr = c.data.valueExpr
							}
							else if (c.autoComplete) {
								editorOptions.dataSource = new ArrayStore({
									data: c.lookup.dataSource,
									key: c.lookup.valueExpr
								})
								editorOptions.displayExpr = c.lookup.displayExpr
								editorOptions.keyExpr = c.lookup.valueExpr
							}
							else if (c.data && c.data.type == "simpleArray") {
								editorOptions.dataSource = c.dataSource
								editorOptions.displayExpr = c.data.displayExpr
								editorOptions.valueExpr = c.data.valueExpr
							}

							return <Item key={i}
								dataField={c.name}
								dataType={c.dataType}
								colSpan={c.colSpan}
								editorType={c.editorType}
								editorOptions={editorOptions}
								helpText={c.helpText}
							/>
						})}
					</Form>
				}

			</Box>
		</Paper>
	</>
})

export default View