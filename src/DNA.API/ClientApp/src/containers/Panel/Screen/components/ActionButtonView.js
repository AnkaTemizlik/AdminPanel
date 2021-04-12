import { DataGrid } from "devextreme-react";
import React, { useCallback, useEffect, useState } from "react";
import { useSelector, useDispatch } from 'react-redux';
import DataTable from "../../../../components/UI/Table/DataTable";
import api from "../../../../store/api";
import { toQueryString } from "../../../../store/utils";
import { showMessage } from '../../../../store/slices/alertsSlice'

const ActionButtonView = ({ model }) => {
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
			let parentRelationName = model.relationFieldNames[0] // EInvoiceSeriCode
			let modelRelationName = model.relationFieldNames[1];  // DocumentNumber
			let filter = [modelRelationName, "=", row[parentRelationName]]
			return { ...q, filter }
		})
	}, [model.relationFieldNames, row]);

	return (<>

	</>
	)
}

export default ActionButtonView;
