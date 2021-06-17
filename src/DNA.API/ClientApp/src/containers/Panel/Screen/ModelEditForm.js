import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from 'react-redux';
//import { Switch, Link, useRouteMatch, useHistory, Redirect, Route } from "react-router-dom";
import Form, { GroupItem, SimpleItem, ButtonItem } from 'devextreme-react/form';
import { useTranslation } from "../../../store/i18next";
import ArrayStore from 'devextreme/data/array_store';
import DataSource from "devextreme/data/data_source";
import Popup from '../../../components/UI/Popup'
import notify from "devextreme/ui/notify";
import { getById, setRow, selectScreen } from './store/screenSlice'
import ActionsView from "./components/ActionsView";
import { Box } from "@material-ui/core";
import { isEmpty, isNotEmpty } from "../../../store/utils";

const ModelEditForm = React.memo((props) => {

	const { name, action, id, showCaption, actions } = props
	const { t } = useTranslation();
	const dispatch = useDispatch();
	const [newItem, setNewItem] = useState(null)
	const screen = useSelector(state => state.screenConfig.screens[name])
	const [data, setData] = useState({})
	const [row, setCurrentRow] = useState({})
	const [loading, setLoading] = useState(false)

	const handleSubmit = (e) => {
		e.preventDefault();
		setLoading(true)
		if (row && data.id > 0) {
			screen.dataSource
				.update(data.id, row)
				.then(status => {
					setLoading(false)
					notify(status.Message ?? t("Saved"), status.Success ? "success" : "error")
					if (status.Success) {
						props.onUpdate && props.onUpdate(status.Resource)
					}
				})
		}
		else {
			screen.dataSource.insert(row)
				.then(status => {
					setLoading(false)
					notify(status.Message ?? t("Saved"), status.Success ? "success" : "error")
					if (status.Success) {
						props.onInsert && props.onInsert(status.Resource)
					}
				})
		}
	}

	useEffect(() => {
		setData({ id, action })
	}, [id, action])

	useEffect(() => {
		if (row) {
			screen.columns.map(c => {
				if (isEmpty(row[c.name])) {
					if (isNotEmpty(c.defaultValue)) {
						// eslint-disable-next-line no-eval
						row[c.name] = eval(c.defaultValue)
					}
				}
			})
		}

	}, [row, screen.columns])

	useEffect(() => {
		if (name && data.id) {
			screen.dataSource.byKey(data.id)
				.then(status => {
					//console.purple("byKey useEffect", status)
					dispatch(setRow({ ...status.Resource }))
					setCurrentRow(status.Resource)
					if (!status.Success)
						notify(status.Message, "error", 5000)
				})
		}
	}, [dispatch, name, data, screen.dataSource])

	return <>

		{actions &&
			<Box pb={4}>
				<ActionsView
					actions={actions.filter(_ => (action == "edit" && _.showOnEditPage == true))}
					showButtonText={true}
				/>
			</Box>
		}

		<form onSubmit={handleSubmit}>
			<Form formData={row}
				//onSubmit={handleSubmit}
				id={`idFor-${name}-${action}-form`}
			>
				<GroupItem caption={showCaption ? t((data.id > 0 ? "Edit" : "New") + " " + name) : undefined}>
					{screen.columns
						.filter(c => c.allowEditing == true)
						.filter(c => props.simple == true ? (c.required && c.allowEditing) : true)
						.map((c, i) => {
							// if (props.simple ? (c.required && c.allowEditing) : false)
							// 	return null
							// if (c.allowEditing != true)
							// 	return null
							// if (c.hidden == true)
							// 	return null

							let editorOptions = {
								...c.editorOptions,
								readOnly: c.allowEditing != true,
								dataType: c.dataType,
								dataField: c.dataField,
								editorType: c.editorType
							}
							if (c.customStore) {

								editorOptions.dataSource = new DataSource({
									store: c.customStore,
									paginate: true,
									pageSize: 10
								})
								editorOptions.displayExpr = c.data.displayExpr
								editorOptions.keyExpr = c.data.valueExpr
								editorOptions.valueExpr = c.data.valueExpr
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

							let item = <SimpleItem key={i}
								dataField={c.name}
								label={{ text: c.caption || c.name }}
								dataType={c.dataType}
								colSpan={c.colSpan}
								editorType={c.editorType}
								editorOptions={editorOptions}
								helpText={c.helpText}
								isRequired={c.required}
							/>

							if (c.data && c.addNewShortcut == true)
								return <GroupItem key={i} colCountByScreen={{ sm: 2, md: 2, lg: 2 }}>
									{item}
									<SimpleItem
										itemType="button"
										horizontalAlignment="right"
										buttonOptions={{
											text: t('New ' + c.data.name),
											icon: "add",
											onClick: () => {
												var i = {
													...newItem,
													visible: true,
													name: c.data.name,
													title: "New " + c.data.name,
													action: "new",
													onClose: (a) => {
														//console.purple("ModelEditForm onClose", newItem, props)
														setNewItem({ visible: false })
														if (typeof a == "number") {
															if (editorOptions.dataSource && editorOptions.dataSource.reload) {
																editorOptions.dataSource.pageIndex(0)
																editorOptions.dataSource.reload()
															}
														}
													},
													simple: true
												}
												//console.purple("ModelEditForm SimpleItem onClick", i)
												setNewItem(i)
											}
										}}
									/>
								</GroupItem>
							else
								return item
						}).filter(c => c != null)
					}
				</GroupItem>

				<ButtonItem
					horizontalAlignment="right"
					buttonOptions={{
						text: t('Save'),
						type: 'default',
						useSubmitBehavior: true
					}}
				/>

			</Form>
		</form>

		{newItem && newItem.visible &&
			<Popup visible={newItem.visible}
				title={newItem.title}
				onClose={newItem.onClose}
			>
				<ModelEditForm name={newItem.name} action={newItem.action} simple={newItem.simple}
					onInsert={newItem.onClose}
				/>
			</Popup>
		}

	</>
})

export default ModelEditForm