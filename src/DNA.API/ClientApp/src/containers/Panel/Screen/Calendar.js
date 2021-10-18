import React, { useEffect, useState } from "react";
import { useSelector, useDispatch } from 'react-redux';
import { useHistory, useParams, useRouteMatch, useLocation, Link } from "react-router-dom";
import { Box, Divider, Grid, Icon, IconButton, Paper, Toolbar, Tooltip, Typography } from "@material-ui/core";
import { useTranslation } from "../../../store/i18next";
import { ApiURL } from "../../../store/axios";
import { selectScreen, getById } from './store/screenSlice'
import Scheduler, { Resource } from 'devextreme-react/scheduler';
import createCustomStore from "../../../components/UI/Table/createCustomStore";
import CustomStore from "devextreme/data/custom_store";
import DataSource from "devextreme/data/data_source";
import notify from "devextreme/ui/notify";
import api from '../../../store/api'
import { isEmpty, isNotEmpty } from "../../../store/utils";
import Iconify from "../../../components/UI/Icons/Iconify";
import Moment from 'react-moment';
import Button from "devextreme-react/button";
import PopupComponent from "../../../components/UI/Popup";
import ModelEditForm from "./ModelEditForm";

// import api from "../../../store/api";

const views = ["day", "week", "month"];

const CalendarWrapper = (props) => {
	const { name } = props;

	const { t } = useTranslation();
	const history = useHistory();
	const params = useParams();
	const dispatch = useDispatch();
	const { currentScreen, row } = useSelector(selectScreen)
	const [resources, setResources] = useState(null)
	const [loading, setLoading] = useState(false)
	const [store, setStore] = useState(null)

	//console.warning("CalendarView", calendar, data)
	const onError = (e) => {
		notify(typeof e == "string" ? e : e.Message, "error", 5000)
	}
	useEffect(() => {
		setStore(() => {
			if (Array.isArray(currentScreen.dataSource))
				return currentScreen.dataSource
			else {
				return createCustomStore(currentScreen.dataSource, currentScreen.keyFieldName, currentScreen.calendar.filter, null, onError)
			}
		})
	}, [currentScreen.dataSource, currentScreen.keyFieldName, currentScreen.calendar.filter])

	// useEffect(() => {
	// 	if (name && currentScreen) {
	// 		if (currentScreen.calendar) {
	// 			var resourcesCopy = currentScreen.calendar.resources.map(r => { return { ...r } })
	// 			resourcesCopy.map(async (r, i) => {
	// 				let ds = r.dataSource
	// 				if (!Array.isArray(r.dataSource)) {
	// 					let loadUrl = ds && ds.load
	// 						? (`${ds.load}${ds.load.indexOf('?') > -1 ? '&' : '?'}${'name=' + (ds.name)}`)
	// 						: `/api/entity?${'name=' + (ds.name)}`
	// 					let status = await api.execute("GET", loadUrl)
	// 					r.dataSource = status.Resource.Items
	// 				}
	// 			})
	// 			setResources(resourcesCopy)
	// 		}
	// 	}
	// }, [currentScreen, name])

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Box pl={1} pr={1}>
					{(!loading && currentScreen.calendar)
						? <CalendarView
							name={name}
							keyFieldName={currentScreen.keyFieldName}
							dataSource={store}
							calendar={currentScreen.calendar}
							columns={currentScreen.columns}
							resources={currentScreen.calendar.resources || []}
						/>
						: <div>Loading...</div>
					}
				</Box>
			</Grid>
		</Grid>
	</>
}

