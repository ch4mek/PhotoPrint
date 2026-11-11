import { useFormikContext } from 'formik';
import { useCallback } from 'react';

import { AdminPanelUserAddingFormValues } from '../types';

export function useUserForm() {
	const {
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		validateField,
	} = useFormikContext<AdminPanelUserAddingFormValues>();

	const handleFieldChange = useCallback(
		(field: string) => async (value: unknown) => {
			await setFieldValue(field, value);
			setFieldTouched(field, true);
			validateField(field);
		},
		[setFieldValue, setFieldTouched, validateField]
	);

	return {
		values,
		errors,
		touched,
		handleFieldChange,
		setFieldValue,
		setFieldTouched,
		validateField,
	};
}
