
import React, { } from "react";
import { Box } from "@material-ui/core";
import CollapsibleCard from "../../../../components/UI/CollapsibleCard";
import { useTranslation } from '../../../../store/i18next'
import Field from './Field'
import FormCard from "./FormCard";
import Action from "./Action";

const ObjectCard = (props) => {

	const { t } = useTranslation()
	const { field, h, handleChange, loading, isArray } = props
	const { _, options } = field
	const { caption, visible, action } = options;
	const readOnly = props.readOnly || options.readOnly

	if (visible == false)
		return null;

	return (
		<Box mb={1} mt={1}>
			<CollapsibleCard title={t(caption)} header={h} exp={h == "h6" || isArray == true}
				addComponent={action ? <Action {...action} /> : null}
				hideActions={isArray == true}
			>
				{_.fields.map((f, i) => {
					return <Field key={i}
						field={f}
						handleChange={handleChange}
						loading={loading}
						readOnly={readOnly}
					/>
				})}
				{_.objects.map((f, i) => {
					return <ObjectCard key={i}
						field={f}
						h={"subtitle1"}
						handleChange={handleChange}
						loading={loading}
						readOnly={readOnly}
					/>
				})}
				{_.forms.map((f, i) => {
					return <FormCard key={i}
						field={f}
						h={"subtitle1"}
						handleChange={handleChange}
						loading={loading}
						readOnly={readOnly}
					/>
				})}
			</CollapsibleCard>
		</Box>
	);
};

export default ObjectCard;
