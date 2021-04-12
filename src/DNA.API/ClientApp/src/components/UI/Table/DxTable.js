import React, { useState, useEffect } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Paper from "@material-ui/core/Paper";
import Toolbar from "@material-ui/core/Toolbar";
import Typography from "@material-ui/core/Typography";
import { Backdrop, Box, Icon, Tooltip } from "@material-ui/core";
import { green } from "@material-ui/core/colors";
import _ from "lodash";
import Moment from "react-moment";
import { Tr } from "../../../store/i18next";
import { SelectionState, EditingState, PagingState, CustomPaging, DataTypeProvider } from "@devexpress/dx-react-grid";
import { Grid, Table, TableColumnVisibility, TableHeaderRow, TableEditRow, TableEditColumn, TableSelection, PagingPanel } from "@devexpress/dx-react-grid-material-ui";
import DxCommandComponent from "./DxCommandComponent";
import DxEditCellComponent from "./DxEditCellComponent";
import Modal from "../Modal";
import i18n from "../../../store/i18n";

const useStyles = makeStyles(() => ({
	table: (props) => {
		const e = { position: "relative" };
		props.columns.map((c, i) => {
			if (c.width) {
				e[`& col:nth-child(${i + (props.allowEditing ? 2 : 1)})`] = { width: c.width };
			}
			return null;
		});
		return e;
	},
}));

