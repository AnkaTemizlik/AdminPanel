import React, { useEffect, useState } from "react";
import { useSelector } from 'react-redux';
import DataTable from "../../../../components/UI/Table/DataTable";
import withSnack from "../../../../store/snack";

const TableView = ({ model, snack }) => {
	const { panel, screenConfig } = useSelector(state => state)
	const { row } = panel.screen
	const screen = screenConfig.screens[model.name]
	//const dispatch = useDispatch()

	const [query, setQuery] = useState({ name: model.name, filter: [] })
	const [error, setError] = useState(null);
	const [defaultValue, setDefaultValue] = useState(null)

	useEffect(() => {
		if (error) {
			snack.show(error)
			setError(null)
		}
	}, [snack, error]);

	useEffect(() => {
		let parentRelationName = model.relationFieldNames[0]
		let modelRelationName = model.relationFieldNames[1];
		let filter = []
		let dv = {}
		if (Array.isArray(parentRelationName)) {
			model.relationFieldNames.map((r, i) => {
				parentRelationName = r[0]
				modelRelationName = r[1];
				filter.push([modelRelationName, "=", row[parentRelationName]])
				if (i < r.length - 1) {
					filter.push("and")
				}
				dv[modelRelationName] = row[parentRelationName]
			})
		}
		else {
			filter.push(modelRelationName)
			filter.push("=")
			filter.push(row[parentRelationName])
			dv[modelRelationName] = row[parentRelationName]
		}

		setDefaultValue(dv)
		setQuery((q) => {
			return { ...q, filter, parentRelationName, modelRelationName }
		})
	}, [model.relationFieldNames, row]);

	if (defaultValue && screenConfig && screen && query && query.filter)
		return <>
			<DataTable
				id={"idForSub" + model.name}
				name={model.name}
				onError={setError}
				dataSource={screen.dataSource}
				columns={screen.columns}
				keyFieldName={screen.keyFieldName}
				rowAlternationEnabled={screen.rowAlternationEnabled}
				onRowPrepared={screen.onRowPrepared}
				editing={model.editing || screen.editing}
				defaultFilter={query.filter}
				defaultValue={defaultValue}
				// allowFilter={false}
				// allowPaging={false}
				gridOptions={screen.grid}
				actions={screen.actions}
			/>
		</>
	else
		return null
}

export default withSnack(TableView);