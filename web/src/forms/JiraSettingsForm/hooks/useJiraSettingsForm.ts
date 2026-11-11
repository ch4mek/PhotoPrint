import { useFormikContext } from 'formik';
import { useCallback } from 'react';

import { JiraSettingsFormValues } from '../types';

export function useJiraSettingsForm() {
	const {
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		validateField,
		handleSubmit,
	} = useFormikContext<JiraSettingsFormValues>();

	const handleFieldChange = useCallback(
		(field: string) => (value: unknown) => {
			setFieldValue(field, value);
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
		handleSubmit,
	};
}
