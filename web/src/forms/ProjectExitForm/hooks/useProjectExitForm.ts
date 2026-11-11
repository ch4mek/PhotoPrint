import { useFormikContext } from 'formik';
import { useCallback } from 'react';

import { projectExitFormFields } from '../components/ProjectExitFormContent/constants';
import { ProjectExitFormValues } from '../types';

export function useProjectExitForm() {
	const {
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		validateField,
		handleSubmit,
	} = useFormikContext<ProjectExitFormValues>();

	const handleFieldChange = useCallback(
		(field: string) => (value: unknown) => {
			setFieldValue(field, value);
			setFieldTouched(field, true);
			validateField(field);
		},
		[validateField]
	);

	const handleClosingReasonChange = useCallback(
		(value: ProjectExitFormValues['closingReason']) => {
			const closingReasonValue = value?.name ? value : null;

			setFieldValue(
				projectExitFormFields.closingReason.name,
				closingReasonValue
			);
			setFieldTouched(projectExitFormFields.closingReason.name, true);
			validateField(projectExitFormFields.closingReason.name);
		},
		[validateField]
	);

	return {
		values,
		errors,
		touched,
		handleFieldChange,
		handleClosingReasonChange,
		setFieldValue,
		setFieldTouched,
		validateField,
		handleSubmit,
	};
}
