import React, { useCallback, useEffect, useState } from "react";
import { useSelector } from 'react-redux';
import DataGrid, { Button, Column, ColumnChooser, Editing, Form, Export, FilterBuilderPopup, FilterPanel, FilterRow, LoadPanel, Popup, Pager, Paging, RequiredRule, SearchPanel, Selection, Position, Lookup, HeaderFilter, Scrolling, Sorting } from "devextreme-react/data-grid";
import { Item } from 'devextreme-react/form';
import { Template } from 'devextreme-react/core/template';
import DropDownBoxComponent from "./DropDownBoxComponent";
import createCustomStore from "./createCustomStore";
import { supplant } from "../../../store/utils";

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
	const [row, setRow] = useState(null)
	const [texts, setTexts] = useState([])
	const [focusedRowKey, setFocusedRowKey] = useState(null)
	const { lists } = useSelector(s => s.screenConfig)
	const defaultValue = props.defaultValue || {}

	// function selectionChanged(selection) {
	// 	const { selectedRowsData } = selection
	// 	// onSelectionChanged && onSelectionChanged(selectedRowsData)
	// 	const data = selectedRowsData ? selectedRowsData[0] : null;
	// 	setRow(data)
	// 	onFocusedRowChanged && onFocusedRowChanged(data)
	// }

	const focusedRowChanged = useCallback((e) => {
		const dataRow = e.row && e.row.data
		let dataRowWithTexts = {}
		if (dataRow) {
			Object.keys(dataRow).map(c => {
				let cell = e.row.cells.find(l => l.column && l.column.dataField == c);
				dataRowWithTexts[c] = cell ? cell.text : dataRow[c]
			})
		}
		setRow(dataRow)
		setTexts(dataRowWithTexts)
		setFocusedRowKey(e.component.option('focusedRowKey'))
		onFocusedRowChanged && onFocusedRowChanged(dataRow, dataRowWithTexts)
	}, [onFocusedRowChanged])

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

		actions && actions.filter(a => a.visible != false && a.type == "dx").map(a => {
			let actionComponent = {
				location: 'before',
				widget: 'dxButton',
				text: (a.icon || a.dxIcon) ? undefined : a.text,
				options: {
					hint: a.text,
					icon: a.icon || a.dxIcon,
					onClick: () => {
						if (row && a.eval) {
							try {
								let command = supplant(a.eval, texts, row)
								if (command) {
									// eslint-disable-next-line no-eval
									eval(command)
									if (a.onSuccess) {
										let text = supplant(a.onSuccess.text, texts, row)
										props.onError && props.onError({ Success: true, Message: text })
									}
								}
								else
									props.onError && props.onError({ Success: false, Message: "'eval' not defined" })
							} catch (error) {
								props.onError && props.onError({ Success: false, Message: error.toString() })
							}
						}
						else {
							console.error("The 'dx' action only supports the 'eval' feature for now.")
						}
					}
				}
			}
			items = [...items, actionComponent]
		})

		e.toolbarOptions.items.unshift(...items);

	}

	const imageCellRender = (data, c) => {
		return <img src={data.value} alt="" />;
	}

	return <DataGrid
		id={props.id || "grid"}
		keyExpr={keyFieldName || "Id"}
		ref={getRef}
		dataSource={store}
		//columnMinWidth={90}
		width={'100%'}
		focusedRowEnabled={true}
		focusedRowKey={focusedRowKey}
		onFocusedRowChanged={focusedRowChanged}
		onToolbarPreparing={onToolbarPreparing}
		//onSelectionChanged={(s) => { console.info("onSelectionChanged", s) }}
		onInitNewRow={(e) => {
			e.data = { ...e.data, ...defaultValue }
		}}
		onEditorPreparing={(e) => {
			if (e.parentType === 'dataRow' && defaultValue[e.dataField]) {
				e.editorOptions.disabled = true;
			}
		}}
		onSaved={s => {
			var g = s.component
			var selectedRow = g.getVisibleRows().find(c => c.key == focusedRowKey)
			// var i = focusedRowKey
			var result = { row: selectedRow, component: s.component }
			focusedRowChanged(result)
			//setRow({ ...row, ...s.changes[0].data })
			// //setFocusedRowKey(0)
			// setFocusedRowKey(i)
		}}
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
		//columnAutoWidth={gridOptions.columnAutoWidth !== false}
		columnResizingMode={gridOptions.columnResizingMode || "widget"}
		disabled={gridOptions.disabled == true}
		noDataText={gridOptions.noDataText}
		hoverStateEnabled={gridOptions.hoverStateEnabled !== false}
		autoNavigateToFocusedRow={gridOptions.autoNavigateToFocusedRow == true}
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
			<Popup title={props.name} showTitle={true} maxWidth={700} height={525}>
				<Position my="center" at="center" of={window} />
			</Popup>

			<Form>
				{columns.map((c, i) => {
					if (Object.keys(defaultValue).indexOf(c.name) > -1)
						return null
					return c.allowEditing == true
						? <Item key={i}
							dataField={c.name}
							colSpan={c.colSpan}
							editorType={c.editorType}
							helpText={c.helpText}
						/>
						: null
				})}
			</Form>

		</Editing>}

		{/* <Selection
			mode="single"
			selectAllMode={"page"}
			showCheckBoxesMode={"onClick"}
			{...gridOptions.selection}
		/> */}

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
				cellRender={c.type == "image"
					? (cell) => imageCellRender(cell, c)
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
						? <Lookup dataSource={c.customStore} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
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
			{...scrolling}
		/>

	</DataGrid>
})

export default DataTable