const DxGrid = (props) => {
	const {
		keyFieldName,
		title,
		rows,
		onRowSaved,
		onRowDeleted,
		onFocusedRowChanged,
		onPageChanged,
		onPageSizeChanged,
		columns,
		rowsPerPage,
		page,
		allowEditing,
		totalItems,
		defaultSelection,
		loading,
	} = props;

	const classes = useStyles(props);

	const [selection, setSelection] = useState([]);
	//const [data, setData] = useState()
	const [errors, setErrors] = useState({});
	const [pageSize, setPageSize] = useState(rowsPerPage > 0 ? rowsPerPage : 10);
	const [pageSizes] = useState([5, 10, 25, 50, 100]);
	const [currentPage, setCurrentPage] = useState(page);
	const [localLoading, setLoading] = useState(false);
	//const [lastQuery, setLastQuery] = useState();
	const [editingRowIds, setEditingRowIds] = useState([]);
	const [addedRows, setAddedRows] = useState([]);
	const [modalOpen, setModalOpen] = React.useState({ open: false });

	const [editingStateColumnExtensions] = useState([{ columnName: keyFieldName, editingEnabled: false }]);
	const [defaultHiddenColumnNames] = useState(columns.filter((c) => c.hidden === true).map((c) => c.name));
	//const [checkColumns] = useState(columns.filter((c) => c.type == "bool" || c.type == "check").map((c) => c.name));
	//const [defaultColumnWidths] = useState(columns.filter((c) => c.hidden != true ).map((c) => ({ columnName: c.name, width: c.width ?? 120 })));

	const changeSelection = (e) => {
		const lastSelected = e.find((selected) => selection.indexOf(selected) === -1);
		if (lastSelected !== undefined) {
			setSelection([lastSelected]);
			onFocusedRowChanged && onFocusedRowChanged(getRowByKey(lastSelected), currentPage, pageSize);
		} else {
			setSelection([]);
			// NOTE: Uncomment the next line in order to allow clear selection by double-click
			onFocusedRowChanged && onFocusedRowChanged(null)
		}
	};

	const getRowByKey = (key) => {
		var filtered = rows.filter((row) => row[keyFieldName] === key);
		if (filtered) return filtered[0];
		else return {};
	};

	const commitChanges = ({ added, changed, deleted }) => {
		let addOrUpdate = added || changed;

		if (addOrUpdate) {
			if (onRowSaved) {
				let changedRows = added
					? [addOrUpdate[0]]
					: rows
						.map((row) => {
							return addOrUpdate[row[keyFieldName]] ? { ...row, ...addOrUpdate[row[keyFieldName]] } : null;
						})
						.filter((r) => r != null);

				setLoading(true);
				onRowSaved(changedRows[0]).then(() => {
					// if (status.Success)
					//     loadData(true);
					// else
					setLoading(false);
				});
			}
		} else if (deleted) {
			if (onRowDeleted) {
				setModalOpen({ open: true, deleted });
			}
		}
	};
	const handleModalCancel = (params) => {
		setModalOpen(params);
	};
	const handleModalConfirm = (params) => {
		setModalOpen(params);
		setLoading(true);
		onRowDeleted(params.deleted[0]).then(() => {
			setLoading(false);
		});
	};

	const handleCurrentPageChange = (e) => {
		onPageChanged(e);
		setCurrentPage(e);
	};
	const handlePageSizeChange = (e) => {
		onPageSizeChanged && onPageSizeChanged(e);
		setPageSize(e);
	};

	useEffect(() => {
		setCurrentPage(page);
	}, [page]);

	useEffect(() => {

		setSelection((s) => {
			if (defaultSelection && defaultSelection.length > 0) {
				if (!_.isEqual(_.sortedUniq(defaultSelection), _.sortedUniq(s))) {
					return defaultSelection;
				}
				else return s;
			}
			else return []
		});
	}, [defaultSelection]);

	const validate = (rows, columns) =>
		Object.entries(rows).reduce((acc, [rowId, row]) => {
			return {
				...acc,
				[rowId]: columns.some((column) => column.required && row[column.name] === ""),
			};
		}, {});

	const validateAdded = (rows, columns) =>
		Object.entries(rows).reduce((acc, [rowId, row]) => {
			return {
				...acc,
				[rowId]: columns.some((column) => column.required && (row[column.name] === undefined || row[column.name] === "")),
			};
		}, {});

	const onEdited = (e) => {
		setErrors(validate(e, columns));
	};

	const onAdded = (e) => {
		setAddedRows(e);
		setErrors(validateAdded(e, columns));
	};

	const CellFormater = (props) => (
		<DataTypeProvider
			for={columns.filter((c) => c.hidden != true).map((c) => c.name)}
			formatterComponent={(p) => {
				const { value, column } = p;
				let content = value;

				if (content) {
					if (column.type == "check" || column.type == "bool") {
						return value == true ?
							<div style={{ position: "relative" }}>
								<Icon style={{ color: green[500], position: "absolute", marginTop: -12 }}>check</Icon>
							</div>
							: null;
					} else if (column.type == "datetime") {
						return (
							<Tooltip title={<Moment format={"LLLL"}>{new Date(content)}</Moment>}>
								<span>
									<Moment format={column.format || "LLL"}>{new Date(content)}</Moment>
								</span>
							</Tooltip>
						);
					} else if (column.type == "number") {
						return (
							<Box display="flex" justifyContent="flex-end">
								{Intl.NumberFormat(i18n.language).format(content)}
							</Box>
						);
					} else if (content.length > 10) {
						content = column.translate ? <Tr>{content}</Tr> : content;
						return (
							<Tooltip title={content}>
								<span>{content}</span>
							</Tooltip>
						);
					}
				}

				return column.translate ? <Tr>{content}</Tr> : content;
			}}
			{...props}
		/>
	);

	return (
		<>
			<Paper className={classes.table}>
				<Backdrop
					open={loading || localLoading}
					onClick={() => { }}
					style={{
						position: "absolute",
						zIndex: 1,
						backgroundColor: "rgba(0, 0, 0, 0.1)",
					}}
				/>

				{title && (
					<Toolbar>
						<Typography variant="h6" id="tableTitle" component="div" style={{ flexGrow: 1 }}>
							{title}
						</Typography>
					</Toolbar>
				)}

				<Grid rows={rows || []} columns={columns} getRowId={(row) => row[keyFieldName]}>
					{allowEditing && (
						<EditingState
							editingRowIds={editingRowIds}
							onEditingRowIdsChange={setEditingRowIds}
							//rowChanges={rowChanges}
							onRowChangesChange={onEdited}
							addedRows={addedRows}
							onAddedRowsChange={onAdded}
							columnExtensions={editingStateColumnExtensions}
							onCommitChanges={commitChanges}
						/>
					)}

					<PagingState currentPage={currentPage} onCurrentPageChange={handleCurrentPageChange} pageSize={pageSize} onPageSizeChange={handlePageSizeChange} />
					<SelectionState selection={selection} onSelectionChange={changeSelection} />
					<CustomPaging totalCount={totalItems} />

					<CellFormater />

					<Table />

					{/* <TableColumnResizing defaultColumnWidths={defaultColumnWidths} /> */}

					<TableColumnVisibility defaultHiddenColumnNames={defaultHiddenColumnNames} />

					{rows ? <TableHeaderRow /> : null}

					{allowEditing && <TableEditRow />}

					{allowEditing && (
						<TableEditColumn
							showAddCommand={addedRows.length === 0 && editingRowIds.length === 0}
							showEditCommand={addedRows.length === 0 && editingRowIds.length === 0}
							showDeleteCommand={addedRows.length === 0 && editingRowIds.length === 0}
							commandComponent={DxCommandComponent}
							cellComponent={(props) => <DxEditCellComponent {...props} errors={errors} />}
							width={80}
						/>
					)}

					<TableSelection selectByRowClick highlightRow />
					{rows ? <PagingPanel pageSizes={pageSizes} /> : null}
				</Grid>

				<Modal
					open={modalOpen.open}
					onClose={() => setModalOpen({ open: false })}
					title={title}
					params={modalOpen}
					ok={handleModalConfirm}
					cancel={handleModalCancel}
					okText="Ok"
					cancelText="Cancel"
				>
					<Typography variant="body1">{`Kaydı silmek istediğinizden emin misiniz?`}</Typography>
				</Modal>
			</Paper>
		</>
	);
};

export default React.memo(DxGrid);

// const EditColumn = ({ children, ...restProps }) => {
//     if (
//         (!restProps.row.account || restProps.row.account === "") &&
//         restProps.tableRow.type === TableEditRow.ADDED_ROW_TYPE
//     ) {
//         return (
//             <TableEditColumn.Cell {...restProps}>
//                 {React.Children.map(children, child =>
//                     child
//                         ? child.props.id === "commit"
//                             ? React.cloneElement(child, { disabled: true })
//                             : child
//                         : false
//                 )}
//             </TableEditColumn.Cell>
//         );
//     }
//     return <TableEditColumn.Cell {...restProps} children={children} />;
// };
