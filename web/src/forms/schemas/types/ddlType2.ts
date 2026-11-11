import * as yup from 'yup';

import { textType1Schema } from './textType1';

export const ddlType2Schema = (fieldName: string, isMandatory = true) => {
	let schema = yup
		.mixed()
		.transform(
			(_, originalValue) =>
				originalValue?.description ||
				originalValue?.name ||
				originalValue?.fullName ||
				null
		)
		.concat(textType1Schema(fieldName, !isMandatory))
		.nullable();

	if (!isMandatory) {
		schema = schema.notRequired();
	}

	return schema;
};
