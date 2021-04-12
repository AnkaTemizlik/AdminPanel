import React from 'react';
import DataGrid, {
	Column,
	FilterRow,
	Lookup,
	Paging,
	Scrolling,
	Selection
} from 'devextreme-react/data-grid';
import DropDownBox from 'devextreme-react/drop-down-box';

const dropDownOptions = { width: 500 };

const DropDownBoxComponent = (props) => {

	const [currentValue, setCurrentValue] = React.useState(props.data.value)
	let dropDownBoxRef = React.useRef(null);

	const contentRender = () => {
		return (
			<DataGrid
				dataSource={props.data.column.lookup.dataSource}
				remoteOperations={true}
				keyExpr={props.edit.key}
				height={250}
				selectedRowKeys={[currentValue]}
				hoverStateEnabled={true}
				onSelectionChanged={onSelectionChanged}
				focusedRowEnabled={true}
				defaultFocusedRowKey={currentValue}
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

	const onSelectionChanged = (selectionChangedArgs) => {
		let val = selectionChangedArgs.selectedRowKeys[0]
		setCurrentValue(val);
		props.data.setValue(val);
		if (selectionChangedArgs.selectedRowKeys.length > 0) {
			dropDownBoxRef.current.instance.close();
		}
	}

	return (
		<DropDownBox
			ref={dropDownBoxRef}
			dropDownOptions={dropDownOptions}
			dataSource={props.data.column.lookup.dataSource}
			value={currentValue}
			displayExpr={props.edit.displayExpr}
			valueExpr={props.edit.valueExpr}
			contentRender={contentRender}
			showClearButton={false}
		>
		</DropDownBox>
	);

}

export default DropDownBoxComponent