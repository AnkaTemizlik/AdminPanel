import React, { useState } from 'react'
import { Box, Grid, Icon, IconButton, Paper, Toolbar, Tooltip } from '@material-ui/core';
import { TextArea } from 'devextreme-react';
import { ToggleButton, ToggleButtonGroup } from '@material-ui/lab';

const JsonToCSharp = (props) => {
	const sizeValues = ['8pt', '10pt', '12pt', '14pt', '18pt', '24pt', '36pt'];
	const fontValues = ['Arial', 'Courier New', 'Georgia', 'Impact', 'Lucida Console', 'Tahoma', 'Times New Roman', 'Verdana'];
	const headerValues = [false, 1, 2, 3, 4, 5];
	const [data, setData] = useState("{}")
	const [defaultValue, setDefaultValue] = useState("{}")
	const [toggle, setToggle] = useState("toDynamic")

	const format = (val) => {
		var parsed = JSON.parse(val)
		var d = JSON.stringify(parsed, null, 4)
		return d
	}

	const onChanged = (val) => {
		setData(toDynamic(val))
	}

	const toDynamic = (val) => {
		let data = format(val)
		data = data.replace(/"(\w+)":/mg, function (x, $1) {
			return $1 + " =";
		})
		data = data.replace(/\]/mg, () => "}")
		data = data.replace(/\{/mg, () => "new {")
		data = data.replace(/\[/mg, "new object[] {")
		return 'dynamic json = ' + data + ';'
	}

	return <>
		<Grid container spacing={2} alignItems="stretch">
			<Grid item xs={12}>
				<Toolbar component={Paper}>
					<Tooltip title="Format">
						<span>
							<IconButton onClick={() => setDefaultValue(format(data))}>
								<Icon>format_indent_increase</Icon>
							</IconButton>
						</span>
					</Tooltip>
					<Box flexGrow="1"></Box>
					<ToggleButtonGroup value={toggle} size="small">
						<ToggleButton value="toDynamic">
							{`to dynamıc`}
						</ToggleButton>
					</ToggleButtonGroup>
				</Toolbar>
			</Grid>
			<Grid item xs={12}>
				<Paper>
					<Box p={2}>

						<Grid container spacing={2}>
							<Grid item md={6}>
								<TextArea
									minHeight={150}
									maxHeight={1000}
									spellcheck={false}
									autoResizeEnabled={true}
									defaultValue={defaultValue}
									style={{
										fontFamily: 'monospace',
										fontSize: 14
									}}
									valueChangeEvent="keyup"
									onValueChanged={(x) => {
										console.info("onValueChanged", x)
									}}
									onInput={(x) => {
									}}
									onPaste={(x) => {
										var val = x.component._changedValue
										onChanged(val)
									}}
									onKeyUp={(x) => {
										var val = x.component._changedValue
										onChanged(val)
									}}
								/>
							</Grid>
							<Grid item md={6}>
								<TextArea
									minHeight={150}
									maxHeight={1000}
									spellcheck={false}
									autoResizeEnabled={true}
									value={data}
									style={{
										fontFamily: 'monospace',
										fontSize: 14
									}}
								/>
							</Grid>
						</Grid>

					</Box>
				</Paper>
			</Grid>
		</Grid>
	</>
}

export default JsonToCSharp