import * as yup from 'yup';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

export const ddlType1Schema = (isMandatory = true) => {
	const schema = yup
		.mixed()
		.transform(
			(_, originalValue) =>
				originalValue?.description ||
				originalValue?.name ||
				originalValue?.fullName ||
				null
		)
		.required(FIELD_IS_REQUIRED);

	if (!isMandatory) {
		return schema.notRequired();
	}

	return schema;
};