const CalendarView = React.memo(({ name, keyFieldName, dataSource, calendar, columns, resources }) => {

	const schedulerRef = React.useRef(null)
	const { t } = useTranslation();
	const [newItem, setNewItem] = useState(null)

	//console.success("resources", resources)

	const onAppointmentAdding = (e) => {
		columns.map(c => {
			if (isNotEmpty(c.defaultValue) && isEmpty(e.appointmentData[c.name])) {
				// eslint-disable-next-line no-eval
				let val = eval(c.defaultValue)
				e.appointmentData[c.name] = val
			}
		})
		return e;
	}

	const onAppointmentDeleting = (e) => {
		// e.appointmentData
		return e;
	}

	const onAppointmentUpdating = async (e) => {
		return e;
	}

	const onAppointmentUpdated = (e) => {
		return e;
	}

	const renderAppointmentTooltip = (e, index, element) => {
		let a = e.appointmentData
		let expr = calendar.tooltipDisplayExpr
		let start = a[calendar.startDateExpr]
		let end = a[calendar.endDateExpr]
		let allDay = !!a[calendar.allDayExpr]
		let recurrence = !!a[calendar.recurrenceRuleExpr]
		console.info("renderAppointmentTooltip", e, index, schedulerRef)
		return <div style={{ textAlign: "left", paddingLeft: 4 }}>
			{expr
				? Array.isArray(expr)
					? expr.map((x, i) => <div key={i}>
						{i == 0
							? <div>
								<div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
									<Typography style={{ display: "inline-block", wordWrap: "normal", whiteSpace: "break-spaces" }}>
										{a[x]}
									</Typography>
									<Button
										hint={t("Delete")}
										icon="trash"
										stylingMode="text"
										onClick={({ event }) => {
											event.stopPropagation()
											schedulerRef.current.instance.deleteAppointment(a)
											schedulerRef.current.instance.hideAppointmentTooltip()
										}}
									/>
								</div>
								<Divider />
							</div>
							: <div key={i}>
								<Typography component="span">{t(x)}: {a[x]}</Typography>
							</div>
						}
					</div>
					)
					: <>
						<div>
							{a[calendar.textExpr]}
						</div>
						<div>
							{<Typography component="span">{t(expr)}: {a[expr]}</Typography>}
						</div>
					</>
				: a[calendar.textExpr]
			}
			<Divider />
			<div style={{ display: "flex", justifyContent: "flex-end", alignItems: "center", fontSize: 10 }}>
				<div>
					{start && <Moment titleFormat="LLLL" withTitle format={allDay ? "LL" : "LLL"}>
						{new Date(start).toISOString()}
					</Moment>}
					{!allDay && end
						? <>
							<span> - </span>
							<Moment titleFormat={allDay ? "LLL" : "LLLL"} withTitle format={allDay ? "LL" : "LLL"}>
								{new Date(end).toISOString()}
							</Moment>
						</>
						: null
					}
				</div>
				{recurrence && <span style={{ paddingLeft: 12, paddingRight: 12 }}>
					<i class="dx-icon-repeat"></i>
				</span>}
			</div>
		</div>
	}

	const renderAppointment = (e) => {
		let a = e.appointmentData
		let expr = calendar.displayExpr
		let recurrence = !!a[calendar.recurrenceRuleExpr]

		return <div>
			{expr
				? Array.isArray(expr)
					? expr.map((t, i) => <div key={i}>
						{i == 0
							? <div style={{ display: "flex", justifyContent: "space-between" }}>
								<span>{a[t]}</span>
								{recurrence && <span style={{ position: "absolute", right: 0 }}>
									<Iconify icon="mdi:repeat" fontSize="1.25em" />
								</span>}
							</div>
							: <Typography component="span" variant="body2">{a[t]}</Typography>
						}
					</div>)
					: <>
						<div>
							{a[calendar.textExpr]}
						</div>
						<div>
							{<Typography variant="body2">{a[expr]}</Typography>}
						</div>
					</>
				: a[calendar.textExpr]
			}
		</div>
	}

	return <>
		{newItem && newItem.visible &&
			<PopupComponent visible={newItem.visible}
				title={newItem.title}
				onClose={newItem.onClose}
			>
				<ModelEditForm name={newItem.name} action={newItem.action} simple={newItem.simple}
					onInsert={newItem.onClose}
				/>
			</PopupComponent>
		}

		<Scheduler
			ref={schedulerRef}
			views={views}
			defaultCurrentView={calendar.defaultCurrentView || "month"}
			dataSource={dataSource}
			editing={calendar.editing}
			startDateExpr={calendar.startDateExpr}
			endDateExpr={calendar.endDateExpr}
			allDayExpr={calendar.allDayExpr}
			textExpr={calendar.textExpr}
			descriptionExpr={calendar.descriptionExpr}
			height={calendar.height || 900}
			startDayHour={calendar.startDayHour || 6}
			endDayHour={calendar.endDayHour || 21}
			firstDayOfWeek={1}
			cellDuration={calendar.cellDuration || 60}
			showAllDayPanel={calendar.showAllDayPanel != false}
			maxAppointmentsPerCell={calendar.maxAppointmentsPerCell || 3}
			recurrenceRuleExpr={calendar.recurrenceRuleExpr}
			appointmentRender={renderAppointment}
			appointmentTooltipRender={renderAppointmentTooltip}
			onContentReady={(e) => {
				//console.info("onContentReady", e.component.option())
			}}
			//appointmentTooltipComponent={AppointmentTooltip}
			//recurrenceEditMode={"series"} //  'dialog' | 'occurrence' | 'series'
			//groupByDate={true}
			//adaptivityEnabled={true}
			onAppointmentAdding={onAppointmentAdding}
			// onAppointmentDeleting={onAppointmentDeleting}
			// onAppointmentUpdating={onAppointmentUpdating}
			// onAppointmentUpdated={onAppointmentUpdated}
			onAppointmentFormOpening={(e) => {
				let row = e.appointmentData;
				const popup = e.popup;
				const form = e.form;

				let mainGroupItems = form.itemOption('mainGroup').items;

				if (row[keyFieldName] > 0) {
					// EDIT
				}
				else {

					// NEW ROW
					columns.map(c => {

						let editor = form.getEditor(c.name)

						// defaultValue
						// eslint-disable-next-line no-eval
						let val = c.defaultValue ? eval(c.defaultValue) : undefined

						if (c.name != calendar.startDateExpr && c.name != calendar.endDateExpr) {
							if (isNotEmpty(c.defaultValue)) {
								e.appointmentData[c.name] = val
								if (editor) {
									editor.option("value", val)
								}
							}
						}

						// allDayExpr
						if (c.name == calendar.allDayExpr && val == true) {
							let endDateEditor = form.getEditor(calendar.endDateExpr)
							let startDateEditor = form.getEditor(calendar.startDateExpr)
							if (endDateEditor) {
								endDateEditor.option("value", startDateEditor.option("value"))
								row[calendar.endDateExpr] = row[calendar.startDateExpr]
							}
						}
					})
				}

				popup.option("showCloseButton", true)
				popup.option("showTitle", true)
				popup.option("title", t("Edit Appointment"))
				let popupButtonItems = popup.option("toolbarItems").filter(_ => _.shortcut != "cancel")
				popupButtonItems.find(_ => _.shortcut == "done").toolbar = "bottom"

				// quick add button
				columns.map(c => {
					if (c.addNewShortcut == true) {
						let dataField = "AddNew" + c.data.name
						if (!!form.getEditor(dataField))
							return
						let editor = form.getEditor(c.name)
						let dataSource = editor.option("dataSource")
						popupButtonItems.push({
							widget: "dxButton",
							location: "before",
							toolbar: "bottom",
							options: {
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
											setNewItem({ visible: false })
											if (typeof a == "number") {
												dataSource.pageIndex(0)
												dataSource.reload && dataSource.reload()
												let editor = form.getEditor(c.name)
												editor.option("value", a)
											}
										},
										simple: true
									}
									setNewItem(i)
								}
							},
						})
					}
				})

				popup.option("toolbarItems", popupButtonItems)

				columns.map(c => {

					// Amount için daha sonra dinamik hale getirilebilir
					if (c.name == "Amount") {
						if (!mainGroupItems.find(function (i) { return i.dataField === "Amount" })) {
							mainGroupItems.push({
								//colSpan: 2,
								label: { text: t("Appointment.Amount") },
								editorType: "dxNumberBox",
								dataField: "Amount",
								format: {
									style: "currency",
									currency: c.currency || "TRY"
								}
							});
						}
					}

					mainGroupItems.map(i => {
						if (i.dataField == c.name) {
							// required
							if (c.required == true) {
								i.validationRules = [{ type: "required" }]
							}
							if (i.editorType == 'dxSelectBox') {
								i.editorOptions.searchEnabled = true
							}
						}
					})
					//if (editor) {
					// if (!mainGroupItems.find(function (i) { return i.dataField === c.name })) {
					// 	console.info("onAppointmentFormOpening required", c.name)

					// }

					// var validationRules = editor.option("validationRules")
					// validationRules.push({ type: "required" })
					// console.info("onAppointmentFormOpening required", validationRules)
					// editor.option("validationRules", validationRules)
					//}

				})

				// daha sonra dinamik hale getirilebilir
				// if (!mainGroupItems.find(function (i) { return i.dataField === "PhoneNumber" })) {
				// 	mainGroupItems.push({
				// 		//colSpan: 2,
				// 		label: { text: t("Appointment.PhoneNumber") },
				// 		editorType: "dxTextBox",
				// 		editorOptions: { readOnly: true },
				// 		dataField: "PhoneNumber"
				// 	});
				// }

				let items = mainGroupItems.filter(i => i.itemType == "group" ? true : i.dataField != undefined)
				form.itemOption('mainGroup', 'items', items);
			}}
			resources={resources}
		>
		</Scheduler>
	</>
})

export default CalendarWrapper