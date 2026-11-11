import { useFormikContext } from 'formik';
import { useCallback, useMemo } from 'react';

import { InternshipFormFields, InternshipFormValues } from '../types';

import { setNumbersType1Value } from '@/helpers/setNumbersType1Value';
import { Dayjs } from '@/lib/dayjs';

export function useInternshipForm() {
	const {
		values,
		errors,
		touched,
		setFieldValue,
		setFieldTouched,
		submitForm,
		isSubmitting,
	} = useFormikContext<InternshipFormValues>();

	const handleFieldChange = useCallback(
		(fieldName: InternshipFormFields) => (value: unknown) => {
			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[setFieldValue, setFieldTouched]
	);

	const handleDateChange = useCallback(
		(fieldName: InternshipFormFields) => (value: Dayjs | null) => {
			setFieldValue(fieldName, value, true);
			setFieldTouched(fieldName, true);
		},
		[setFieldValue, setFieldTouched]
	);

	const handleNumberChange = useMemo(
		() => setNumbersType1Value(setFieldValue),
		[setFieldValue]
	);

	return {
		values,
		errors,
		touched,
		isSubmitting,
		submitForm,
		setFieldValue,
		handleFieldChange,
		handleDateChange,
		handleNumberChange,
	};
}
