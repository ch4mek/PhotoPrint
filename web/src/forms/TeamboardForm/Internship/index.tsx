import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';

import { InternshipFormContent } from './components/InternshipFormContent';
import { POPUP_TITLE } from './constants';
import { validationSchema } from './schema';
import { InternshipFormProps, InternshipFormValues } from './types';

export function InternshipForm({
	handleClose,
	onSubmit,
	userId,
	initialValues,
}: InternshipFormProps) {
	const handleSubmit = useCallback(
		async (
			values: InternshipFormValues,
			{ setSubmitting }: FormikHelpers<InternshipFormValues>
		) => {
			const isSubmitted = await onSubmit({ ...values, userId });

			setSubmitting(false);

			if (isSubmitted) {
				handleClose();
			}
		},
		[handleClose, onSubmit, userId]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={POPUP_TITLE}
			id={userId}
			{...{ initialValues, validationSchema }}
		>
			{({ handleOutsideClose }) => (
				<InternshipFormContent
					onClose={handleClose}
					handleOutsideClose={handleOutsideClose}
				/>
			)}
		</FormikWithBackup>
	);
}
