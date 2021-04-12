import React, { } from "react";
import Moment from "react-moment";
import { Grid, Box, Paper, Table, TableContainer, TableHead, TableRow, TableBody, TableCell, Icon, Typography } from "@material-ui/core";
import { green } from "@material-ui/core/colors";
import TextArea from 'devextreme-react/text-area';
import { Tr, useTranslation } from "../../../../store/i18next";
import i18n from "../../../../store/i18n";

const RowFieldsView = React.memo(({ row, columns, name }) => {
	const { t } = useTranslation();

	function renderCellValue(r, c) {
		if (r && r[c.name]) {
			let value = r[c.name];

			if (c.translate)
				value = t(value);

			if (c.type == "datetime" || c.type == "date" || c.type == "time") {
				return (
					<Moment titleFormat="LLLL" withTitle format={c.localized || "LL"}>
						{new Date(value)}
					</Moment>
				);
			}

			if (c.type == "check" || c.type == "bool")
				return <Icon style={{ color: green[500], position: "absolute", marginTop: -12 }}>
					check
					</Icon>;

			if (c.type == "number" || c.type == "numeric") {
				if (c.currency) {
					return <Box>
						{Intl.NumberFormat(i18n.language, { style: 'currency', currency: c.currency }).format(value)}
					</Box>
				}
				else return <Box>
					{Intl.NumberFormat(i18n.language).format(value)}
				</Box>
			}

			if (c.type == "textArea") {
				return <TextArea readOnly={true} value={value} autoResizeEnabled={true} maxHeight={200} />
			}

			if (c.type == "code")
				return <TextArea readOnly={true} value={value} autoResizeEnabled={true} style={{ font: "13px monospace" }} />
			//return <pre style={{ whiteSpace: "pre-wrap" }}>{value}</pre>;

			return value;
		}
		return "";
	}

	return row ? (
		<TableContainer>

			<Table size="small">
				{/* <TableHead>
						<TableRow>
							<TableCell style={{ width: 260 }} />
							 <TableCell style={{ minWidth: 160 }}>{t("Field")}</TableCell> 
							<TableCell />
						</TableRow>
					</TableHead> */}
				<TableBody>
					{columns.map((c, i) => {
						var tr = <TableRow hover key={i} >
							<TableCell component="th" style={{ width: 180 }}>
								{c.title || t(`${name}.${c.name}`)}
							</TableCell>
							<TableCell>{renderCellValue(row, c)}</TableCell>
						</TableRow>
						return tr
					})}
				</TableBody>
			</Table>
		</TableContainer>
	) : null;
});

export default RowFieldsView;