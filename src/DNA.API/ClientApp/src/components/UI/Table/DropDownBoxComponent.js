import React, { useEffect } from 'react';
import DataGrid, {
	Column,
	FilterRow,
	Lookup,
	Paging,
	Scrolling,
	Selection
} from 'devextreme-react/data-grid';
import DropDownBox from 'devextreme-react/drop-down-box';

const dropDownOptions = { minWidth: 360 };

const DropDownBoxComponent = (props) => {

	const [currentValue, setCurrentValue] = React.useState([props.data.value])
	const [isGridBoxOpened, setGridBoxOpened] = React.useState(false)
	let dropDownBoxRef = React.useRef(null);
	const { editorOptions } = props.data.item

	const dataGridOnSelectionChanged = (selectionChangedArgs) => {
		setCurrentValue(selectionChangedArgs.selectedRowKeys);
		setGridBoxOpened(false)
	}

	const syncDataGridSelection = (e) => {
		setCurrentValue(e.value)
	}

	const onGridBoxOpened = (e) => {
		if (e.name === 'opened') {
			setGridBoxOpened(e.value)
		}
	}

	useEffect(() => {
		props.data.setValue(currentValue ? currentValue[0] : null);
	}, [currentValue])

	const contentRender = () => {
		return (
			<DataGrid
				dataSource={props.data.column.lookup.dataSource}
				remoteOperations={true}
				keyExpr={props.edit.key}
				height={250}
				selectedRowKeys={currentValue}
				hoverStateEnabled={true}
				onSelectionChanged={dataGridOnSelectionChanged}
				focusedRowEnabled={true}
			//defaultFocusedRowKey={currentValue}
			>

				{props.edit.columns.map((c, i) => {

					return <Column
						key={i}
						visible={c.hidden !== true}
						dataField={c.name}
						dataType={c.dataType || "string"}
						width={c.width || (i < 10 ? 120 : undefined)}
						visibleIndex={c.index}
						caption={c.caption}
						defaultSortOrder={c.defaultSortOrder}
						format={c.format}
					>

						{c.lookup &&
							<Lookup {...c.lookup} />}

						{typeof c.customStore == 'function'
							? <Lookup dataSource={(o) => c.customStore(o)} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
							: c.customStore
								? <Lookup dataSource={c.customStore} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
								: c.dataSource
									? <Lookup dataSource={c.dataSource} valueExpr={c.data.valueExpr} displayExpr={c.data.displayExpr} />
									: null
						}

					</Column>
				})}

				<Paging enabled={true} defaultPageSize={10} />
				<Scrolling mode="virtual" />
				<Selection mode="single" />
				<FilterRow visible={true} />
			</DataGrid>
		);
	}


	return (
		<DropDownBox
			ref={dropDownBoxRef}
			value={currentValue}
			opened={isGridBoxOpened}
			onValueChanged={syncDataGridSelection}
			onOptionChanged={onGridBoxOpened}
			dropDownOptions={dropDownOptions}
			dataSource={props.data.column.lookup.dataSource}
			displayExpr={props.edit.displayExpr}
			valueExpr={props.edit.valueExpr}
			contentRender={contentRender}
			deferRendering={false}
			//showClearButton={false}
			{...editorOptions}
		>
		</DropDownBox>
	);

}

export default DropDownBoxComponent