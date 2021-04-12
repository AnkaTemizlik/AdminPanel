
import React, { useEffect, useState, useCallback } from "react";
import { useSelector, useDispatch } from 'react-redux';
import api from "../../../../store/api";
import { toQueryString } from "../../../../store/utils";
import RowFieldsView from "./RowFieldsView";
import { showMessage } from '../../../../store/slices/alertsSlice'

const ModelView = ({ model }) => {
	const { panel, screenConfig } = useSelector(state => state)
	const { row } = panel.screen
	const dispatch = useDispatch()

	const [data, setData] = useState({})
	const [query, setQuery] = useState({ name: model.name, filter: [] })

	const getEntities = useCallback((q) => {
		api.entity.getEntities(toQueryString(q))
			.then((status) => {
				if (status.Success) {
					setData(status.Resource)
				}
				else dispatch(showMessage(status))
			})
	}, [dispatch])

	useEffect(() => {
		if (query.filter.length > 0)
			getEntities(query)
	}, [getEntities, query])

	useEffect(() => {
		setQuery((q) => {
			let parentRelationName = model.relationFieldNames[0] // HeaderId
			let modelRelationName = model.relationFieldNames[1];  // Id
			let filter = [modelRelationName, "=", row[parentRelationName]]
			return { ...q, filter }
		})
	}, [model.relationFieldNames, row]);

	return (<>
		{data && data.Items &&
			<RowFieldsView
				row={data.Items[0]}
				columns={screenConfig.screens[model.name].columns}
				name={model.name}
			/>
		}
	</>
	)
}

export default ModelView;
