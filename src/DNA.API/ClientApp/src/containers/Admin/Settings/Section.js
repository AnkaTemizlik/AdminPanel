
import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Grid, Icon, IconButton, Tooltip } from "@material-ui/core";
import { Box } from "@material-ui/core";
import Field from './components/Field'
import ObjectCard from './components/ObjectCard'
import FormCard from './components/FormCard'
import CollapsibleCard from "../../../components/UI/CollapsibleCard";
import { useTranslation } from '../../../store/i18next'
import Action from "./components/Action";

const Section = (props) => {
	const { t } = useTranslation()
	const { loading } = props;
	const { currentSection, configs } = useSelector((state) => state.appsettings)
	const _ = configs._[currentSection]

	function handleChange(fullname, val) {
		props.onValueChange(fullname, val);
	}

	return (_) ? (
		<Grid container spacing={2}>

			{_.fields.length > 0 ? (
				<Grid item xs={12} sm={6} md={6} lg={4}>
					<Box mb={1} mt={1}>
						<CollapsibleCard title={t("General")} header={"h6"} exp={true}
							addComponent={_.action ? <Action {..._.action} /> : null}
						>
							{_.fields.map((f, i) => {
								return <Field key={i}
									field={f}
									handleChange={handleChange}
									loading={loading}
								/>
							})}

						</CollapsibleCard>
					</Box>
				</Grid>
			) : null}

			{_.forms.map((f, i) => {
				return (
					<Grid item xs={12} sm={6} md={6} lg={4} key={i}>
						<FormCard
							field={f}
							h="h6"
							handleChange={handleChange}
							loading={loading}
						/>
					</Grid>
				);
			})}

			{_.objects.map((f, i) => {
				return (
					<Grid item xs={12} sm={6} md={6} lg={4} key={i}>
						<ObjectCard
							field={f}
							h="h6"
							handleChange={handleChange}
							loading={loading}
						/>
					</Grid>
				);
			})}
		</Grid>
	) : (
		<Grid item xs={12}></Grid>
	);
};

export default Section;
