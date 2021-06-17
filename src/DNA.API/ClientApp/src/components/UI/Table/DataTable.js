import React, { useCallback, useEffect, useState } from "react";
import _ from 'lodash'
import { useSelector } from 'react-redux';
import { useHistory } from "react-router-dom";
import DataGrid, { Button, Column, ColumnChooser, Editing, Form, Export, FilterBuilderPopup, FilterPanel, FilterRow, LoadPanel, Popup, Pager, Paging, RequiredRule, SearchPanel, Selection, Position, Lookup, HeaderFilter, Scrolling, Sorting } from "devextreme-react/data-grid";
import { SimpleItem } from 'devextreme-react/form';
import { Template } from 'devextreme-react/core/template';
import DropDownBoxComponent from "./DropDownBoxComponent";
import createCustomStore from "./createCustomStore";
import { isNotEmpty, supplant } from "../../../store/utils";
import { useTranslation } from "react-i18next";
import { Avatar } from "@material-ui/core";

const DataTable = React.memo((props) => {
	const {
		keyFieldName,
		instance,
		dataSource,
		columns,
		children,
		onSelectionChanged,
		onFocusedRowChanged,
		defaultFilter,
		actions
	} = props

	const gridOptions = props.gridOptions || {};
	const editing = props.editing || gridOptions.editing || { enabled: false, mode: "popup", useIcons: true };
	const pager = gridOptions.pager || { visible: true }
	const paging = gridOptions.paging || { enabled: true, defaultPageSize: 10 }
	const sorting = gridOptions.sorting || { enabled: true, mode: "multiple", showSortIndexes: true, default: { selector: keyFieldName, desc: true } }
	const scrolling = gridOptions.scrolling || { mode: "standard" }
	const searchPanel = gridOptions.searchPanel || { visible: true }

	const [store, setStore] = useState(null)
	const [grid, setGrid] = useState(null)
	const [texts, setTexts] = useState([])
	const [focusedRowKey, setFocusedRowKey] = useState(null)
	const { lists } = useSelector(s => s.screenConfig)
	const defaultValue = props.defaultValue || {}
	let history = useHistory();
	const { t } = useTranslation()
	//const [newItem, setNewItem] = useState({ visible: false, name: null })

	function selectionChanged(e) {
		const { selectedRowsData } = e
		// onSelectionChanged && onSelectionChanged(selectedRowsData)
		const data = selectedRowsData ? selectedRowsData[0] : null;
		onFocusedRowChanged && onFocusedRowChanged(data)
		// let dataRowWithLabels = {}
		// if (data) {
		// 	Object.keys(data).map(c => {
		// 		let cell = e.row.cells.find(l => l.column && l.column.dataField == c);
		// 		dataRowWithLabels[c] = cell ? cell.text : data[c]
		// 	})
		// }
		// setTexts(dataRowWithLabels)
	}

	// const focusedRowChanged = (e) => {
	// 	//const dataRow = e.row && e.row.data
	// 	const dataRow = e.selectedRowsData[0]
	// 	let dataRowWithLabels = {}
	// 	if (dataRow) {
	// 		Object.keys(dataRow).map(c => {
	// 			let cell = e.row.cells.find(l => l.column && l.column.dataField == c);
	// 			dataRowWithLabels[c] = cell ? cell.text : dataRow[c]
	// 		})
	// 	}
	// 	setTexts(dataRowWithLabels)
	// 	//setFocusedRowKey(e.component.option('focusedRowKey'))
	// 	onFocusedRowChanged && onFocusedRowChanged(dataRow, dataRowWithLabels)
	// }

	// useEffect(() => {
	// 	console.purple("DataTable", newItem)
	// }, [newItem])

	useEffect(() => {
		setStore(() => {
			return Array.isArray(dataSource)
				? dataSource
				: createCustomStore(dataSource, keyFieldName, defaultFilter, lists)
		})
	}, [dataSource, keyFieldName, defaultFilter, lists])

	const getRef = (ref) => {
		if (ref && ref.instance) {
			instance && instance(ref.instance)
			setGrid(ref.instance)
		}
	}

	const refresh = () => {
		grid && grid.refresh();
	};

	const onToolbarPreparing = (e) => {
		if (props.actionsTemplate) {
			e.toolbarOptions.items.unshift({
				location: 'before',
				template: 'actionsTemplate'
			})
		}
		let items = [];

		if (!Array.isArray(dataSource)) {
			let refreshComponent = {
				location: 'before',
				widget: 'dxButton',
				options: {
					hint: "Refresh",
					icon: 'refresh',
					onClick: refresh
				}
			}
			items = [...items, refreshComponent]
		}

		// actions && actions.filter(a => a.visible != false && a.type == "dx").map(a => {
		// 	let actionComponent = {
		// 		location: 'before',
		// 		widget: 'dxButton',
		// 		text: (a.icon || a.dxIcon) ? undefined : a.text,
		// 		options: {
		// 			hint: a.text,
		// 			icon: a.icon || a.dxIcon,
		// 			onClick: () => {
		// 				if (row && a.eval) {
		// 					try {
		// 						let command = supplant(a.eval, texts, row)
		// 						if (command) {
		// 							// eslint-disable-next-line no-eval
		// 							eval(command)
		// 							if (a.onSuccess) {
		// 								let text = supplant(t(a.onSuccess.text), texts, row)
		// 								props.onError && props.onError({ Success: true, Message: text })
		// 							}
		// 						}
		// 						else
		// 							props.onError && props.onError({ Success: false, Message: "'eval' not defined" })
		// 					} catch (error) {
		// 						props.onError && props.onError({ Success: false, Message: error.toString() })
		// 					}
		// 				}
		// 				else if (a.route) {
		// 					history.push(supplant(a.route, texts, row))
		// 				}
		// 				else {
		// 					console.error("The 'dx' action only supports the 'eval' and 'route' feature for now.")
		// 				}
		// 			}
		// 		}
		// 	}
		// 	items = [...items, actionComponent]
		// })

		e.toolbarOptions.items.unshift(...items);

	}

	const imageCellRender = (data, c) => {
		let style = { maxHeight: 42, margin: "-6px 0" }
		console.success("imageCellRender", data, c)
		return data.value
			? <Avatar src={data.value} alt={data.value} style={style} />
			: <div style={style}>
				<Avatar />
			</div>
	}

	return <>

		<DataGrid
			id={props.id || "grid"}
			keyExpr={keyFieldName || "Id"}
			ref={getRef}
			dataSource={store}
			width={'100%'}
			//focusedRowEnabled={true}
			//focusedRowKey={focusedRowKey}
			//onFocusedRowChanging={focusedRowChanging}
			//onFocusedRowChanged={focusedRowChanged}
			onSelectionChanged={selectionChanged}
			onToolbarPreparing={onToolbarPreparing}
			onInitNewRow={(e) => {
				e.data = { ...e.data, ...defaultValue }
				//console.info("onInitNewRow", defaultValue, e.data)
			}}
			// onEditorPreparing={(e) => {
			// 	if (e.parentType === 'dataRow' && defaultValue[e.dataField]) {
			// 		//e.editorOptions.disabled = true;
			// 	}
			// }}
			// onSaved={s => {
			// 	var g = s.component
			// 	var selectedRow = g.getVisibleRows().find(c => c.key == focusedRowKey)
			// 	//var result = { row: selectedRow, component: s.component }
			// 	focusedRowChanged(selectedRow)
			// }}
			// onRowPrepared={(e) => {
			// 	if (isNotEmpty(props.onRowPrepared) && e.rowType == 'data') {
			// 		var formating = props.onRowPrepared.formating;
			// 		if (formating && formating.field && e.data[formating.field] == formating.value) {
			// 			for (const key in formating.style) {
			// 				e.rowElement.style[key] = formating.style[key]
			// 			}
			// 		}
			// 	}
			// }}
			//columnAutoWidth={true}
			columnResizingMode={gridOptions.columnResizingMode || "widget"}
			disabled={gridOptions.disabled == true}
			noDataText={gridOptions.noDataText}
			hoverStateEnabled={gridOptions.hoverStateEnabled !== false}
			//autoNavigateToFocusedRow={gridOptions.autoNavigateToFocusedRow == true}
			allowColumnResizing={gridOptions.allowColumnResizing !== false}
			allowColumnReordering={gridOptions.allowColumnReordering !== false}
			columnHidingEnabled={gridOptions.columnHidingEnabled !== false}
			rowAlternationEnabled={gridOptions.rowAlternationEnabled == true}
			showBorders={gridOptions.showBorders == true}
			showColumnHeaders={gridOptions.showColumnHeaders !== false}
			showColumnLines={gridOptions.showColumnLines !== false}
			showRowLines={gridOptions.showRowLines == true}
			wordWrapEnabled={gridOptions.wordWrapEnabled == true}
			remoteOperations={gridOptions.remoteOperations !== false}
			columnAutoWidth={true}
		>

			{/* <StateStoring enabled={true} type="localStorage" storageKey={"gridStorage_" + props.id} />  */}

			<SearchPanel {...searchPanel} />

			<Export enabled={true} allowExportSelectedData={false} />

			<ColumnChooser enabled={true} allowSearch={true} mode={"dragAndDrop"} />

			<LoadPanel enabled={!Array.isArray(dataSource)} shadingColor="rgba(0,0,0,0.05)" shading={true} />

			{gridOptions.allowFilter !== false &&
				<HeaderFilter visible={true} />}

			{gridOptions.allowFilter !== false &&
				<FilterRow visible={true} />}

			{gridOptions.allowFilter !== false &&
				<FilterPanel visible={true} />}

			{gridOptions.allowFilter !== false &&
				<FilterBuilderPopup width={360} height={480} />}

			{editing && <Editing
				mode={editing.mode || "popup"}
				allowUpdating={editing.allowUpdating == true}
				allowDeleting={editing.allowDeleting == true}
				allowAdding={editing.allowAdding == true}
				useIcons={editing.useIcons !== false}
				{...editing}
			>
				<Popup title={t(`Edit ${props.name}`)} showTitle={true} maxWidth={700} height={525}>
					<Position my="center" at="center" of={window} />
				</Popup>

				<Form>

					{_.orderBy(columns, ['editIndex'], ['asc']).map((c, i) => {
						// if (Object.keys(defaultValue).indexOf(c.name) > -1)
						// 	return null
						var item = c.allowEditing == true
							? <SimpleItem key={i}
								dataField={c.name}
								colSpan={c.colSpan}
								editorType={c.editorType}
								helpText={c.helpText}
								editorOptions={c.editorOptions}
							/>
							: null

						// if (c.data && c.addNewShortcut == true)
						// 	return <GroupItem colCount={2} key={i} colCountByScreen={{ sm: 2, md: 2, lg: 2 }}>
						// 		{item}
						// 		<SimpleItem
						// 			itemType="button"
						// 			horizontalAlignment="right"
						// 			buttonOptions={{
						// 				text: t('New ' + c.data.name),
						// 				icon: "add",
						// 				onClick: () => {
						// 					var i = {
						// 						...newItem,
						// 						visible: true,
						// 						name: c.data.name,
						// 						title: "New " + c.data.name,
						// 						action: "new",
						// 						onClose: () => {
						// 							setNewItem({ visible: false })
						// 							c.customStore.reload && c.customStore.reload()
						// 						},
						// 						simple: true
						// 					}
						// 					setNewItem(i)
						// 				}
						// 			}}
						// 		/>
						// 	</GroupItem>
						// else
						return item

					}).filter(c => c != null)}

				</Form>

			</Editing>
			}

			<Selection
				mode="single"
			//selectAllMode={"page"}
			//showCheckBoxesMode={"onClick"}
			//{...gridOptions.selection}
			/>

			{children}

			{props.editing && props.editing.enabled == true &&
				<Column type="buttons" width={70}>
					{props.editing.allowUpdating && <Button name="edit" />}
					{props.editing.allowDeleting && <Button name="delete" />}
				</Column>
			}

			{columns.map((c, i) => {
				let defaultSortOrder = c.defaultSortOrder ? c.defaultSortOrder : undefined
				if (defaultSortOrder == undefined && sorting && sorting.default) {
					if (sorting.default.selector == c.dataField)
						defaultSortOrder = sorting.default.desc ? "desc" : "asc"
				}
				// if (c.type == "check") {
				// 	defaultValue[c.name] = isNotEmpty(c.defaultValue) ? c.defaultValue : false
				// 	console.warning("col defaultValue", c, defaultValue)
				// }

				// if (defaultValue && defaultValue[c.name]) {

				// }
				return <Column
					key={i}
					visible={c.hidden !== true}
					dataField={c.name}
					dataType={c.dataType || "string"}
					width={c.width || (i < 10 ? 120 : undefined)}
					//visibleIndex={c.index}
					caption={c.caption}
					defaultSortOrder={defaultSortOrder}
					allowFiltering={c.allowFiltering !== false}
					allowHeaderFiltering={c.allowHeaderFiltering === true}
					cellRender={
						c.type == "image"
							? (cell) => imageCellRender(cell, c)
							: c.type == "color"
								? (cell) => <div className="dx-colorbox-color-result-preview" style={{ backgroundColor: cell.value, margin: 'auto', position: "inherit" }} />
								: undefined
					}
					format={c.format}
					setCellValue={c.setCellValue}
					editCellComponent={
						(c.editWith && c.editWith.type == "table")
							? (ediCellProps) => <DropDownBoxComponent {...ediCellProps} edit={c.editWith} />
							: null}
				>
					{c.required === true &&
						<RequiredRule />}
					{typeof c.customStore == 'function'
						? <Lookup dataSource={(o) => c.customStore(o)} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
						: c.customStore
							? <Lookup dataSource={{
								store: c.customStore,
								paginate: true,
								pageSize: 10
							}} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
							: c.dataSource
								? <Lookup dataSource={c.dataSource} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
								: c.lookup
									? <Lookup {...c.lookup} />
									: null
					}

				</Column>
			})}

			<Paging
				defaultPageSize={10}
				{...paging}
			/>

			<Sorting {...sorting} />

			<Pager
				visible={pager.visible !== false}
				allowedPageSizes={pager.allowedPageSizes || [5, 10, 25, 100]}
				infoText={pager.infoText || "Sayfa {0}/{1} ({2} kayıt)"}
				showNavigationButtons={pager.showNavigationButtons !== false}
				showInfo={pager.showInfo !== false}
				showPageSizeSelector={pager.showPageSizeSelector !== false}
			/>

			{props.actionsTemplate &&
				<Template name="actionsTemplate" render={props.actionsTemplate} />}

			<Scrolling
				mode="standard"
				{...scrolling}
			/>

		</DataGrid>

		{/* <PopupComponent visible={newItem.visible}
			title={newItem.title}
			onClose={newItem.onClose}
		>
			{newItem.visible && <ModelEditForm name={newItem.name} action={newItem.action} simple={newItem.simple}
				onInsert={newItem.onClose}
			/>}
		</PopupComponent> */}

	</>
})

export default DataTable
