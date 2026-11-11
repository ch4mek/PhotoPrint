import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { useUserDevelopmentPlanById } from '@/queries/teamboard/useUserDevelopmentPlanById';

import { DevelopmentPlanFormContent } from './components/DevelopmentPlanFormContent';
import { developmentPlanFormInitialValues, FORM_NAME } from './constants';
import { developmentPlanValidationSchema } from './schema';
import { DevelopmentPlanFormProps, DevelopmentPlanFormValues } from './types';
import { convertDevelopmentPlanToFormInitialState } from './utils';

export function DevelopmentPlanForm({
	handleClose,
	onSubmit,
	id,
	getDevPlanById,
}: DevelopmentPlanFormProps) {
	const isEditMode = Boolean(id);

	const { data, isLoading } = useUserDevelopmentPlanById(id, {
		enabled: Boolean(id && (!getDevPlanById || !getDevPlanById(id))),
	});

	const initialValues: DevelopmentPlanFormValues = useMemo(() => {
		if (isEditMode && data) {
			return convertDevelopmentPlanToFormInitialState(data);
		}

		if (getDevPlanById && id) {
			return getDevPlanById(id);
		}

		return developmentPlanFormInitialValues;
	}, [isEditMode, data, getDevPlanById]);

	const handleFormSubmit = useCallback(
		async (
			values: DevelopmentPlanFormValues,
			{ setSubmitting }: FormikHelpers<DevelopmentPlanFormValues>
		) => {
			try {
				setSubmitting(true);

				await onSubmit(values);
			} finally {
				setSubmitting(false);
			}
		},
		[onSubmit]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={developmentPlanValidationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={id}
		>
			{(formikProps) => (
				<DevelopmentPlanFormContent
					{...{ formikProps, isEditMode, isLoading }}
					onClose={handleClose}
				/>
			)}
		</FormikWithBackup>
	);
}
